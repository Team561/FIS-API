using FIS_API.Models;
using System.Xml;

namespace FIS_API.Dtos.Outbound
{
    public class FirefighterOutDto
    {
        public Guid FirefighterId { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public DateTime? ActiveDate { get; set; }

        public bool Active { get; set; }

        public int RankId { get; set; }

        public FireDepartmentOutDto? FireDept { get; set; }

        internal static FirefighterOutDto GetDtoFromFirefighter(Firefighter firefighter, bool includeFireDept = false, bool includeChief = false)
        {
            var dto = new FirefighterOutDto();

            dto.FirefighterId = firefighter.IdFf;
            dto.Name = firefighter.Name;
            dto.Surname = firefighter.Surname;
            dto.ActiveDate = firefighter.ActiveDate;
            dto.Active = firefighter.Active;
            dto.RankId = firefighter.RankId;

            if (includeFireDept) dto.FireDept = FireDepartmentOutDto.GetDtoFromFireDept(firefighter.Fd, includeChief);

            return dto;
        }
    }
}
