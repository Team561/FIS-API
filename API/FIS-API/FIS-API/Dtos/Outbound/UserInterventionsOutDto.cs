namespace FIS_API.Dtos.Outbound
{
    public class UserInterventionsOutDto
	{
		public List<InterventionOutDto> ActiveInterventions { get; set; }
		public List<InterventionOutDto> InactiveInterventions { get; set; }
		public UserInterventionsOutDto()
		{
			ActiveInterventions = new();
			InactiveInterventions = new();
		}
	}
}
