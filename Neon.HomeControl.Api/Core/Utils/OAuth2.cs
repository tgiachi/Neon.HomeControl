using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	///     A simple C# OAuth2 auth-code wrapper library.
	/// </summary>
	public class OAuth2
	{
		#region Class functionality

		/// <summary>
		///     Construct a OAuth2 forwarding URI to redirect with.
		/// </summary>
		/// <param name="provider">OAuth2 provider wrapper.</param>
		/// <param name="redirectUri">URI to redirect back to the system.</param>
		/// <param name="locale">Language locale for provider interface.</param>
		/// <returns>URI to redirect system to, for user authorization.</returns>
		public static string CreateRedirect(OAuth2Provider provider, string redirectUri, string locale = "en")
		{
			var parameters = new Dictionary<string, string>
			{
				{"client_id", provider.ClientId},

				{"display", "page"},

				{"locale", locale},

				{"redirect_uri", redirectUri},

				{"response_type", "code"}
			};


			if (provider.Offline)

				parameters.Add(
					"access_type",
					"offline");


			if (!string.IsNullOrWhiteSpace(provider.Scope))

				parameters.Add(
					"scope",
					provider.Scope);


			if (!string.IsNullOrWhiteSpace(provider.State))

				parameters.Add(
					"state",
					provider.State);


			var qs = buildQueryString(parameters);

			var url =
				provider.AuthUri + "?" +
				qs;


			return url;
		}


		/// <summary>
		///     Request a access token by exchanging a auth code.
		/// </summary>
		/// <param name="provider">OAuth2 provider wrapper.</param>
		/// <param name="redirectUri">URI to redirect back to the system.</param>
		/// <param name="code">Authorization code.</param>
		/// <returns>Authentication response object.</returns>
		public static OAuth2AuthenticateResponse AuthenticateByCode(OAuth2Provider provider, string redirectUri,
			string code)
		{
			var parameters = new Dictionary<string, string>
			{
				{"client_id", provider.ClientId},

				{"client_secret", provider.ClientSecret},

				{"redirect_uri", redirectUri},

				{"code", code},

				{"grant_type", "authorization_code"}
			};


			if (!string.IsNullOrWhiteSpace(provider.Scope))

				parameters.Add(
					"scope",
					provider.Scope);


			if (!string.IsNullOrWhiteSpace(provider.State))

				parameters.Add(
					"state",
					provider.State);


			var reply = request(
				provider.AccessTokenUri,
				payload: buildQueryString(parameters));


			return interpretReply(reply);
		}


		/// <summary>
		///     Request a new access token by refreshing an old.
		/// </summary>
		/// <param name="provider">OAuth2 provider wrapper.</param>
		/// <param name="refreshToken">Access/refresh token to use.</param>
		/// <returns>Authentication response object.</returns>
		public static OAuth2AuthenticateResponse AuthenticateByToken(OAuth2Provider provider, string refreshToken)
		{
			var parameters = new Dictionary<string, string>
			{
				{"client_id", provider.ClientId},

				{"client_secret", provider.ClientSecret},

				{"refresh_token", refreshToken},

				{"grant_type", "refresh_token"}
			};


			if (!string.IsNullOrWhiteSpace(provider.Scope))

				parameters.Add(
					"scope",
					provider.Scope);


			if (!string.IsNullOrWhiteSpace(provider.State))

				parameters.Add(
					"state",
					provider.State);


			var reply = request(
				provider.AccessTokenUri,
				payload: buildQueryString(parameters));


			return interpretReply(reply);
		}


		/// <summary>
		///     Get user info from the providers user endpoint.
		/// </summary>
		/// <param name="provider">OAuth2 provider wrapper.</param>
		/// <param name="accessToken">Access token to use.</param>
		/// <returns>Raw data from the provider.</returns>
		public static string GetUserInfo(OAuth2Provider provider, string accessToken)
		{
			var parameters = new Dictionary<string, string>
			{
				{"access_token", accessToken}
			};


			return request(
				provider.UserInfoUri,
				"GET",
				buildQueryString(parameters));
		}

		#endregion

		#region Helper functions

		/// <summary>
		///     Construct a query-string from dictionary.
		/// </summary>
		/// <param name="parameters">Set of parameters in dictionary form to construct from.</param>
		/// <returns>Query-string.</returns>
		private static string buildQueryString(Dictionary<string, string> parameters)
		{
			return parameters.Aggregate("", (c, p) => c + "&" + p.Key + "=" + HttpUtility.UrlEncode(p.Value))
				.Substring(1);
		}


		/// <summary>
		///     Interpret the reply from the auth-call.
		/// </summary>
		/// <param name="reply">The string body from the web-request.</param>
		/// <returns>Authentication response object.</returns>
		private static OAuth2AuthenticateResponse interpretReply(string reply)
		{
			var response = new OAuth2AuthenticateResponse();


			if (reply.StartsWith("{"))
			{
				// JSON

				var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(reply);


				if (dict.ContainsKey("access_token")) response.AccessToken = dict["access_token"];

				if (dict.ContainsKey("refresh_token")) response.RefreshToken = dict["refresh_token"];

				if (dict.ContainsKey("state")) response.State = dict["state"];


				var seconds = 0;


				if (dict.ContainsKey("expires")) int.TryParse(dict["expires"], out seconds);

				if (dict.ContainsKey("expires_in")) int.TryParse(dict["expires_in"], out seconds);


				if (seconds > 0)

					response.Expires = DateTime.Now.AddSeconds(seconds);
			}

			else if (reply.Contains('&'))
			{
				// QueryString

				var dict = reply.Split('&');


				foreach (var entry in dict)
				{
					var index = entry.IndexOf('=');


					if (index == -1)

						continue;


					var key = entry.Substring(0, index);

					var value = entry.Substring(index + 1);


					switch (key)
					{
						case "access_token":

							response.AccessToken = value;

							break;


						case "refresh_token":

							response.RefreshToken = value;

							break;


						case "state":

							response.State = value;

							break;


						case "expires":

						case "expires_in":

							int seconds;


							if (int.TryParse(value, out seconds))

								response.Expires = DateTime.Now.AddSeconds(seconds);


							break;
					}
				}
			}


			return response;
		}


		/// <summary>
		///     Make a web-request and return response string.
		/// </summary>
		/// <param name="uri">URI to contact.</param>
		/// <param name="method">HTTP method to use.</param>
		/// <param name="payload">Payload to deliver via query-string or body.</param>
		/// <returns>Response from web-request.</returns>
		private static string request(string uri, string method = "POST", string payload = null)
		{
			if (method == "GET" &&
				!string.IsNullOrWhiteSpace(payload))

				uri += "?" + payload;


			var request = WebRequest.Create(uri) as HttpWebRequest;


			if (request == null)

				throw new WebException("Could not create WebRequest.");


			request.Method = method;

			request.ContentType = "application/x-www-form-urlencoded;";

			request.Expect = null;


			Stream stream;


			if (method == "POST" &&
				!string.IsNullOrWhiteSpace(payload))
			{
				var buffer = Encoding.UTF8.GetBytes(payload);


				request.ContentLength = buffer.Length;

				stream = request.GetRequestStream();


				stream.Write(buffer, 0, buffer.Length);

				stream.Close();
			}

			else
			{
				request.ContentLength = 0;
			}


			var response = request.GetResponse() as HttpWebResponse;


			if (response == null)

				throw new WebException("Could not get response from request.");


			stream = response.GetResponseStream();


			if (stream == null)

				throw new WebException("Could not get stream from response.");


			var reader = new StreamReader(stream);

			var body = reader.ReadToEnd();


			reader.Close();

			stream.Close();


			return body;
		}

		#endregion
	}


	/// <summary>
	///     OAuth2 provider wrapper.
	/// </summary>
	public class OAuth2Provider
	{
		public bool Offline = false;

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string AuthUri { get; set; }

		public string AccessTokenUri { get; set; }

		public string UserInfoUri { get; set; }

		public string Scope { get; set; }

		public string State { get; set; }
	}


	/// <summary>
	///     Authentication response object.
	/// </summary>
	public class OAuth2AuthenticateResponse
	{
		public string AccessToken { get; set; }

		public string RefreshToken { get; set; }

		public DateTime Expires { get; set; }

		public string State { get; set; }
	}
}