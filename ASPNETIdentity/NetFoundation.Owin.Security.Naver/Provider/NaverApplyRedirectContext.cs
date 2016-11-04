using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace NetFoundation.Owin.Security.Naver
{
    public class NaverApplyRedirectContext : BaseContext<NaverAuthenticationOptions>
    {
        ///<summary>
        ///Gets the URI used for the redirect operation.
        ///</summary>
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authenticaiton properties of the challenge
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }

        /// <summary>
        /// Creates a new context object.
        /// </summary>
        /// <param name="context">The OWIN request context</param>
        /// <param name="options">The Naver OAuth 2.0 middleware options</param>
        /// <param name="properties">The authenticaiton properties of the challenge</param>
        /// <param name="redirectUri">The initial redirect URI</param>
        public NaverApplyRedirectContext(IOwinContext context, NaverAuthenticationOptions options, AuthenticationProperties properties, string redirectUri) : base(context, options)
        {
            this.RedirectUri = redirectUri;
            this.Properties = properties;
        }
    }
}
