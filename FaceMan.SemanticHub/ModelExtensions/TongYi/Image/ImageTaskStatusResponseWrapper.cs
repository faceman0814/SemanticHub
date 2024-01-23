using FaceMan.SemanticHub.ConverterHelper;
using FaceMan.SemanticHub.ModelExtensions.ImageGeneration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    /// <summary>
    /// 基本输出类，包含响应文本到图像操作的常用属性。
    /// </summary>
    public record ImageTaskStatusResponseWrapper
    {
        /// <summary>
        /// 本次请求的异步任务的作业 id，实际作业结果需要通过异步任务查询接口获取。
        /// </summary>
        [JsonPropertyName("task_id")]
        public string TaskId { get; init; }

        /// <summary>
        /// 被查询作业的作业状态
        /// </summary>
        [JsonPropertyName("task_status")]
        public ImageTaskStatusEnum TaskStatus { get; init; }

        /// <summary>
        /// 作业中每个batch任务的状态
        /// </summary>
        [JsonPropertyName("task_metrics")]
        public TaskMetrics? TaskMetrics { get; init; }

        /// <summary>
        /// 本次请求成功生成的图片张数
        /// </summary>
        [JsonPropertyName("image_count")]
        public int ImageCount { get; init; }

        /// <summary>
        /// 任务提交的时间。
        /// </summary>
        [JsonPropertyName("submit_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime SubmitTime { get; init; }

        /// <summary>
        /// 任务计划运行的时间。
        /// </summary>
        [JsonPropertyName("scheduled_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime ScheduledTime { get; init; }

        /// <summary>
        /// 与任务相关的键值格式附加数据。
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraData { get; init; }

        /// <summary>
        /// 如果任务处于成功状态，则将当前实例转换为 <see cref="SuccessTaskResponse"/> 。
        /// 如果操作无效，则抛出 <see cref="InvalidOperationException"/> 异常。
        /// </summary>返回
        /// <returns><see cref="SuccessTaskResponse"/>的实例。</returns>
        public SuccessTaskResponseWrapper AsSuccess()
        {
            if (this.TaskStatus != ImageTaskStatusEnum.Succeeded || ExtraData == null)
            {
                throw new InvalidOperationException($"Text2ImageBaseOutput is not in succeed status.");
            }

            return JsonSerializer.Deserialize<SuccessTaskResponseWrapper>(JsonSerializer.Serialize(this))!;
        }


        /// <summary>
        /// 如果任务处于失败状态，则将当前实例转换为<see cref="FailedTaskResponse"/>。
        /// 如果操作无效，则抛出 <see cref="InvalidOperationException"/> 异常。
        /// </summary> 
        /// <returns><see cref="FailedTaskResponse"/>的实例.</returns>。
        public FailedTaskResponseWrapper AsFailed()
        {
            if (this.TaskStatus != ImageTaskStatusEnum.Failed || ExtraData == null)
            {
                throw new InvalidOperationException($"Text2ImageBaseOutput is not in failed status.");
            }

            return JsonSerializer.Deserialize<FailedTaskResponseWrapper>(JsonSerializer.Serialize(this))!;
        }
    }

    /// <summary>
    /// 在文本到图像任务失败时提供详细信息的输出类。
    /// </summary>
    public record FailedTaskResponseWrapper : ImageTaskStatusResponseWrapper
    {
        /// <summary>
        /// 任务结束的时间。
        /// </summary>
        [JsonPropertyName("end_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EndTime { get; init; }

        /// <summary>
        /// 与故障相关的错误代码。
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; init; }

        /// <summary>
        /// 有关故障的描述性信息。
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; init; }
    }

    /// <summary>
    ///  代表成功完成文本到图像任务的输出类。
    /// </summary>
    public record SuccessTaskResponseWrapper : ImageTaskStatusResponseWrapper
    {
        /// <summary>
        /// 任务生成的图像结果列表.
        /// </summary>
        [JsonPropertyName("results")]
        public List<ImageResult> Results { get; init; }

        /// <summary>
        /// 任务成功结束的时间。
        /// </summary>
        [JsonPropertyName("end_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EndTime { get; init; }
    }

    public record TaskMetrics
    {
        /// <summary>
        /// 获取或初始化任务总数。
        /// </summary>
        [JsonPropertyName("TOTAL")]
        public int Total { get; init; }

        /// <summary>
        /// 获取或初始化成功完成的任务数。
        /// </summary>
        [JsonPropertyName("SUCCEEDED")]
        public int Succeeded { get; init; }

        /// <summary>
        /// 获取或初始化失败任务的数量。
        /// </summary>
        [JsonPropertyName("FAILED")]
        public int Failed { get; init; }
    }

    public record ImageResult
    {
        /// <summary>
        /// 获取或设置图片的 URL。
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; }

        /// <summary>
        /// 获取或设置与图片相关的代码。
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; init; }

        /// <summary>
        /// 获取或设置错误信息。
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; init; }

        /// <summary>
        /// 获取指示图像生成是否成功的值。
        /// </summary>
        public bool IsSuccess => Code == null && Message == null;
    }
}
