using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	/// Utility class for SHA1 Hash
	/// </summary>
	public static class HashUtils
	{

		/// <summary>
		/// Hash input string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string HashSha1(this string input)
		{
			var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
			return string.Concat(hash.Select(b => b.ToString("x2")));
		}
	}
}