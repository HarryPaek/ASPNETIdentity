using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFoundation.Owin.Security.Naver
{
    /// <summary>
    /// Specifies callback methods which the <see cref="NaverAuthenticationMiddleware"></see> invokes to enable developer control over the authentication process. />
    /// </summary>
    public interface INaverAuthenticationProvider
    {
        /// <summary>
        /// Invoked whenever Naver succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task Authenticated(NaverAuthenticatedContext context);

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity" />.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task ReturnEndpoint(NaverReturnEndpointContext context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the Google OpenID middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="Microsoft.Owin.Security.AuthenticationProperties" /> of the challenge </param>
        void ApplyRedirect(NaverApplyRedirectContext context);
    }
}
