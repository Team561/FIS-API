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
		public ActionResult<IEnumerable<InterventionDto>> FetchUserInterventions(bool includeInactive = false)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User)
						.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).FirstOrDefault(x => x.Email == email);

				List<InterventionDto> result = new List<InterventionDto>();

				if (includeInactive)
					foreach (FirefighterIntervention intervention in userData.User.FirefighterInterventions)
						result.Add(InterventionDto.GetDtoFromIntervention(intervention.Int));
				else
					foreach (FirefighterIntervention intervention in userData.User.FirefighterInterventions)
						if (intervention.Int.Active)
							result.Add(InterventionDto.GetDtoFromIntervention(intervention.Int));

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
		public ActionResult<IEnumerable<InterventionDto>> FetchCommanderInterventions(bool includeInactive = false)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User)
						.Include(x => x.User.Interventions).FirstOrDefault(x => x.Email == email);

				List<InterventionDto> result = new List<InterventionDto>();

				if (includeInactive)
					foreach (Intervention intervention in userData.User.Interventions)
						result.Add(InterventionDto.GetDtoFromIntervention(intervention));
				else
					foreach (Intervention intervention in userData.User.Interventions)
						if (intervention.Active)
							result.Add(InterventionDto.GetDtoFromIntervention(intervention));

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
		public ActionResult<InterventionDto> FetchUserInterventionCommander(int InterventionId)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Rank)
					.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).ThenInclude(x => x.Type)
					.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).ThenInclude(x => x.Cmdr)
					.FirstOrDefault(x => x.Email == email);

				var intervention = userData.User.FirefighterInterventions.FirstOrDefault(x => x.Int.IdInt == InterventionId).Int;
					
				if (intervention == null)
					return BadRequest("Invalid intervention ID (or user unauthorized),");

				return Ok(FirefighterDto.GetDtoFromFirefighter(intervention.Cmdr));
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
		public ActionResult FetchInterventionTypes()
		{
			try
			{
				return Ok(InterventionTypeDto.GetAllDtosFromContext(_context));
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
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var targetChief = _context.Logins.Include(x => x.User).FirstOrDefault(x => x.Email == email).User.IdFf;

				Intervention newIntervention = new()
				{
					CmdrId = targetChief,
					Location = interventionData.Location,
					TypeId = interventionData.InterventionTypeId
				};

				_context.Interventions.Add(newIntervention);
				_context.SaveChanges();

				return Ok("Intervention created successfully.");
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
		public ActionResult SetInterventionState(int interventionID, bool active)
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
