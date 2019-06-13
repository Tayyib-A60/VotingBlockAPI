using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Core;
using API.Extensions;
using API.Persistence;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string mySpecificOrigin = "VotingBlockOrigin";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy(mySpecificOrigin, builder => {
                    builder.WithOrigins("https://votingblock.herokuapp.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAutoMapper();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDbContext<VotingDBContext>(options =>{
                var pgUserId = Environment.GetEnvironmentVariable("User");
                var pgPassword = Environment.GetEnvironmentVariable("Password");
                var pgHost = Environment.GetEnvironmentVariable("Host");
                var pgPort = Environment.GetEnvironmentVariable("Port");
                var pgDatabase = Environment.GetEnvironmentVariable("Database");

                var connStr = $"Host={pgHost};Port={pgPort};Username={pgUserId};Password={pgPassword};Database={pgDatabase},sslmode='Prefer';Trust Server Certificate='true'";

                options.UseNpgsql(connStr);
            });
            // options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(auth => {
               auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors(mySpecificOrigin);
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
        }
    }
}
