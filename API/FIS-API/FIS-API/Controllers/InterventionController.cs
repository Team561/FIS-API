using FIS_API.Dtos;
using FIS_API.Dtos.Outbound;
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
		public ActionResult<UserInterventionsOutDto> FetchUserInterventions(bool includeActive = false, bool includeInactive = false)
		{
			try
			{
				if (!includeActive && !includeInactive)
					return BadRequest("It is pointless.");

				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User)
						.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).FirstOrDefault(x => x.Email == email);

				UserInterventionsOutDto result = new();

				if (includeActive)
					foreach (FirefighterIntervention FFIntervention in userData.User.FirefighterInterventions)
						if (FFIntervention.Int.Active)
							result.ActiveInterventions.Add(InterventionOutDto.GetDtoFromIntervention(FFIntervention.Int));
				if (includeInactive)
					foreach (FirefighterIntervention FFIntervention in userData.User.FirefighterInterventions)
						if (!FFIntervention.Int.Active)
							result.InactiveInterventions.Add(InterventionOutDto.GetDtoFromIntervention(FFIntervention.Int));

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
		public ActionResult<IEnumerable<InterventionOutDto>> FetchCommanderInterventions(bool includeActive = false, bool includeInactive = false)
		{
			try
			{
				if (!includeActive && !includeInactive)
					return BadRequest("It is pointless.");

				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User)
						.Include(x => x.User.Interventions).FirstOrDefault(x => x.Email == email);

				UserInterventionsOutDto result = new();

				if (includeActive)
					foreach (Intervention intervention in userData.User.Interventions)
						if (intervention.Active)
							result.ActiveInterventions.Add(InterventionOutDto.GetDtoFromIntervention(intervention));
				if (includeInactive)
					foreach (Intervention intervention in userData.User.Interventions)
						if (!intervention.Active)
							result.InactiveInterventions.Add(InterventionOutDto.GetDtoFromIntervention(intervention));

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
		public ActionResult<FirefighterOutDto> FetchInterventionCommander(int InterventionId)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Rank)
					.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).ThenInclude(x => x.Type)
					.Include(x => x.User.FirefighterInterventions).ThenInclude(x => x.Int).ThenInclude(x => x.Cmdr)
					.FirstOrDefault(x => x.Email == email);

				var intervention = userData.User.FirefighterInterventions.FirstOrDefault(x => x.Int.IdInt == InterventionId)?.Int;
					
				if (intervention == null)
					return BadRequest("Invalid intervention ID or user did not participate in target intervention");

				return Ok(FirefighterOutDto.GetDtoFromFirefighter(intervention.Cmdr));
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
				return Ok(InterventionTypeOutDto.GetAllDtosFromContext(_context));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult CreateIntervention(InterventionInDto interventionData)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var targetChief = _context.Logins.Include(x => x.User).FirstOrDefault(x => x.Email == email).User.IdFf;

				Intervention newIntervention = new()
				{
					CmdrId = targetChief,
					Location = interventionData.Location,
					TypeId = interventionData.InterventionTypeId,
					Active = true
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
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var chiefData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Interventions)
					.Include(x => x.User.FireDepartments).ThenInclude(x => x.Firefighters)
					.FirstOrDefault(x => x.Email == email);

				var intervention = chiefData.User.Interventions.FirstOrDefault(x => x.IdInt == interventionID);
				if (intervention == null)
					return BadRequest("Bad intervention ID");

				if(_context.FirefighterInterventions.FirstOrDefault(x => ((x.FfId.ToString() == firefighterUID) && (x.IntId == interventionID))) != null)
					return BadRequest("Intervention - firefighter pair already exists");

				Firefighter firefighter = null;
				foreach(FireDepartment fd in chiefData.User.FireDepartments) // Should only be one
					foreach(Firefighter ff in fd.Firefighters)
						if(ff.IdFf.ToString() == firefighterUID)
						{
							var link = new FirefighterIntervention();
							link.FfId = ff.IdFf;
							link.IntId = intervention.IdInt;

							_context.FirefighterInterventions.Add(link);
							_context.SaveChanges();

							return Ok("Firefighter successfully added to intervention.");
						}

				return BadRequest("Bad firefighter ID");
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
		public ActionResult<List<FirefighterOutDto>> GetInterventionParticipants(int InterventionID)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var chiefData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Interventions).ThenInclude(x => x.FirefighterInterventions)
					.ThenInclude(x => x.Ff).FirstOrDefault(x => x.Email == email);

				var intervention = chiefData.User.Interventions.FirstOrDefault(x => x.IdInt == InterventionID);
				if (intervention == null)
				{
					return BadRequest("Bad interventionID");
				}

				var result = new List<FirefighterOutDto>();

				foreach (FirefighterIntervention ffInt in intervention.FirefighterInterventions)
				{
					result.Add(FirefighterOutDto.GetDtoFromFirefighter(ffInt.Ff));
				}

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
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult SetInterventionState(int interventionID, bool active = false)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var chiefData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Interventions)
					.FirstOrDefault(x => x.Email == email);

				var intervention = chiefData.User.Interventions.FirstOrDefault(x => x.IdInt == interventionID);
				if (intervention == null)
					return BadRequest("Bad intervention ID");

				intervention.Active = active;
				_context.SaveChanges();

				return Ok($"Intervention state successfully set to ${active}");
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
