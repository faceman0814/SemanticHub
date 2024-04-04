using FaceMan.SemanticHub.Generation.ImageGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    public record SemanticHubTongYiImageResponseWrapper
    {
        /// <summary>
        /// 本次请求的异步任务的作业 id，实际作业结果需要通过异步任务查询接口获取。
        /// </summary>
        [JsonPropertyName("task_id")]
        public string TaskId { get; init; } = null!;
        /// <summary>
        /// 提交异步任务后的作业状态。
        /// </summary>
        [JsonPropertyName("task_status")]
        public ImageTaskStatusEnum TaskStatus { get; init; }

        /// <summary>
        ///本次请求的系统唯一码
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; init; } = null!;
    }
}
