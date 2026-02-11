using DifyAi.Dto.ParamDto;

using IcedMango.DifyAi.Request;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IcedMango.DifyAi.Services
{
    public class DifyAiWorkflowServices : IDifyAiWorkflowServices
    {

        private readonly IRequestExtension _requestExtension;
        private readonly ClientPipeline _pipeline;
        private readonly IConfiguration _config;

        public DifyAiWorkflowServices(IRequestExtension requestExtension, ClientPipeline clientPipeline, IConfiguration configuration)
        {
            _requestExtension = requestExtension;
            _pipeline = clientPipeline;
            _config = configuration;
        }
        public async Task<DifyApiResult<Dify_WorkflowCompletionResDto>> CreateWorkflowBlockModeAsync(Dify_WorkflowParamDto paramDto, string overrideApiKey, CancellationToken cancellationToken)
        {
            paramDto.ResponseMode = "blocking";

            var res = await _requestExtension.HttpPost<Dify_WorkflowCompletionResDto>(
                "workflows/run",
                paramDto,
                overrideApiKey,
                cancellationToken);

            return res;
        }


        public AsyncCollectionResult<Dify_WorkflowChunkCompletionResDto> CreateWorkflowStreamModeAsync(Dify_WorkflowParamDto paramDto, string overrideApiKey = "", CancellationToken cancellationToken = default)
        {
            paramDto.ResponseMode = "streaming";
            var content = BinaryContent.Create(paramDto);

            async Task<ClientResult> sendRequestAsync() => await CompleteWorkflowAsync(content, new RequestOptions());
            return new AsyncStreamingWorkflowChunkCompletionUpdateCollection(sendRequestAsync, cancellationToken);
        }

        public virtual async Task<ClientResult> CompleteWorkflowAsync(BinaryContent content, RequestOptions options = null)
        {
            using PipelineMessage message = CreateCreateChatCompletionRequest(content, options);
            return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }

        PipelineMessage CreateCreateChatCompletionRequest(BinaryContent content, RequestOptions options)
        {
            var message = _pipeline.CreateMessage();
            message.ResponseClassifier = PipelineMessageClassifier.Create(new ushort[] { 200 });
            var request = message.Request;
            request.Method = "POST";

            // 确保BaseUrl末尾有正确的"/"
            string baseUrl = _config.GetValue<string>("DifyAi:BaseUrl") ?? string.Empty;
            if (!string.IsNullOrEmpty(baseUrl) && !baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            string url = $"{baseUrl}workflows/run";

            request.Uri = new Uri(url);
            //request.Headers.Set("Accept", "application/json");
            request.Headers.Set("Content-Type", "application/json");
            request.Content = content;
            message.Apply(options);
            return message;
        }

        public CollectionResult<Dify_WorkflowChunkCompletionResDto> CreateWorkflowStreamMode(Dify_WorkflowParamDto paramDto, string overrideApiKey = "", CancellationToken cancellationToken = default)
        {
            paramDto.ResponseMode = "streaming";
            var content = BinaryContent.Create(paramDto);

            ClientResult sendRequest() => CompleteWorkflow(content, new RequestOptions());
            return new InternalStreamingWorkflowChunkCompletionUpdateCollection(sendRequest, cancellationToken);
        }

        public virtual ClientResult CompleteWorkflow(BinaryContent content, RequestOptions options = null)
        {
            using PipelineMessage message = CreateCreateChatCompletionRequest(content, options);
            return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
        }
    }


    internal class AsyncStreamingWorkflowChunkCompletionUpdateCollection : AsyncCollectionResult<Dify_WorkflowChunkCompletionResDto>
    {
        private readonly Func<Task<ClientResult>> _sendRequestAsync;
        private readonly CancellationToken _cancellationToken;

        public AsyncStreamingWorkflowChunkCompletionUpdateCollection(
            Func<Task<ClientResult>> sendRequestAsync,
            CancellationToken cancellationToken)
        {
            //Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));

            _sendRequestAsync = sendRequestAsync;
            _cancellationToken = cancellationToken;
        }

        public override ContinuationToken? GetContinuationToken(ClientResult page)

            // Continuation is not supported for SSE streams.
            => null;

        public async override IAsyncEnumerable<ClientResult> GetRawPagesAsync()
        {
            // We don't currently support resuming a dropped connection from the
            // last received event, so the response collection has a single element.
            yield return await _sendRequestAsync();
        }

        protected async override IAsyncEnumerable<Dify_WorkflowChunkCompletionResDto> GetValuesFromPageAsync(ClientResult page)
        {
            await using IAsyncEnumerator<Dify_WorkflowChunkCompletionResDto> enumerator = new AsyncStreamingWorkflowChunkCompletionEnumerator(page, _cancellationToken);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return enumerator.Current;
            }
        }

        private sealed class AsyncStreamingWorkflowChunkCompletionEnumerator : IAsyncEnumerator<Dify_WorkflowChunkCompletionResDto>
        {
            private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

            private readonly CancellationToken _cancellationToken;
            private readonly PipelineResponse _response;

            // These enumerators represent what is effectively a doubly-nested
            // loop over the outer event collection and the inner update collection,
            // i.e.:
            //   foreach (var sse in _events) {
            //       // get _updates from sse event
            //       foreach (var update in _updates) { ... }
            //   }
            private IAsyncEnumerator<System.Net.ServerSentEvents.SseItem<byte[]>>? _events;
            private IEnumerator<Dify_WorkflowChunkCompletionResDto>? _updates;

            private Dify_WorkflowChunkCompletionResDto? _current;
            private bool _started;

            public AsyncStreamingWorkflowChunkCompletionEnumerator(ClientResult page, CancellationToken cancellationToken)
            {
                _response = page.GetRawResponse();
                _cancellationToken = cancellationToken;
            }

            Dify_WorkflowChunkCompletionResDto IAsyncEnumerator<Dify_WorkflowChunkCompletionResDto>.Current
                => _current!;

            async ValueTask<bool> IAsyncEnumerator<Dify_WorkflowChunkCompletionResDto>.MoveNextAsync()
            {
                if (_events is null && _started)
                {
                    throw new ObjectDisposedException(nameof(AsyncStreamingWorkflowChunkCompletionEnumerator));
                }

                _cancellationToken.ThrowIfCancellationRequested();
                _events ??= CreateEventEnumeratorAsync();
                _started = true;

                if (_updates is not null && _updates.MoveNext())
                {
                    _current = _updates.Current;
                    return true;
                }

                if (await _events.MoveNextAsync().ConfigureAwait(false))
                {
                    if (_events.Current.Data.AsSpan().SequenceEqual(TerminalData))
                    {
                        _current = default;
                        return false;
                    }

                    using JsonDocument doc = JsonDocument.Parse(_events.Current.Data);
                    List<Dify_WorkflowChunkCompletionResDto> updates = [Dify_WorkflowChunkCompletionResDto.DeserializeWorkflowChunkCompletionResDto(doc.RootElement)];
                    _updates = updates.GetEnumerator();

                    if (_updates.MoveNext())
                    {
                        _current = _updates.Current;
                        return true;
                    }
                }

                _current = default;
                return false;
            }

            private IAsyncEnumerator<SseItem<byte[]>> CreateEventEnumeratorAsync()
            {
                if (_response.ContentStream is null)
                {
                    throw new InvalidOperationException("Unable to create result from response with null ContentStream");
                }

                IAsyncEnumerable<SseItem<byte[]>> enumerable = SseParser.Create(_response.ContentStream, (_, bytes) => bytes.ToArray()).EnumerateAsync();
                return enumerable.GetAsyncEnumerator(_cancellationToken);
            }

            public async ValueTask DisposeAsync()
            {
                await DisposeAsyncCore().ConfigureAwait(false);

                GC.SuppressFinalize(this);
            }

            private async ValueTask DisposeAsyncCore()
            {
                if (_events is not null)
                {
                    await _events.DisposeAsync().ConfigureAwait(false);
                    _events = null;

                    // Dispose the response so we don't leave the network connection open.
                    _response?.Dispose();
                }
            }
        }
    }

    internal class InternalStreamingWorkflowChunkCompletionUpdateCollection : CollectionResult<Dify_WorkflowChunkCompletionResDto>
    {
        private readonly Func<ClientResult> _sendRequest;
        private readonly CancellationToken _cancellationToken;

        public InternalStreamingWorkflowChunkCompletionUpdateCollection(
            Func<ClientResult> sendRequest,
            CancellationToken cancellationToken)
        {
            _sendRequest = sendRequest;
            _cancellationToken = cancellationToken;
        }

        public override ContinuationToken? GetContinuationToken(ClientResult page)
            // Continuation is not supported for SSE streams.
            => null;

        public override IEnumerable<ClientResult> GetRawPages()
        {
            // We don't currently support resuming a dropped connection from the
            // last received event, so the response collection has a single element.
            yield return _sendRequest();
        }

        protected override IEnumerable<Dify_WorkflowChunkCompletionResDto> GetValuesFromPage(ClientResult page)
        {
            using IEnumerator<Dify_WorkflowChunkCompletionResDto> enumerator = new StreamingChatUpdateEnumerator(page, _cancellationToken);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        private sealed class StreamingChatUpdateEnumerator : IEnumerator<Dify_WorkflowChunkCompletionResDto>
        {
            private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

            private readonly CancellationToken _cancellationToken;
            private readonly PipelineResponse _response;

            // These enumerators represent what is effectively a doubly-nested
            // loop over the outer event collection and the inner update collection,
            // i.e.:
            //   foreach (var sse in _events) {
            //       // get _updates from sse event
            //       foreach (var update in _updates) { ... }
            //   }
            private IEnumerator<SseItem<byte[]>>? _events;
            private IEnumerator<Dify_WorkflowChunkCompletionResDto>? _updates;

            private Dify_WorkflowChunkCompletionResDto? _current;
            private bool _started;

            public StreamingChatUpdateEnumerator(ClientResult page, CancellationToken cancellationToken)
            {

                _response = page.GetRawResponse();
                _cancellationToken = cancellationToken;
            }

            Dify_WorkflowChunkCompletionResDto IEnumerator<Dify_WorkflowChunkCompletionResDto>.Current
                => _current!;

            object IEnumerator.Current => _current!;

            public bool MoveNext()
            {
                if (_events is null && _started)
                {
                    throw new ObjectDisposedException(nameof(StreamingChatUpdateEnumerator));
                }

                _cancellationToken.ThrowIfCancellationRequested();
                _events ??= CreateEventEnumerator();
                _started = true;

                if (_updates is not null && _updates.MoveNext())
                {
                    _current = _updates.Current;
                    return true;
                }

                if (_events.MoveNext())
                {
                    if (_events.Current.Data.AsSpan().SequenceEqual(TerminalData))
                    {
                        _current = default;
                        return false;
                    }

                    using JsonDocument doc = JsonDocument.Parse(_events.Current.Data);
                    List<Dify_WorkflowChunkCompletionResDto> updates = [Dify_WorkflowChunkCompletionResDto.DeserializeWorkflowChunkCompletionResDto(doc.RootElement)];
                    _updates = updates.GetEnumerator();

                    if (_updates.MoveNext())
                    {
                        _current = _updates.Current;
                        return true;
                    }
                }

                _current = default;
                return false;
            }

            private IEnumerator<SseItem<byte[]>> CreateEventEnumerator()
            {
                if (_response.ContentStream is null)
                {
                    throw new InvalidOperationException("Unable to create result from response with null ContentStream");
                }

                IEnumerable<SseItem<byte[]>> enumerable = SseParser.Create(_response.ContentStream, (_, bytes) => bytes.ToArray()).Enumerate();
                return enumerable.GetEnumerator();
            }

            public void Reset()
            {
                throw new NotSupportedException("Cannot seek back in an SSE stream.");
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing && _events is not null)
                {
                    _events.Dispose();
                    _events = null;

                    // Dispose the response so we don't leave the network connection open.
                    _response?.Dispose();
                }
            }
        }
    }
}
