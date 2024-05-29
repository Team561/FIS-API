using System.Security.Claims;

namespace FIS_API.Security
{
	public class JWTUsernameReader
	{
		public static string Read(ClaimsPrincipal User)
		{
			string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (username == null)
			{
				throw new BadHttpRequestException("Token has no username. Wait, what? How did you get here? Get the hell out of my house.");
			}

			return username;
		}
	}
}
