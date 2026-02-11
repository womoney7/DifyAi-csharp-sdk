namespace DifyAi.Dto.Config;

public class DifyAiConfigDto
{
    /// <summary>
    ///     DifyAi Base Url
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    ///     DifyAi Api Key
    /// </summary>
    public string BotApiKey { get; set; }
    
    /// <summary>
    ///     DifyAi Dataset Api Key
    /// </summary>
    public string DatasetApiKey { get; set; }

    /// <summary>
    ///     网络请求超时时间（分钟），默认 5 分钟
    /// </summary>
    public int TimeoutMinutes { get; set; } = 5;
}