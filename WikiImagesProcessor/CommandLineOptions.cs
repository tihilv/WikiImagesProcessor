using CommandLine;
using WikiImagesProcessor.Abstractions.Model;

namespace WikiImagesProcessor
{
    class CommandLineOptions
    {
        [Option('l', "location", Required = true, HelpText = "Location name to be processed.")]
        public string LocationName { get; set; }

        [Option("log", HelpText = "Path to log configuration file.", Default = @"Resources\log4net.config")]
        public string LogConfigPath { get; set; }

        [Option('a', "articles", HelpText = "Wiki articles limit to search.", Default = 50)]
        public int ArticlesLimit { get; set; }

        [Option('d', "domain", HelpText = "Wiki subdomain to search (en, de, ru...).", Default = "en")]
        public string Subdomain { get; set; }

        [Option('i', "ignore-equals", HelpText = "Ignore equals image titles.", Default = false)]
        public bool IgnoreEqualTitles { get; set; }

        public ProcessOptions ToProcessOptions()
        {
            return new ProcessOptions(LocationName, ArticlesLimit, Subdomain, IgnoreEqualTitles);
        }
    }
}
