using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using WikiImagesProcessor.Abstractions.Model;
using WikiImagesProcessor.Abstractions.Services;
using WikiImagesProcessor.Services;

namespace WikiImagesProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserResult<CommandLineOptions> parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
            if (parseResult.Tag != ParserResultType.Parsed)
                return;
            var options = ((Parsed<CommandLineOptions>)parseResult).Value;

            IServiceProvider serviceProvider = InitServiceProvider(options);
            
            var logService = serviceProvider.GetService<ILogService>();
            var workfoProcessor = serviceProvider.GetService<IWorkflowProcessor>();

            LogOptions(options, logService);

            try
            {
                var result = workfoProcessor.Process(options.ToProcessOptions()).Result;
                PrintResult(result);

            }
            catch (AggregateException ex)
            {
                foreach (var exc in ex.Flatten().InnerExceptions)
                {
                    RegisterException(exc, logService);
                }
            }
            catch (Exception ex)
            {
                RegisterException(ex, logService);
            }
        }

        private static void RegisterException(Exception exc, ILogService logService)
        {
            Console.WriteLine($"{exc.Message}\nSee log for details.");
            logService.Exception(exc);
        }

        private static IServiceProvider InitServiceProvider(CommandLineOptions options)
        {
            return new ServiceCollection()
                            .AddSingleton<ILogService, LogService>(provider => new LogService(options.LogConfigPath))
                            .AddSingleton<IJsonHttpService, JsonHttpService>()
                            .AddSingleton<ICoordinateService, CoordinateService>()
                            .AddSingleton<IWikiService, WikiService>()
                            .AddSingleton<IDistanceService, LevenshteinDistanceService>()
                            .AddSingleton<IWorkflowProcessor, WorkflowProcessor>()
                            .BuildServiceProvider();
        }

        private static void PrintResult(List<Tuple<ImageInfo, ImageInfo>> result)
        {
            if ((result?.Count ?? 0) != 0)
            {
                var grouping = result.GroupBy(r => r.Item1.ToString());

                foreach (var group in grouping)
                {
                    Console.WriteLine(@group.Key);
                    foreach (var tuple in @group)
                    {
                        Console.WriteLine($"   {tuple.Item2}");
                    }
                }
            }
            else
                Console.WriteLine("No similar images found.");
        }

        private static void LogOptions(CommandLineOptions options, ILogService logService)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Input parameters for current run: ");

            foreach (PropertyInfo propertyInfo in typeof(CommandLineOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(m => m.Name))
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(options, new object[0]);
                builder.AppendLine($"   {name} = '{value}'");
            }
            logService.Info(builder.ToString());
        }

    }
}
