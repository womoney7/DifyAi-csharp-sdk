using DifyAi.Dto.ParamDto;

using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcedMango.DifyAi.Services
{
    public interface IDifyAiWorkflowServices
    {
        public Task<DifyApiResult<Dify_WorkflowCompletionResDto>> CreateWorkflowBlockModeAsync(
        Dify_WorkflowParamDto paramDto,
        string overrideApiKey = "", CancellationToken cancellationToken = default);

        public AsyncCollectionResult<Dify_WorkflowChunkCompletionResDto> CreateWorkflowStreamModeAsync(
        Dify_WorkflowParamDto paramDto,
        string overrideApiKey = "", CancellationToken cancellationToken = default);

       public CollectionResult<Dify_WorkflowChunkCompletionResDto> CreateWorkflowStreamMode(
       Dify_WorkflowParamDto paramDto,
       string overrideApiKey = "", CancellationToken cancellationToken = default);
    }
}
