using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    public record SemanticHubTongYiImageRequestWrapper
    {
        public static WanXiangRequestWrapper<TInput, TParameters> Create<TInput, TParameters>(string model, TInput input, TParameters? parameters = default) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            input = input ?? throw new ArgumentNullException(nameof(input)),
            Parameters = parameters,
        };

        public static WanXiangRequestWrapper<TInput, object> Create<TInput>(string model, TInput input) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            input = input ?? throw new ArgumentNullException(nameof(input)),
        };
    }

    public record WanXiangRequestWrapper<TInput, TParameters> : SemanticHubTongYiImageRequestWrapper
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("input")]
        public TInput input { get; init; }

        [JsonPropertyName("parameters")]
        public TParameters? Parameters { get; init; }
    }
}
