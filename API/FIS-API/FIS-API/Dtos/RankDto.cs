using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class RankDto
	{
		public int IdRank { get; set; }

		public string Name { get; set; } = null!;

		public static RankDto GetDtoFromRank(Rank rank)
		{
			var dto = new RankDto();

			dto.IdRank = rank.IdRank;
			dto.Name = rank.Name;

			return dto;
		}

		public static IEnumerable<RankDto> GetAllDtosFromContext(FirefighterDbContext context)
		{
			var result = new List<RankDto>();

			foreach (Rank IT in context.Ranks)
				result.Add(RankDto.GetDtoFromRank(IT));

			return result;
		}
	}
}
