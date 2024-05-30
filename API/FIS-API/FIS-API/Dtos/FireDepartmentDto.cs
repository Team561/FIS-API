using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class FireDepartmentDto
	{
		public Guid FireDepartmentId { get; set; }

		public string Name { get; set; } = null!;

		public string Location { get; set; } = null!;

		public bool Active { get; set; }

		public Guid? CommanderId { get; set; }

		public virtual FirefighterDto fireChief { get; set; } = null!;

		internal static FireDepartmentDto GetDtoFromFireDept(FireDepartment fireDept, bool includeFireChief = false)
		{
			var dto = new FireDepartmentDto();

			dto.FireDepartmentId = fireDept.IdFd;
			dto.Name = fireDept.Name;
			dto.Location = fireDept.Location;
			dto.Active = fireDept.Active;
			dto.CommanderId = fireDept.CmdrId;

			if (includeFireChief) dto.fireChief = FirefighterDto.GetDtoFromFirefighter(fireDept.Cmdr);

			return dto;
		}
	}
}
