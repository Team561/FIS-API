using static FIS_API.Logic.InvitationHandler;

namespace FIS_API.Dtos
{
	public class InvitationDto
	{
		public Guid firefighterID;
		public int interventionID;

		public static InvitationDto getDtoFromObject(Invitation invitation)
		{
			InvitationDto dto = new();

			dto.firefighterID = invitation.firefighterID;
			dto.interventionID = invitation.interventionID;

			return dto;
		}

		public static IEnumerable<InvitationDto> getDtoEnumerableFromObjectEnumerable(IEnumerable<Invitation> invitations)
		{
			List<InvitationDto> dtos = new();

			foreach (var invitation in invitations)
				dtos.Add(getDtoFromObject(invitation));

			return dtos;
		}
	}
}
