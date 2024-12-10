using System.ClientModel.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;

using IcedMango.DifyAi.Request;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace DifyAi.Dto.ParamDto;

public class Dify_WorkflowParamDto : Dify_BaseRequestParamDto, IJsonModel<Dify_WorkflowParamDto>
{
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }
    public Dify_WorkflowParamDto() { }
    public Dify_WorkflowParamDto(string user, Dictionary<string, string> inputs, string responseMode)
    {
        this.User = user;
        this.Inputs = inputs;
        this.ResponseMode = responseMode;
    }

    /// <summary>
    ///     User identifier, used to define the identity of the end-user for retrieval and statistics.
    ///     Should be uniquely defined by the developer within the application.
    /// </summary>
    public string User { get; set; }


    /// <summary>
    /// Allows the entry of various variable values defined by the App
    /// </summary>
    public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// The mode of response return
    /// </summary>
    [JsonProperty("response_mode")]
    public string ResponseMode { get; internal set; }

    void IJsonModel<Dify_WorkflowParamDto>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowParamDto>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(Dify_WorkflowParamDto)} does not support writing '{format}' format.");
        }

        writer.WriteStartObject();
        if (SerializedAdditionalRawData?.ContainsKey("user") != true)
        {
            writer.WritePropertyName("user"u8);
            writer.WriteStringValue(User);
            
        }
        if (SerializedAdditionalRawData?.ContainsKey("inputs") != true)
        {
            if (Inputs != null)
            {
                writer.WritePropertyName("inputs"u8);
                writer.WriteStartObject();
                foreach (var item in Inputs)
                {
                    writer.WritePropertyName(item.Key.ToString(CultureInfo.InvariantCulture));
                    writer.WriteStringValue(item.Value);
                }
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull("inputs");
            }

          

        }
        if (SerializedAdditionalRawData?.ContainsKey("response_mode") != true)
        {
            writer.WritePropertyName("response_mode"u8);
            writer.WriteStringValue(ResponseMode);
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

    Dify_WorkflowParamDto IJsonModel<Dify_WorkflowParamDto>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowParamDto>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(Dify_WorkflowParamDto)} does not support reading '{format}' format.");
        }

        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeChatCompletionOptions(document.RootElement, options);
    }

    public Dify_WorkflowParamDto Create(BinaryData data, ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowParamDto>)this).GetFormatFromOptions(options) : options.Format;

        switch (format)
        {
            case "J":
                {
                    using JsonDocument document = JsonDocument.Parse(data);
                    return DeserializeChatCompletionOptions(document.RootElement, options);
                }
            default:
                throw new FormatException($"The model {nameof(Dify_WorkflowParamDto)} does not support reading '{options.Format}' format.");
        }
    }

    public string GetFormatFromOptions(ModelReaderWriterOptions options)
    {
        return "J";
    }

    public BinaryData Write(ModelReaderWriterOptions options)
    {
        var format = options.Format == "W" ? ((IPersistableModel<Dify_WorkflowParamDto>)this).GetFormatFromOptions(options) : options.Format;

        switch (format)
        {
            case "J":
                return ModelReaderWriter.Write(this, options);
            default:
                throw new FormatException($"The model {nameof(Dify_WorkflowParamDto)} does not support writing '{options.Format}' format.");
        }
    }


    internal static Dify_WorkflowParamDto DeserializeChatCompletionOptions(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= new ModelReaderWriterOptions("W");

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string user = default;
        Dictionary<string, string> inputs = default;
        string responseMode = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("response_mode"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    responseMode = null;
                    continue;
                }
                responseMode = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("inputs"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    inputs = null;
                }
                else
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (var property0 in property.Value.EnumerateObject())
                    {
                        dictionary.Add(property0.Name, property0.Value.GetString());
                    }
                    inputs = dictionary;
                }
                continue;
            }

            if (property.NameEquals("user"u8))
            {
                user = property.Value.GetString();
                continue;
            }

            if (true)
            {
                rawDataDictionary ??= new Dictionary<string, BinaryData>();
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new Dify_WorkflowParamDto()
        {
            User = user,
            Inputs = inputs,
            ResponseMode = responseMode,
        };
    }

    
}
