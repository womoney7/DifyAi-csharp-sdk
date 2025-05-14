using System;
using System.ClientModel.Primitives;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace IcedMango.DifyAi.Request
{
    public static class ClientPipelineExtensions
    {

        public static async ValueTask<PipelineResponse> ProcessMessageAsync(
            this ClientPipeline pipeline,
            PipelineMessage message,
            RequestOptions options)
        {
            await pipeline.SendAsync(message).ConfigureAwait(false);
            if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
            {
                throw await TryBufferResponseAndCreateErrorAsync(message).ConfigureAwait(false) switch
                {
                    string errorMessage when !string.IsNullOrEmpty(errorMessage)
                        => new ClientResultException(errorMessage, message.Response),
                    _ => new ClientResultException(message.Response),
                };
            }

            return message.BufferResponse ?
                message.Response :
                message.ExtractResponse();
        }

        public static PipelineResponse ProcessMessage(
            this ClientPipeline pipeline,
            PipelineMessage message,
            RequestOptions options)
        {
            pipeline.Send(message);

            if (message.Response.IsError && (options?.ErrorOptions & ClientErrorBehaviors.NoThrow) != ClientErrorBehaviors.NoThrow)
            {
                throw TryBufferResponseAndCreateError(message) switch
                {
                    string errorMessage when !string.IsNullOrEmpty(errorMessage)
                        => new ClientResultException(errorMessage, message.Response),
                    _ => new ClientResultException(message.Response),
                };
            }

            return message.BufferResponse ?
                message.Response :
                message.ExtractResponse();
        }

        private static string TryBufferResponseAndCreateError(PipelineMessage message)
        {
            message.Response.BufferContent();
            return TryCreateErrorMessageFromResponse(message.Response);
        }

        private static async Task<string> TryBufferResponseAndCreateErrorAsync(PipelineMessage message)
        {
            await message.Response.BufferContentAsync().ConfigureAwait(false);
            return TryCreateErrorMessageFromResponse(message.Response);
        }

        private static string TryCreateErrorMessageFromResponse(PipelineResponse response)
        {
            try
            {
                using JsonDocument errorDocument = JsonDocument.Parse(response.Content);
                errorDocument.RootElement.TryGetProperty("message", out JsonElement messageElement);
                return messageElement.GetString();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
