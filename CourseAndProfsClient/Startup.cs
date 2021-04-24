using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CourseAndProfsClient;
//using CourseAndProfsClient.Identity;
//using CourseAndProfsClient.Helpers;
//using CourseAndProfsClient.Helpers.Extensions;
//using CourseAndProfsClient.Options;

using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Configuration.Persistence.Interceptors;
using Kritikos.Configuration.Persistence.Services;
using Kritikos.PureMap;
using Kritikos.PureMap.Contracts;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using CourseAndProfsPersistence.Identity;
using CourseAndProfsPersistence;
using System;

namespace CourseAndProfsClient
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
    {
      services.AddControllersWithViews();
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      //services.AddSingleton<IPureMapper>(sp => new PureMapper(MappingConfiguration.Mapping));

      services.AddHttpContextAccessor();
      services.AddSingleton<TimestampSaveChangesInterceptor>();
      services.AddSingleton<AuditSaveChangesInterceptor<Guid>>();


      services.AddDbContextPool<CaPDbContext>((serviceProvider, options) =>
      {
        options.UseNpgsql(
            Configuration.GetConnectionString("CaP"))
          .AddInterceptors(
            serviceProvider.GetRequiredService<TimestampSaveChangesInterceptor>(),
            serviceProvider.GetRequiredService<AuditSaveChangesInterceptor<Guid>>())
          .EnableCommonOptions(env);
      });

      services.AddDatabaseDeveloperPageExceptionFilter();
      services.AddHostedService<MigrationService<CaPDbContext>>();





      services.AddIdentity<CaPUser, CaPRoles>(c =>
      {
        var isDevelopment = env.IsDevelopment();
        c.User.RequireUniqueEmail = !isDevelopment;
        c.Password = new PasswordOptions
        {
          RequireDigit = !isDevelopment,
          RequireLowercase = !isDevelopment,
          RequireUppercase = !isDevelopment,
          RequireNonAlphanumeric = !isDevelopment,
          RequiredLength = isDevelopment
            ? 4
            : 6,
        };
      })
        .AddEntityFrameworkStores<CaPDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();

      services.AddIdentityServer()
        .AddApiAuthorization<CaPUser, CaPDbContext>();

      

      //services.AddCorrelation();
      services.AddControllersWithViews();
      services.AddRazorPages();

      services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

      services.AddCors(options =>
      {
        options.AddPolicy("AllowAll",
          builder =>
          {
            builder.AllowAnyOrigin();
          });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }
  }
}
