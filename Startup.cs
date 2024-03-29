﻿using System;
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
            // services.AddCors(options =>
            // {
            //     options.AddPolicy(mySpecificOrigin, builder => {
            //         builder.WithOrigins("https://votingblock.herokuapp.com", "http://localhost:5000", "http://localhost:5001")
            //         .AllowAnyHeader()
            //         .AllowAnyMethod();
            //     });
            // });
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAutoMapper();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDbContext<VotingDBContext>(options =>{
                
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connStr;
                if (env == "Development") {
                    connStr = Configuration.GetConnectionString("DefaultConnection");
                } else {
                    // Heroku provides PostgreSQL connection URL via env variable
                    // var connUrl = Environment.GetEnvironmentVariable("HEROKU_POSTGRESQL_SILVER_URL");
                    var connUrl = "postgres://ptgircjbbmersp:a828509805c6dd3a78072d43290f4fb788d79763ec576f6c81e5113d047f996e@ec2-54-228-243-238.eu-west-1.compute.amazonaws.com:5432/d8s09424j0fhla";
                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);

                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];

                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};sslmode=Prefer;Trust Server Certificate=true";
                    Console.WriteLine("ConnectionString: " + connStr);
                }
            options.UseNpgsql(connStr);
            });
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
            // app.UseCors(mySpecificOrigin);
             app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
        }
    }
}
