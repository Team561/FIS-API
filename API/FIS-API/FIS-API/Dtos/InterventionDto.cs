using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class InterventionDto
	{
		public int InterventionId { get; set; }

		public string Location { get; set; }

		public bool Active { get; set; }

		public int InterventionTypeId { get; set; }

		public Guid CommanderId { get; set; }

		public static InterventionDto GetDtoFromIntervention(Intervention intervention)
		{
			var dto = new InterventionDto();

			dto.InterventionId = intervention.IdInt;
			dto.Location = intervention.Location;
			dto.Active = intervention.Active;
			dto.InterventionTypeId = intervention.TypeId;
			dto.CommanderId = intervention.CmdrId;

			return dto;
		}
	}
}
