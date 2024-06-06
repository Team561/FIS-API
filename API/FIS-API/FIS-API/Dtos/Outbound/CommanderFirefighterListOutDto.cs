namespace FIS_API.Dtos.Outbound
{
	public class CommanderFirefighterListOutDto
	{
		public List<FirefighterOutDto> ActiveFirefighters { get; set; }
		public List<FirefighterOutDto> InactiveFirefighters { get; set; }
		public CommanderFirefighterListOutDto()
		{
			ActiveFirefighters = new();
			InactiveFirefighters = new();
		}
	}
}
