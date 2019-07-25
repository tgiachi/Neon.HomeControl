using Microsoft.IdentityModel.Tokens;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Entities.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Neon.HomeControl.Web.Auth
{
	public class AuthenticationManager
	{
		private readonly IDataAccess<UserEntity> _dataAccess;
		private readonly NeonConfig _neonConfig;

		public AuthenticationManager(NeonConfig neonConfig, IDataAccess<UserEntity> dataAccess)
		{
			_neonConfig = neonConfig;
			_dataAccess = dataAccess;
		}

		public string Authenticate(string username, string password)
		{
			var user = _dataAccess.Query()
				.SingleOrDefault(x => x.Email == username && x.Password == password.HashSha1());

			// return null if user not found
			if (user == null)
				return null;

			// authentication successful so generate jwt token
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_neonConfig.JwtToken);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim("Username", user.UserName),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim("FirstName", user.FirstName),
					new Claim("LastName", user.LastName),
					new Claim("Roles", $"[{string.Join(',', user.UsersRoles.Select(s => s.Role.Name))}]")
				}),
				Expires = DateTime.UtcNow.AddDays(15),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);


			return tokenHandler.WriteToken(token);
		}
	}
}