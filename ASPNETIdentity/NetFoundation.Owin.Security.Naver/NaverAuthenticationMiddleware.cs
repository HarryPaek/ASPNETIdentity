using System;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace NetFoundation.Owin.Security.Naver
{
    /// <summary>
    /// OWIN middleware for authenticating users using Naver OAuth 2.0
    /// </summary>
    public class NaverAuthenticationMiddleware : AuthenticationMiddleware<NaverAuthenticationOptions>
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a <see cref="NaverAuthenticationMiddleware" />
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="app">The OWIN application</param>
        /// <param name="options">Configuration options for the middleware</param>
        public NaverAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, NaverAuthenticationOptions options) : base(next, options)
        {
            if (String.IsNullOrWhiteSpace(Options.ClientId))
                throw new ArgumentException(String.Format("The '{0}' option must be provided.", "ClientId"));
            if (String.IsNullOrWhiteSpace(Options.ClientSecret))
                throw new ArgumentException(String.Format("An ICertificateValidator cannot be specified at the same time as an HttpMessageHandler unless it is a WebRequestHandler.", "ClientSecret"));

            logger = app.CreateLogger<NaverAuthenticationMiddleware>();

            if (Options.Provider == null)
                Options.Provider = new NaverAuthenticationProvider();

            if (Options.StateDataFormat == null)
            {
                IDataProtector dataProtector = app.CreateDataProtector(typeof(NaverAuthenticationMiddleware).FullName, Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            if (String.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();

            httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024 * 1024 * 10
            };
        }

        /// <summary>
        /// Provides the <see cref="Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> configured with the <see cref="NaverAuthenticationOptions" /> supplied to the constructor.</returns>
        protected override AuthenticationHandler<NaverAuthenticationOptions> CreateHandler()
        {
            return new NaverAuthenticationHandler(httpClient, logger);
        }

        private HttpMessageHandler ResolveHttpMessageHandler(NaverAuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator != null)
            {
                // Set the cert validate callback
                var webRequestHandler = handler as WebRequestHandler;
                if (webRequestHandler == null)
                    throw new InvalidOperationException("An ICertificateValidator cannot be specified at the same time as an HttpMessageHandler unless it is a WebRequestHandler.");

                webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            }

            return handler;
        }
    }
}
