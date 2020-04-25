using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                // we check the cookie to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";
                // When we sign-in  we will deal out a cookie
                config.DefaultSignInScheme = "ClientCookie";
                // use this to check if we are allowed to do something
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44388/oauth/authorize";
                    config.TokenEndpoint = "https://localhost:44388/oauth/token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {
                            var access_token = context.AccessToken;
                            var base64payload = access_token.Split('.')[1];
                            var bytes = Convert.FromBase64String(base64payload);
                            var JsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonPayload);

                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
