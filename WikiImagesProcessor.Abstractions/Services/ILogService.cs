using System;

namespace WikiImagesProcessor.Abstractions.Services
{
    public interface ILogService
    {
        void Trace(string message);
        void Info(string message);
        void Warning(string message);
        void Exception(Exception ex);
    }
}