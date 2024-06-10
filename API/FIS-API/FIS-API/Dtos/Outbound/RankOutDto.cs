using FIS_API.Models;

namespace FIS_API.Dtos.Outbound
{
    public class RankOutDto
    {
        public int IdRank { get; set; }

        public string Name { get; set; } = null!;

        public static RankOutDto GetDtoFromRank(Rank rank)
        {
            var dto = new RankOutDto();

            dto.IdRank = rank.IdRank;
            dto.Name = rank.Name;

            return dto;
        }

        public static IEnumerable<RankOutDto> GetAllDtosFromContext(FirefighterDbContext context)
        {
            var result = new List<RankOutDto>();

            foreach (Rank IT in context.Ranks)
                result.Add(GetDtoFromRank(IT));

            return result;
        }
    }
}
