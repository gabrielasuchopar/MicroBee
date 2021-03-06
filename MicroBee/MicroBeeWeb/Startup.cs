﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MicroBee.Web.DAL.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;
using MicroBee.Web.DAL.Entities;
using MicroBee.Web.DAL.Repositories;
using MicroBee.Web.Services;

using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Http;

namespace MicroBee.Web
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
			// Identity
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<MicroBeeDbContext>()
				.AddDefaultTokenProviders();

			// Db context
			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
			{
				services.AddDbContext<MicroBeeDbContext>(options =>
					options.UseSqlServer(Configuration.GetConnectionString("AzDbConnection")));
			}
			else
			{
				string connection = Configuration.GetConnectionString("MicroBeeDatabase");
				services.AddDbContext<MicroBeeDbContext>(options => options.UseSqlServer(connection));
			}
			services.BuildServiceProvider().GetService<MicroBeeDbContext>().Database.Migrate();

			// Repositories
			services.AddTransient<IMicroItemRepository, MicroItemRepository>();
			services.AddTransient<IMicroImageRepository, MicroImageRepository>();
			services.AddTransient<ICategoryRepository, CategoryRepository>();

			// Services
			services.AddTransient<IMicroItemService, MicroItemService>();
			services.AddTransient<IMicroImageService, MicroImageService>();
			services.AddTransient<ICategoryService, CategoryService>();

			// Jwt bearer token authentication setup
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); //clears default MS claim names
			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = true,
						ValidateIssuerSigningKey = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						RequireExpirationTime = true,

						ValidIssuer = Configuration["JwtIssuer"],
						ValidAudience = Configuration["JwtAudience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"]))
					};
				});

			//TBD
			//services.AddHsts(options =>
			//{
			//	options.Preload = true;
			//	options.IncludeSubDomains = true;
			//});

			//services.AddHttpsRedirection(options =>
			//{
			//	options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
			//});

			services.AddCors();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, MicroBeeDbContext context)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/error");

				//TBD
				//app.UseHsts();
			}

			app.UseAuthentication();

			app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

			//TBD
			//app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}

