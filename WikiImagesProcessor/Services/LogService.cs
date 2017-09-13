using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{
    class LogService : ILogService
    {
        private readonly ILog _log;

        public LogService(string logConfigPath)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(logConfigPath));

            _log = LogManager.GetLogger(typeof(Program));
        }

        public void Trace(string message)
        {
            _log.Debug(message);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warning(string message)
        {
            _log.Warn(message);
        }

        public void Exception(Exception ex)
        {
            _log.Error("Error occured", ex);
        }
    }
}
