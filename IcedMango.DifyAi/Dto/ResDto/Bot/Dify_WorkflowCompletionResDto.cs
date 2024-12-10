using Newtonsoft.Json;

namespace DifyAi.Dto.ResDto;

public class Dify_WorkflowCompletionResDto
{

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

    /// <summary>
    ///      App mode, fixed as chat
    /// </summary>
    public Data Data { get; set; }


}



public class Data
{
    [JsonProperty("workflow_id")] public string WorkflowId { get; set; }

    [JsonProperty("Status")] public string Status { get; set; }

    [JsonProperty("outputs")]
    public Outputs Outputs { get; set; }

    [JsonProperty("error")] public string Error { get; set; }

    [JsonProperty("total_tokens")] public int TotalTokens { get; set; }

    [JsonProperty("total_steps")] public int TotalSteps { get; set; }

    public string Id { get; set; }

    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("finished_at")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? FinishedAt { get; set; }
}

public class Outputs
{
    public string Text { get; set; }
}