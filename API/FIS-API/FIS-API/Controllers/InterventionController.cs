using FIS_API.Dtos;
using FIS_API.Models;
using FIS_API.Security;
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
		public ActionResult<IEnumerable<InterventionDto>> FetchUserInterventions(bool includeInactive = false, bool includeCommanders = false)
		{
			try
			{
				string username = JWTUsernameReader.Read(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Rank)
					.Include(x => x.User.Interventions).ThenInclude(x => x.Type).FirstOrDefault(x => x.Username == username);

				List<InterventionDto> result = new List<InterventionDto>();

				if (includeInactive)
					foreach (Intervention Int in userData.User.Interventions)
						result.Add(InterventionDto.GetDtoFromIntervention(Int, includeCommanders));
				else
					foreach (Intervention Int in userData.User.Interventions)
						if (Int.Active)
							result.Add(InterventionDto.GetDtoFromIntervention(Int, includeCommanders));

				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize]
		public ActionResult<InterventionDto> FetchUserInterventionCmdr()
		{
			try
			{
				string username = JWTUsernameReader.Read(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Rank)
					.Include(x => x.User.Interventions).ThenInclude(x => x.Type).FirstOrDefault(x => x.Username == username);


				return BadRequest("Not yet implemented...");
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult CreateIntervention(InterventionDto interventionData)
		{
			try
			{
				string username = JWTUsernameReader.Read(User);

				return BadRequest("Not yet implemented...");
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult AddFirefighterToIntervention(int interventionID, string firefighterUID)
		{
			try
			{
				return BadRequest("Not yet implemented...");
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult EndIntervention(int interventionID)
		{
			try
			{
				return BadRequest("Not yet implemented...");
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
