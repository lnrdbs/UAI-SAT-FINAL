using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealtimeChatSample.Api.hub;
using System;
using System.Text;
using AuthSample;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RealtimeChatSample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
                options.EnableJSONP = true;

            });

            var key = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("my super secret key goes here")));


            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {

                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidAudience = AuthSample.Security.TokenHandler.JWT_TOKEN_AUDIENCE,
                ValidIssuer = AuthSample.Security.TokenHandler.JWT_TOKEN_ISSUER,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(0) //TimeSpan.Zero
            };

            services.AddCors();
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });
            services.AddAuthorization(options =>
            {

                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).
                  RequireAuthenticatedUser().Build();
            });

            services.AddAuthentication().
                AddJwtBearer(options =>
                options.TokenValidationParameters = tokenValidationParameters);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Habilito  para poder servir archivos estáticos
            app.UseStaticFiles();

            app.Map("/signalr", map =>
            {
                map.UseCors(opt =>
                      opt.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                map.RunSignalR();
            });

            app.UseMvc();

           

        }
    }
}
