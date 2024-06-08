using FIS_API.Dtos;
using FIS_API.Models;
using System.Collections.Concurrent;
using static FIS_API.Logic.InvitationHandler;

namespace FIS_API.Logic
{
	public static class InvitationHandler
	{
		private static volatile List<KeyValuePair<Guid, Invitations>> commanderData = new();

		/// <summary>
		/// Returns false if invitation already exists
		/// </summary>
		public static bool AddInvitation(Guid commanderID, InvitationDto invitation, IConfiguration config)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.Key == commanderID);

				if (target.Value == null)
				{
					commanderData.Add(new KeyValuePair<Guid, Invitations>(commanderID, new Invitations(commanderID, config)));
					target = commanderData.Last();
				}

				return target.Value.Add(invitation, config);
			}
		}

		public static bool RemoveInvitation(Guid commanderID, InvitationDto dto)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.Key == commanderID);

				if (target.Value == null)
					return false;

				return target.Value.Remove(dto);
			}
		}

		public static IEnumerable<int> GetFirefighterInvitations(Guid commanderID, Guid firefighterID)
		{
			lock (commanderData)
			{
				var cmdr = commanderData.FirstOrDefault(x => x.Key == commanderID);
				if (cmdr.Value == null)
				{
					return null!;
				}

				return cmdr.Value.getAllFromFirefighterID(firefighterID);
			}
		}

		public static IEnumerable<InvitationDto> GetSentInvitations(Guid commanderID)
		{
			lock (commanderData)
			{
				var cmdr = commanderData.FirstOrDefault(x => x.Key == commanderID);
				if (cmdr.Value == null)
				{
					return null!;
				}

				return InvitationDto.getDtoEnumerableFromObjectEnumerable(cmdr.Value.invitations);
;
			}
		}

		public class Invitations
		{
			public volatile LinkedList<Invitation> invitations = new();
			private readonly Guid key;

			private static volatile System.Threading.Timer? tickTock;

			public Invitations(Guid _key, IConfiguration config)
			{
				key = _key;

				var targetTime = DateTime.Now;
				targetTime = targetTime.AddMinutes(double.Parse(config["Time:InterventionRecoveryTimeoutMinutes"]));

				tickTock = new System.Threading.Timer(onTimedEvent, this, (long)(targetTime - DateTime.Now).TotalMilliseconds, (long)(targetTime - DateTime.Now).TotalMilliseconds);
			}

			private static void onTimedEvent(object? state)
			{
				var invitations = (Invitations)state;

				var node = invitations.invitations.First;

				while (node != null)
				{
					var next = node.Next;

					var comparison = DateTime.Compare(node.Value.expirationTime, DateTime.Now);

					if (comparison > 0)
						invitations.invitations.Remove(node);
					else
						break; // these node values are always ordered

					node = next;
				}

				invitations.RemoveLogic();
			}

			public IEnumerable<int> getAllFromFirefighterID(Guid firefighterID)
			{
				var target = invitations.Where(x => x.firefighterID == firefighterID).ToList();

				var result = new List<int>();
				foreach (var item in target)
					result.Add(item.interventionID);

				return result;
			}

			/// <summary>
			/// Returns false if list already has invitation
			/// </summary>
			public bool Add(InvitationDto dto, IConfiguration config)
			{
				var targetTime = DateTime.Now;
				targetTime = targetTime.AddMinutes(double.Parse(config["Time:InterventionRecoveryTimeoutMinutes"]));

				bool result = false;
				foreach (Invitation target in invitations)
					if (target.firefighterID == dto.firefighterID && target.interventionID == dto.interventionID)
						return false;

				invitations.AddLast(new Invitation() { firefighterID = dto.firefighterID, interventionID = dto.interventionID, expirationTime = targetTime });
				return true;
			}

			public bool Remove(InvitationDto dto)
			{
				bool result = false;

				var node = invitations.First;
				while (node != null)
				{
					var next = node.Next;

					if (node.Value.interventionID == dto.interventionID && node.Value.firefighterID == dto.firefighterID)
					{
						invitations.Remove(node);
						result = true;
						break;
					}

					node = next;
				}

				RemoveLogic();

				return result;
			}

			private void RemoveLogic()
			{

				if (invitations.Count <= 0)
				{
					tickTock.Dispose();

					int index = 0;
					foreach (var kvp in InvitationHandler.commanderData)
					{
						if (kvp.Key == key)
							break;
						index++;
					}
					InvitationHandler.commanderData.RemoveAt(index);
				}
			}
		}

		public class Invitation
		{
			public Guid firefighterID;
			public int interventionID;
			public DateTime expirationTime;
		}
	}
}
