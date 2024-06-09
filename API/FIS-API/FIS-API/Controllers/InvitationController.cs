using FIS_API.Dtos;
using FIS_API.Dtos.Outbound;
using FIS_API.Logic;
using FIS_API.Models;
using FIS_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvitationController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly FirefighterDbContext _context;

		public InvitationController(IConfiguration configuration, FirefighterDbContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		[HttpPost("[action]")]
		[Authorize]
		public ActionResult<IEnumerable<InvitationDto>> FetchUserInvitations()
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Fd).ThenInclude(x => x.Cmdr).FirstOrDefault(x => x.Email == email);

				IEnumerable<int> ids = InvitationHandler.LockGetFirefighterInvitations(userData.User.Fd.Cmdr.IdFf, userData.UserGuid);

				var interventions = _context.Interventions.Where(x => ids.Contains(x.IdInt)).ToList();

				List<InterventionOutDto> result = new();
				foreach (var intervention in interventions)
					if (intervention.Active) // Do not display invitations tied to inactive interventions to firefighters, instead simply let them die on their own (in case the intervention gets recovered)
						result.Add(InterventionOutDto.GetDtoFromIntervention(intervention));

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
		public ActionResult AcceptIntervnetionInvitation(int interventionID)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Fd).ThenInclude(x => x.Cmdr).ThenInclude(x => x.Interventions)
					.Include(x => x.User).ThenInclude(x => x.Fd).ThenInclude(x => x.Cmdr).ThenInclude(x => x.FirefighterInterventions).FirstOrDefault(x => x.Email == email);

				var result = InvitationHandler.LockRemoveInvitation(userData.User.Fd.Cmdr.IdFf, new InvitationDto() { FirefighterID = userData.UserGuid, InterventionID = interventionID });

				if (!result)
					return BadRequest("The invitation does not exist");

				// If this is the first accepted invitation, the commander must be added to the intervention as well.
				bool shouldActivate = true;

				foreach (var FFIntervention in userData.User.Fd.Cmdr.FirefighterInterventions)
					if (FFIntervention.IntId == interventionID)
					{
						shouldActivate = false;
						break;
					}

				if (shouldActivate)
				{
					FirefighterIntervention FFIntervention = new();
					FFIntervention.IntId = interventionID;
					FFIntervention.FfId = userData.User.Fd.Cmdr.IdFf;

					_context.FirefighterInterventions.Add(FFIntervention);
				}

				FirefighterIntervention FirefighterIntervention = new();
				FirefighterIntervention.IntId = interventionID;
				FirefighterIntervention.FfId = userData.UserGuid;

				_context.FirefighterInterventions.Add(FirefighterIntervention);

				_context.SaveChanges();

				return Ok("Invitation accepted successfully.");
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
		public ActionResult DeclineIntervnetionInvitation(int interventionID)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Fd).ThenInclude(x => x.Cmdr).ThenInclude(x => x.Interventions)
					.Include(x => x.User).ThenInclude(x => x.Fd).ThenInclude(x => x.Cmdr).ThenInclude(x => x.FirefighterInterventions).FirstOrDefault(x => x.Email == email);

				var result = InvitationHandler.LockRemoveInvitation(userData.User.Fd.Cmdr.IdFf, new InvitationDto() { FirefighterID = userData.UserGuid, InterventionID = interventionID });

				if (!result)
					return BadRequest("The invitation does not exist");

				return Ok();
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
		public ActionResult InviteFirefighterToIntervention(int interventionID, Guid firefighterID)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var chiefData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Interventions).FirstOrDefault(x => x.Email == email);

				if (chiefData.UserGuid == firefighterID)
					return BadRequest("You cannot invite yourself to an intervention");

				var userData = _context.Firefighters.Include(x => x.FirefighterInterventions).FirstOrDefault(x => x.IdFf == firefighterID);//chiefData.User.Fd.Firefighters.FirstOrDefault(x => x.IdFf == firefighterID);

				if (userData == null)
					return BadRequest("This firefighter is not under your command or does not exist");

				foreach (var FFIntervention in userData.FirefighterInterventions)
					if (FFIntervention.IntId == interventionID)
						return BadRequest("User already asigned to intervention");

				var intervention = chiefData.User.Interventions.FirstOrDefault(x => x.IdInt == interventionID);

				if (intervention == null)
					return BadRequest("The intervention does not exist");

				if (intervention.Active == false)
					return BadRequest("The intervention is not active");

				var result = InvitationHandler.LockAddInvitation(chiefData.UserGuid, new InvitationDto() { FirefighterID = firefighterID, InterventionID = interventionID }, _configuration);

				if (!result)
					return BadRequest("The invitation already exists");

				return Ok("Invitation sent successfully");
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
		public ActionResult<IEnumerable<InvitationDto>> FetchCommanderInvitations()
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.FirstOrDefault(x => x.Email == email);

				var result = InvitationHandler.LockGetCommanderInvitations(userData.UserGuid);

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
	}
}
