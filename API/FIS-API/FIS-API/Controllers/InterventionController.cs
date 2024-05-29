using FIS_API.Dtos;
using FIS_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;

namespace FIS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InterventionController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly FirefighterDbContext _context;

		public InterventionController(IConfiguration configuration, FirefighterDbContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		[HttpPost("[action]")]
		[Authorize]
		public ActionResult<IEnumerable<Intervention>> FetchMyInterventions(bool includeInactive)
		{
			string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if(username == null)
			{
				return BadRequest("Token has no username. Wait, what?");
			}

			var userData = _context.Logins.Include(x => x.User).Include(x => x.User.Interventions).FirstOrDefault(x => x.Username == username);

			List<Intervention> result = new List<Intervention>();

			if (includeInactive)
			{
				foreach (Intervention Int in userData.User.Interventions)
				{
					result.Add(Int);
				}
			}
			else
			{
				foreach (Intervention Int in userData.User.Interventions)
				{
					if (Int.Active)
					{
						result.Add(Int);
					}
				}
			}

			return result;
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult CreateIntervention()
		{


			return BadRequest("Not yet implemented...");
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult AddFirefighterToIntervention(int interventionID, string firefighterUID)
		{


			return BadRequest("Not yet implemented...");
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult EndIntervention(int interventionID)
		{


			return BadRequest("Not yet implemented...");
		}
	}
}
