using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiImagesProcessor.Abstractions.Model;

namespace WikiImagesProcessor.Abstractions.Services
{
    public interface IWorkflowProcessor
    {
        Task<List<Tuple<ImageInfo, ImageInfo>>> Process(ProcessOptions processOptions);
    }
}
