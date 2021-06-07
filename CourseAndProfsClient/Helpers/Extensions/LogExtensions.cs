using System;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;

namespace CourseAndProfsClient.Helpers.Extensions
{
	public static class LogExtensions
	{
		public const string logSC = "SourceContext = 'Microsoft.Hosting.Lifetime' or SourceContext = 'Microsoft.EntityFrameworkCore.Database.Command' or SourceContext = 'Serilog.AspNetCore.RequestLoggingMiddleware' or SourceContext = 'IdentityServer4.Hosting.IdentityServerMiddleware'";
		public static LoggerConfiguration CreateStartupLogger(this LoggerConfiguration logConfiguration)
			=> logConfiguration
				   .MinimumLevel.Verbose()
				   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				   .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
				   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				   .MinimumLevel.Override("System", LogEventLevel.Error)
				   .Enrich.WithMachineName()
				   .Enrich.WithEnvironmentUserName()
				   .Enrich.WithProcessId()
				   .Enrich.WithProcessName()
				   .Enrich.WithThreadId()
				   .Enrich.WithThreadName()
				   .Enrich.FromLogContext()
				   .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
					   .WithDefaultDestructurers()
					   .WithDestructurers(new[] {new DbUpdateExceptionDestructurer()})
					   .WithRootName("Exception"))
				   .WriteTo.Console(theme: AnsiConsoleTheme.Code, standardErrorFromLevel: LogEventLevel.Verbose)
				   .WriteTo.Debug()
			   ?? throw new ArgumentNullException(nameof(logConfiguration));

		internal static LoggerConfiguration ConfigureLogger(
			this LoggerConfiguration logConfiguration,
			IConfiguration configuration,
			IWebHostEnvironment environment)
			=> logConfiguration
				.CreateStartupLogger()
				.Enrich.WithProperty("Application", environment.ApplicationName)
				.Enrich.WithProperty("Environment", environment.EnvironmentName)
				.WriteTo.Logger(log => log
					.MinimumLevel.ControlledBy(Program.LevelSwitch)
					.Filter.ByExcluding(logSC)
					.WriteTo.File(
						configuration.GetValue("AzureDeployment", false)
							? $@"D:\home\LogFiles\Application\{environment.ApplicationName}.txt"
							: Path.Combine(Directory.GetCurrentDirectory(), "Logs",
								$"{environment.ApplicationName}-.log"),
						fileSizeLimitBytes: 31_457_280,
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 10,
						shared: true,
						flushToDiskInterval: TimeSpan.FromSeconds(1))
					.WriteTo.Seq(
						configuration["Seq:Uri"],
						apiKey: configuration["Seq:ApiKey"],
						controlLevelSwitch: Program.LevelSwitch));
	}
}
