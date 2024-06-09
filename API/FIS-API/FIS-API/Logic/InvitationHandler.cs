using FIS_API.Dtos;
using FIS_API.Models;
using System.Collections.Concurrent;
using static FIS_API.Logic.InvitationHandler;

namespace FIS_API.Logic
{
	public static class InvitationHandler
	{
		private static volatile List<Invitations> commanderData = new();

		/// <summary>
		/// Returns false if invitation already exists
		/// </summary>
		public static bool LockAddInvitation(Guid commanderID, InvitationDto invitation, IConfiguration config)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.linkedCommanderID == commanderID);

				if (target == null)
				{
					commanderData.Add(new Invitations(commanderID, config));
					target = commanderData.Last();
				}

				return target.Add(invitation, config);
			}
		}

		public static bool LockRemoveInvitation(Guid commanderID, InvitationDto dto)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.linkedCommanderID == commanderID);

				if (target == null)
					return false;

				return target.Remove(dto);
			}
		}

		public static IEnumerable<int> LockGetFirefighterInvitations(Guid commanderID, Guid firefighterID)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.linkedCommanderID == commanderID);
				if (target == null)
				{
					return null!;
				}

				return target.getAllFromFirefighterID(firefighterID);
			}
		}

		public static IEnumerable<InvitationDto> LockGetCommanderInvitations(Guid commanderID)
		{
			lock (commanderData)
			{
				var target = commanderData.FirstOrDefault(x => x.linkedCommanderID == commanderID);
				if (target == null)
				{
					return null!;
				}

				return InvitationDto.getDtoEnumerableFromObjectEnumerable(target.invitations);
;
			}
		}

		public class Invitations
		{
			public volatile LinkedList<Invitation> invitations = new();
			public readonly Guid linkedCommanderID;

			private static volatile System.Threading.Timer? tickTock;

			public Invitations(Guid _commanderID, IConfiguration config)
			{
				linkedCommanderID = _commanderID;

				var targetTime = DateTime.Now;
				targetTime = targetTime.AddMinutes(double.Parse(config["Time:InterventionRecoveryTimeoutMinutes"]));

				tickTock = new System.Threading.Timer(LockOnTimedEvent, this, (long)(targetTime - DateTime.Now).TotalMilliseconds, (long)(targetTime - DateTime.Now).TotalMilliseconds);
			}

			private static void LockOnTimedEvent(object? state)
			{
				// usually we're safe as everything goes through the handler first, but now we gotta lock this one properly
				lock (InvitationHandler.commanderData)
				{
					var invitations = (Invitations)state;

					var node = invitations.invitations.First;

					while (node != null)
					{
						var next = node.Next;

						var comparison = DateTime.Compare(node.Value.expirationTime, DateTime.Now);

						if (comparison < 0)
							invitations.invitations.Remove(node);
						else
							break; // these node values are always ordered

						node = next;
					}

					invitations.SelfDestructIfEmpty();
				}
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

				foreach (Invitation target in invitations)
					if (target.firefighterID == dto.FirefighterID && target.interventionID == dto.InterventionID)
						return false;

				invitations.AddLast(new Invitation() { firefighterID = dto.FirefighterID, interventionID = dto.InterventionID, expirationTime = targetTime });
				return true;
			}

			public bool Remove(InvitationDto dto)
			{
				bool result = false;

				var node = invitations.First;
				while (node != null)
				{
					var next = node.Next;

					if (node.Value.interventionID == dto.InterventionID && node.Value.firefighterID == dto.FirefighterID)
					{
						invitations.Remove(node);
						result = true;
						break;
					}

					node = next;
				}

				SelfDestructIfEmpty();

				return result;
			}

			private void SelfDestructIfEmpty()
			{

				if (invitations.Count <= 0)
				{
					tickTock.Dispose();

					int index = 0;
					foreach (var kvp in InvitationHandler.commanderData)
					{
						if (kvp.linkedCommanderID == linkedCommanderID)
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
