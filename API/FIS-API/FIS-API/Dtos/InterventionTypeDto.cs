using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class InterventionTypeDto
	{
		public int IdType { get; set; }

		public string Name { get; set; } = null!;

		public static InterventionTypeDto GetDtoFromInterventionType(InterventionType intervention)
		{
			var dto = new InterventionTypeDto();

			dto.IdType = intervention.IdType;
			dto.Name = intervention.Name;

			return dto;
		}

		public static IEnumerable<InterventionTypeDto> GetAllDtosFromContext(FirefighterDbContext context)
		{
			var result = new List<InterventionTypeDto>();

			foreach (InterventionType IT in context.InterventionTypes)
				result.Add(InterventionTypeDto.GetDtoFromInterventionType(IT));

			return result;
		}
	}
}
