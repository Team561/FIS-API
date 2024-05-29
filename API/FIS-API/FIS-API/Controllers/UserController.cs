using FIS_API.Dtos;
using FIS_API.Models;
using FIS_API.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
		public ActionResult LogIn(LoginDto loginData)
		{
			try
			{
				loginData.Username = loginData.Username.Trim();
				loginData.Password = loginData.Password.Trim();

				var genericLoginFail = "Incorrect username or password";

				// Try to get a user from database
				var existingUser = _context.Logins.Include(x => x.User).Include(x => x.User.Rank).FirstOrDefault(x => x.Username == loginData.Username);
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
						360,
						loginData.Username,
						existingUser.User.Rank.Name);

				return Ok(serializedToken);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost("[action]")]
		public ActionResult GetSaltAndHashForPassword(IWebHostEnvironment env, string password)
		{
			if(!env.IsDevelopment())
			{
				return BadRequest("DEV MODE ONLY, BUZZ OFF");
			}

			try
			{
				password = password.Trim();

				// Hash the password
				string b64salt = PasswordHashProvider.GetSalt();
				string b64hash = PasswordHashProvider.GetHash(password, b64salt);

				List<string> result = new List<string>() { "PASS: [" + password + ']', "SALT: [" + b64salt + ']', "HASH: [" + b64hash + ']' };

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
