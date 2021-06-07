using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using CourseAndProfsClient.Helpers;
using CourseAndProfsClient.Helpers.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Extensions.Logging;

namespace CourseAndProfsClient
{
	public static class Program
	{
		internal static readonly LoggingLevelSwitch LevelSwitch = new();
		private static Microsoft.Extensions.Logging.ILogger logger = new NullLogger<Startup>();

		public static async Task Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.CreateStartupLogger()
				.CreateBootstrapLogger();

			using var loggerProvider = new SerilogLoggerProvider();
			logger = loggerProvider.CreateLogger(nameof(Startup));

			try
			{
				var host = CreateHostBuilder(args).Build();
				logger = host.Services.GetRequiredService<ILogger<Startup>>();

				await host.RunAsync();
			}
			catch (Exception e)
			{
				logger.LogCritical(e, LogTemplates.BootstrappingError, e.Message);
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog((_, services, configuration) => configuration
					.ConfigureLogger(
						services.GetRequiredService<IConfiguration>(),
						services.GetRequiredService<IWebHostEnvironment>()))
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
