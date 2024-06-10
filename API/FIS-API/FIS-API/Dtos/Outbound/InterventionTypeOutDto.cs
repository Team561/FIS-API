using FIS_API.Models;

namespace FIS_API.Dtos.Outbound
{
    public class InterventionTypeOutDto
    {
        public int IdType { get; set; }

        public string Name { get; set; } = null!;

        public static InterventionTypeOutDto GetDtoFromInterventionType(InterventionType intervention)
        {
            var dto = new InterventionTypeOutDto();

            dto.IdType = intervention.IdType;
            dto.Name = intervention.Name;

            return dto;
        }

        public static IEnumerable<InterventionTypeOutDto> GetAllDtosFromContext(FirefighterDbContext context)
        {
            var result = new List<InterventionTypeOutDto>();

            foreach (InterventionType IT in context.InterventionTypes)
                result.Add(GetDtoFromInterventionType(IT));

            return result;
        }
    }
}
