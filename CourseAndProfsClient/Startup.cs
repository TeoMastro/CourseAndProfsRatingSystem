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
using CourseAndProfsClient.Helpers;
using Serilog;

namespace CourseAndProfsClient
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      Configuration = configuration;
      Environment = environment;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IPureMapper>(sp => new PureMapper(MappingConfiguration.Mapping));
      services.AddControllersWithViews();
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      services.AddHttpContextAccessor();
      //services.AddSingleton<TimestampSaveChangesInterceptor>();
      //services.AddSingleton<AuditSaveChangesInterceptor<Guid>>();


      services.AddDbContextPool<CaPDbContext>((serviceProvider, options) =>
      {
        options.UseNpgsql(
            Configuration.GetConnectionString("CaP"))
          .EnableCommonOptions(Environment);
      });

      services.AddDatabaseDeveloperPageExceptionFilter();
      services.AddHostedService<MigrationService<CaPDbContext>>();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "CourseAndProfs.Web.Api", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description =
            "Enter 'Bearer' [space] and then your valid Token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer", },
            },
            Array.Empty<string>()
          },
        });
        c.DescribeAllParametersInCamelCase();
        c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
      });


      services.AddIdentity<CaPUser, CaPRoles>(c =>
      {
        var isDevelopment = Environment.IsDevelopment();
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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseSwagger();
      //if (env.IsDevelopment())
      //{
      app.UseDeveloperExceptionPage();

      app.UseMigrationsEndPoint();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourseAndProfs.Web.Api v1");
        c.DocumentTitle = "CourseAndProfs API";
        c.DocExpansion(DocExpansion.None);
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableValidator();
        c.DisplayOperationId();
        c.DisplayRequestDuration();
      });
      //}
      //else
      //{
      //  app.UseExceptionHandler("/Error");
      //  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      //  app.UseHsts();
      //}

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseReDoc(c =>
      {
        c.RoutePrefix = "docs";
        c.DocumentTitle = "Course And Professors Rating System v1";
        c.SpecUrl("/swagger/v1/swagger.json");
        c.ExpandResponses("none");
        c.RequiredPropsFirst();
        c.SortPropsAlphabetically();
        c.HideDownloadButton();
        c.HideHostname();
      });

      app.UseSerilogIngestion(x =>
      {
        x.ClientLevelSwitch = Program.LevelSwitch;
      });

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
