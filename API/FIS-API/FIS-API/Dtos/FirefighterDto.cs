using FIS_API.Models;
using System.Xml;

namespace FIS_API.Dtos
{
	public class FirefighterDto
	{
		public Guid FirefighterId { get; set; }

		public string Name { get; set; } = null!;

		public string Surname { get; set; } = null!;

		public DateTime? ActiveDate { get; set; }

		public bool Active { get; set; }

		public int RankId { get; set; }

		public FireDepartmentDto? FireDept { get; set; }

		internal static FirefighterDto GetDtoFromFirefighter(Firefighter firefighter, bool includeFireDept = false, bool includeChief = false)
		{
			var dto = new FirefighterDto();

			dto.FirefighterId = firefighter.IdFf;
			dto.Name = firefighter.Name;
			dto.Surname = firefighter.Surname;
			dto.ActiveDate = firefighter.ActiveDate;
			dto.Active = firefighter.Active;
			dto.RankId = firefighter.RankId;

			if (includeFireDept) dto.FireDept = FireDepartmentDto.GetDtoFromFireDept(firefighter.Fd, includeChief);

			return dto;
		}
	}
}
