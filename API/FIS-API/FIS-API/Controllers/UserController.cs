﻿using FIS_API.Dtos;
using FIS_API.Dtos.Inbound;
using FIS_API.Dtos.Outbound;
using FIS_API.Models;
using FIS_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FIS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly FirefighterDbContext _context;

		public UserController(IConfiguration configuration, FirefighterDbContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		// POST api/<LoginController>
		[HttpPost("[action]")]
		public ActionResult LogIn(LoginInDto loginData)
		{
			loginData.Username = loginData.Username.Trim();
			loginData.Password = loginData.Password.Trim();

			try
			{
				const string genericLoginFail = "Incorrect username or password";

				// Try to get a user from database
				var existingUser = _context.Logins.Include(x => x.User).Include(x => x.User.Rank).FirstOrDefault(x => x.Email == loginData.Username);
				if (existingUser == null)
					return BadRequest(genericLoginFail);

				// Check is password hash matches
				var b64hash = PasswordHashProvider.GetHash(loginData.Password, existingUser.PasswordSalt);
				if (b64hash != existingUser.PasswordHash)
					return BadRequest(genericLoginFail);

				// Create and return JWT token
				var secureKey = _configuration["JWT:SecureKey"];

				var serializedToken =
					JwtTokenProvider.CreateToken(
						secureKey,
						3600,
						loginData.Username,
						existingUser.User.Rank.Name);

				return Ok(serializedToken);
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
		public ActionResult<FirefighterOutDto> FetchPersonalData(bool includeAdditionalData = false)
		{
			try
			{
				string email = JwtTokenProvider.ReadMailFromToken(User);

				FirefighterOutDto result = null;

				if (includeAdditionalData)
				{
					var userData = _context.Logins.Include(x => x.User)
						.Include(x => x.User.Fd).ThenInclude(x => x.Cmdr).FirstOrDefault(x => x.Email == email);

					result = FirefighterOutDto.GetDtoFromFirefighter(userData.User, true, true);
				}
				else
				{
					var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.Rank).FirstOrDefault(x => x.Email == email);

					result = FirefighterOutDto.GetDtoFromFirefighter(userData.User);
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
		[Authorize]
		public ActionResult FetchRanks()
		{
			try
			{
				return Ok(RankOutDto.GetAllDtosFromContext(_context));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		[Authorize(Roles = "Fire Fighter Commander")]
		public ActionResult<CommanderFirefighterListOutDto> FetchCommanderFirefighters(bool includeActive, bool includeInactive)
		{
			try
			{
				if(!includeActive && !includeInactive)
					return BadRequest("It is pointless.");

				string email = JwtTokenProvider.ReadMailFromToken(User);

				var userData = _context.Logins.Include(x => x.User).ThenInclude(x => x.FireDepartments).ThenInclude(x => x.Firefighters).FirstOrDefault(x => x.Email == email);

				CommanderFirefighterListOutDto result = new();

				if (includeActive)
					foreach (FireDepartment fd in userData.User.FireDepartments)
						foreach (Firefighter ff in fd.Firefighters)
							if (ff.Active)
								result.ActiveFirefighters.Add(FirefighterOutDto.GetDtoFromFirefighter(ff));
				if (includeInactive)
					foreach (FireDepartment fd in userData.User.FireDepartments)
						foreach (Firefighter ff in fd.Firefighters)
							if (!ff.Active)
									result.InactiveFirefighters.Add(FirefighterOutDto.GetDtoFromFirefighter(ff));

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
		public ActionResult GenerateSaltAndHashForPassword(IWebHostEnvironment env, string password)
		{

			try
			{
				IPAddress addr = System.Net.IPAddress.Parse(HttpContext.Connection.RemoteIpAddress.ToString());
				if (!System.Net.IPAddress.IsLoopback(addr))
				{
					return BadRequest("Localhost only, sorry");
				}

				if (!env.IsDevelopment())
				{
					return BadRequest("Dev mode only, sorry");
				}

				password = password.Trim();

				// Hash the password
				string b64salt = PasswordHashProvider.GetSalt();
				string b64hash = PasswordHashProvider.GetHash(password, b64salt);

				List<string> result = new List<string>() { "PASS: [" + password + ']', "SALT: [" + b64salt + ']', "HASH: [" + b64hash + ']' };

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
