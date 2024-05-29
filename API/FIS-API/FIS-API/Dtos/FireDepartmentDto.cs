using FIS_API.Models;

namespace FIS_API.Dtos
{
	public class FireDepartmentDto
	{
		public Guid FireDepartmentId { get; set; }

		public string Name { get; set; } = null!;

		public string Location { get; set; } = null!;

		public bool Active { get; set; }

		public virtual FirefighterDto? Commander { get; set; }

		public virtual ICollection<FirefighterDto> Firefighters { get; set; } = new List<FirefighterDto>();

		internal static FireDepartmentDto GetDtoFromFireDept(FireDepartment fireDept, bool includeCmdr = false, bool includeFirefighters = false)
		{
			var dto = new FireDepartmentDto();

			dto.FireDepartmentId = fireDept.IdFd;
			dto.Name = fireDept.Name;
			dto.Location = fireDept.Location;
			dto.Active = fireDept.Active;

			if (includeCmdr)
				dto.Commander = FirefighterDto.GetDtoFromFirefighter(fireDept.Cmdr);
			if(includeFirefighters)
				foreach (Firefighter ff in fireDept.Firefighters)
					dto.Firefighters.Add(FirefighterDto.GetDtoFromFirefighter(ff));

			return dto;
		}
	}
}
