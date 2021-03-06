using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityAccess.Api.Models;
using IdentityAccess.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdentityAccess.Api
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
			//configure EntityFramework for SQL Server

			services.AddDbContext<UserDbContext>(options =>
			{
				options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=UserDB;Trusted_Connection=True;");

				//options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); if the connection string declared in the appsettings
			});

			//Create Identity Requirements
			services.AddIdentity<IdentityUser, IdentityRole>(options =>
			 {
				 options.Password.RequireDigit = true;
				 options.Password.RequireLowercase = true;
				 options.Password.RequiredLength = 5;
			 }).AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders();

			 //Adding Authentication properties
			services.AddAuthentication(auth =>
			{
				auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => {
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = Configuration["AuthSettings:Audience"],
					ValidIssuer = Configuration["AuthSettings:Issuer"],
					RequireExpirationTime = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
					ValidateIssuerSigningKey = true
				};
			});

			services.AddScoped<IUserService,UserService>();
			services.AddTransient<IMailService, SendGridMailSerice>();
			services.AddControllers();
			services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
			});
		}
	}
}
