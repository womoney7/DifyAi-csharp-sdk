using IcedMango.DifyAi.Request;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System.ClientModel.Primitives;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace DifyAi.Dto.ResDto;

public class Dify_WorkflowChunkCompletionResDto : IJsonModel<Dify_WorkflowChunkCompletionResDto>
{

    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }

    /// <summary>
    /// Workflow Run ID
    /// </summary>
    [JsonProperty("workflow_run_id")]
    public string WorkflowRunId { get; set; }

    /// <summary>
    /// Task ID
    /// </summary>
    [JsonProperty("task_id")]
    public string TaskId { get; set; }

    public string Event { get; set; }

    public WorkflowChunkCompletionData Data { get; set; }

    public static Dify_WorkflowChunkCompletionResDto DeserializeWorkflowChunkCompletionResDto(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= new ModelReaderWriterOptions("W");

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        string workflowRunId = default;
        string taskId = default;
        string dataEvent = default;
        WorkflowChunkCompletionData data = default;

        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("workflow_run_id"u8))
            {
                workflowRunId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("task_id"u8))
            {
                taskId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("event"u8))
            {
                dataEvent = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("data"u8))
            {
                data = DeserializeData(property.Value, options);
                continue;
            }
            if (true)
            {
                rawDataDictionary ??= new Dictionary<string, BinaryData>();
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new Dify_WorkflowChunkCompletionResDto()
        {
            WorkflowRunId = workflowRunId,
            Event = dataEvent,
            TaskId = taskId,
            Data = data,
            SerializedAdditionalRawData = serializedAdditionalRawData
        };
    }

    public static WorkflowChunkCompletionData DeserializeData(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string id = default;
        string nodeId = default;
        string nodeType = default;
        string workflowId = default;
        string title = default;
        int index = default;
        string text = default;
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        Dictionary<string, object> outputs = new Dictionary<string, object>();
        Dictionary<string, object> processData = new Dictionary<string, object>();
        string status = default;
        string error = default;
        float? elapsedTime = default;
        int? totalTokens = default;
        int? totalSteps = default;
        string predecessorNodeId = default;
        int? sequenceNumber = default;
        DateTimeOffset? createdAt = default;
        DateTimeOffset? finishedAt = default;
        List<string> fromVariableSelector = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("id"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                id = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("node_id"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                nodeId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("node_type"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                nodeType = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("workflow_id"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                workflowId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("title"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                title = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("index"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                index = property.Value.GetInt32();
                continue;
            }
            if (property.NameEquals("inputs"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }

                inputs = new Dictionary<string, object>();
                foreach (var propInput in property.Value.EnumerateObject())
                {
                    if (propInput.Value.ValueKind == JsonValueKind.String)
                        inputs.Add(propInput.Name, propInput.Value.GetString());
                    else if (propInput.Value.ValueKind == JsonValueKind.Array)
                    {

                        outputs.Add(propInput.Name, propInput.Value.EnumerateArray().Select(x => x.GetString()).ToArray());
                    }
                }

                continue;
            }

            if (property.NameEquals("outputs"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }

                outputs = new Dictionary<string, object>();
                foreach (var propOutput in property.Value.EnumerateObject())
                {
                    if (propOutput.Value.ValueKind == JsonValueKind.String)
                        outputs.Add(propOutput.Name, propOutput.Value.GetString());
                    else if (propOutput.Value.ValueKind == JsonValueKind.Array)
                    {
                        outputs.Add(propOutput.Name, propOutput.Value.EnumerateArray().Select(x => x.GetString()).ToArray());
                    }

                }

                continue;
            }

            if (property.NameEquals("process_data"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                processData = new Dictionary<string, object>();
                foreach (var propProData in property.Value.EnumerateObject())
                {
                    if (propProData.Value.ValueKind == JsonValueKind.String)
                        processData.Add(propProData.Name, propProData.Value.GetString());
                    else if (propProData.Value.ValueKind == JsonValueKind.Array)
                    {
                        processData.Add(propProData.Name, propProData.Value.EnumerateArray().Where(p => p.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToArray());
                    }
                }

                continue;
            }
            if (property.NameEquals("status"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                status = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("error"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                error = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("elapsed_time"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    elapsedTime = null;
                    continue;
                }
                elapsedTime = property.Value.GetSingle();
                continue;
            }
            if (property.NameEquals("total_tokens"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    totalTokens = null;
                    continue;
                }
                totalTokens = property.Value.GetInt32();
                continue;
            }
            if (property.NameEquals("total_steps"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    totalSteps = null;
                    continue;
                }
                totalSteps = property.Value.GetInt32();
                continue;
            }
            if (property.NameEquals("predecessor_node_id"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {

                    continue;
                }
                predecessorNodeId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("sequence_number"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    sequenceNumber = null;
                    continue;
                }
                sequenceNumber = property.Value.GetInt32();
                continue;
            }
            if (property.NameEquals("created_at"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                createdAt = property.Value.GetDateTimeOffset("U");
                continue;
            }
            if (property.NameEquals("finished_at"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                finishedAt = property.Value.GetDateTimeOffset("U");
                continue;
            }
            if (property.NameEquals("text"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                text = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("text"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                text = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("from_variable_selector"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }

                fromVariableSelector = property.Value.EnumerateArray()
                    .Where(p => p.ValueKind == JsonValueKind.String)
                    .Select(x => x.GetString()).ToList();
                continue;
            }

            if (true)
            {
                rawDataDictionary ??= new Dictionary<string, BinaryData>();
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }

        return new WorkflowChunkCompletionData()
        {
            Id = id,
            WorkflowId = workflowId,
            NodeId = nodeId,
            NodeType = nodeType,
            Title = title,
            Index = index,
            Inputs = inputs,
            Outputs = outputs,
            ProcessData = processData,
            Status = status,
            Error = error,
            ElapsedTime = elapsedTime,
            TotalTokens = totalTokens,
            TotalSteps = totalSteps,
            PredecessorNodeId = predecessorNodeId,
            SequenceNumber = sequenceNumber,
            CreatedAt = createdAt,
            FinishedAt = finishedAt,
            Text = text,
            FromVarSelector = fromVariableSelector
        };
    }

    public Dify_WorkflowChunkCompletionResDto Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowChunkCompletionResDto>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(Dify_WorkflowChunkCompletionResDto)} does not support reading '{format}' format.");
        }

        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeWorkflowChunkCompletionResDto(document.RootElement, options);
    }

    public Dify_WorkflowChunkCompletionResDto Create(BinaryData data, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowChunkCompletionResDto>)this).GetFormatFromOptions(options) : options.Format;

        switch (format)
        {
            case "J":
                {
                    using JsonDocument document = JsonDocument.Parse(data);
                    return DeserializeWorkflowChunkCompletionResDto(document.RootElement, options);
                }
            default:
                throw new FormatException($"The model {nameof(Dify_WorkflowChunkCompletionResDto)} does not support reading '{options.Format}' format.");
        }
    }

    public string GetFormatFromOptions(ModelReaderWriterOptions options)
    {
        return "J";
    }

    public void Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowChunkCompletionResDto>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(Dify_WorkflowChunkCompletionResDto)} does not support writing '{format}' format.");
        }

        writer.WriteStartObject();
        if (SerializedAdditionalRawData?.ContainsKey("id") != true)
        {
            writer.WritePropertyName("workflow_run_id"u8);
            writer.WriteStringValue(WorkflowRunId);
        }
        if (SerializedAdditionalRawData?.ContainsKey("task_id") != true)
        {
            writer.WritePropertyName("task_id"u8);
            writer.WriteStringValue(TaskId);
        }
        if (SerializedAdditionalRawData?.ContainsKey("event") != true)
        {
            writer.WritePropertyName("event"u8);
            writer.WriteStringValue(TaskId);
        }
        if (SerializedAdditionalRawData?.ContainsKey("usage") != true)
        {
            writer.WritePropertyName("data"u8);
            writer.WriteObjectValue<WorkflowChunkCompletionData>(Data, options);
        }
        if (SerializedAdditionalRawData != null)
        {
            foreach (var item in SerializedAdditionalRawData)
            {
                if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                {
                    continue;
                }
                writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        System.Text.Json.JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
            }
        }
        writer.WriteEndObject();
    }

    public BinaryData Write(ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowChunkCompletionResDto>)this).GetFormatFromOptions(options) : options.Format;

        switch (format)
        {
            case "J":
                return ModelReaderWriter.Write(this, options);
            default:
                throw new FormatException($"The model {nameof(Dify_WorkflowChunkCompletionResDto)} does not support writing '{options.Format}' format.");
        }
    }
}

public class WorkflowChunkCompletionData
{
    public string Id { get; set; }

    [JsonProperty("node_id")]
    public string NodeId { get; set; }

    [JsonProperty("node_type")]
    public string NodeType { get; set; }

    [JsonProperty("workflow_id")]
    public string WorkflowId { get; set; }

    public string Title { get; set; }

    public int? Index { get; set; }

    public string Text { get; set; }

    [JsonProperty("from_variable_selector")]
    public List<string> FromVarSelector { get; set; }

    public Dictionary<string, object> Inputs { get; set; }

    public Dictionary<string, object> Outputs { get; set; }

    [JsonProperty("process_data")]
    public Dictionary<string, object> ProcessData { get; set; }

    public string Status { get; set; }

    public string Error { get; set; }

    [JsonProperty("elapsed_time")]
    public float? ElapsedTime { get; set; }

    [JsonProperty("total_tokens")]
    public int? TotalTokens { get; set; }

    [JsonProperty("total_steps")]
    public int? TotalSteps { get; set; }

    [JsonProperty("predecessor_node_id")]
    public string PredecessorNodeId { get; set; }

    [JsonProperty("sequence_number")]
    public int? SequenceNumber { get; set; }

    [JsonProperty("execution_metadata")]
    public Metadata ExecutionMetadata { get; set; } = null;

    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonProperty("finished_at")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset? FinishedAt { get; set; }
}

//public class Metadata
//{
//    [JsonProperty("total_tokens")]
//    public int TotalTokens { get; set; }

//    [JsonProperty("total_price")]
//    public Decimal TotalPrice { get; set; }

//    [JsonProperty("currency")]
//    public string Currency { get; set; }
//}