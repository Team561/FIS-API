using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FIS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{

		// POST api/<LoginController>
		[HttpPost("[action]")]
		public void LogIn([FromBody] string value)
		{
		}
	}
}
