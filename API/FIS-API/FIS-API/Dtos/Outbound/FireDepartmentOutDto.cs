using FIS_API.Models;

namespace FIS_API.Dtos.Outbound
{
    public class FireDepartmentOutDto
    {
        public Guid FireDepartmentId { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public bool Active { get; set; }

        public Guid? CommanderId { get; set; }

        public virtual FirefighterOutDto fireChief { get; set; } = null!;

        public static FireDepartmentOutDto GetDtoFromFireDept(FireDepartment fireDept, bool includeFireChief = false)
        {
            var dto = new FireDepartmentOutDto();

            dto.FireDepartmentId = fireDept.IdFd;
            dto.Name = fireDept.Name;
            dto.Location = fireDept.Location;
            dto.Active = fireDept.Active;
            dto.CommanderId = fireDept.CmdrId;

            if (includeFireChief) dto.fireChief = FirefighterOutDto.GetDtoFromFirefighter(fireDept.Cmdr);

            return dto;
        }
    }
}
