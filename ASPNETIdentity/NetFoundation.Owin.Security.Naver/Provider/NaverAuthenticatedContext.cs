using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace NetFoundation.Owin.Security.Naver
{
    public class NaverAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="NaverAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized Naver user info</param>
        /// <param name="accessToken">Naver access token</param>
        /// <param name="expires">Seconds until expiration</param>
        //

        /// <summary>
        /// Initializes a <see cref="NaverAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized Naver user info</param>
        /// <param name="accessToken">Naver access token</param>
        /// <param name="refreshToken">Naver refresh token</param>
        /// <param name="expires">Seconds until expiration</param>
        public NaverAuthenticatedContext(IOwinContext context, JObject user, string accessToken, string refreshToken, string expires) : base(context)
        {
            this.User = user;
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
                this.ExpiresIn = TimeSpan.FromSeconds(expiresValue);

            Id = TryGetValue(user, "response", "enc_id");
            InternalId = TryGetValue(user, "response", "id");
            Name = TryGetValue(user, "response", "nickname");
            UserName = TryGetValue(user, "response", "name");
            Link = TryGetValue(user, "response", "profile_image");
            Email = TryGetValue(user, "response", "email");
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the Naver user obtained from the endpoint https://api.Naver.com/v1/people/~
        /// </remarks>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the Naver access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Naver regresh token
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the Naver access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the Naver user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the Naver user internal ID
        /// </summary>
        public string InternalId { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        public string Link { get; private set; }

        /// <summary>
        /// Gets the Naver username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the Naver email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }

        private static string TryGetValue(JObject user, string propertyName, string subProperty)
        {
            JToken jToken;
            if (user.TryGetValue(propertyName, out jToken)) {
                JObject jObject = JObject.Parse(jToken.ToString());

                if (jObject != null && jObject.TryGetValue(subProperty, out jToken))
                    return jToken.ToString();
            }

            return null;
        }

        private static string TryGetFirstValue(JObject user, string propertyName, string subProperty)
        {
            JToken jToken;
            if (user.TryGetValue(propertyName, out jToken)) {
                JArray jArray = JArray.Parse(jToken.ToString());

                if (jArray != null && jArray.Count > 0) {
                    JObject jObject = JObject.Parse(jArray.First.ToString());

                    if (jObject != null && jObject.TryGetValue(subProperty, out jToken))
                        return jToken.ToString();
                }
            }

            return null;
        }
    }
}
