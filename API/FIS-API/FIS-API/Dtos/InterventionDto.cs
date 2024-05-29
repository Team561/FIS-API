using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class InterventionDto
	{
		public int InterventionId { get; set; }

		public string Location { get; set; }

		public bool Active { get; set; }

		public string InterventionType { get; set; }

		public FirefighterDto? Commander { get; set; }

		public static InterventionDto GetDtoFromIntervention(Intervention intervention, bool includeCommander = false)
		{
			var dto = new InterventionDto();

			dto.InterventionId = intervention.IdInt;
			dto.Location = intervention.Location;
			dto.Active = intervention.Active;
			dto.InterventionType = intervention.Type.Name;

			if(includeCommander) dto.Commander = FirefighterDto.GetDtoFromFirefighter(intervention.Cmdr);

			return dto;
		}
	}
}
