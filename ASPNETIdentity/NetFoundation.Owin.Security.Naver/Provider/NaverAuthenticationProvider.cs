using System;
using System.Threading.Tasks;

namespace NetFoundation.Owin.Security.Naver
{
    /// <summary>
    /// Default <see cref="INaverAuthenticationProvider" /> implementation.
    /// </summary>
    public class NaverAuthenticationProvider : INaverAuthenticationProvider
    {
        /// <summary>
        /// Initializes a <see cref="NaverAuthenticationProvider"/>
        /// </summary>
        public NaverAuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
            OnApplyRedirect = context => context.Response.Redirect(context.RedirectUri);
        }

        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<NaverAuthenticatedContext, Task> OnAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the function that is invoked when the ReturnEndpoint method is invoked.
        /// </summary>
        public Func<NaverReturnEndpointContext, Task> OnReturnEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is invoked when the ApplyRedirect method is invoked.
        /// </summary>
        public Action<NaverApplyRedirectContext> OnApplyRedirect { get; set; }

        /// <summary>
        /// Invoked whenever Naver succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task Authenticated(NaverAuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task ReturnEndpoint(NaverReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the Naver OAuth 2.0 middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="Microsoft.Owin.Security.AuthenticationProperties" /> of the challenge </param>
        public virtual void ApplyRedirect(NaverApplyRedirectContext context)
        {
            this.OnApplyRedirect(context);
        }
    }
}
