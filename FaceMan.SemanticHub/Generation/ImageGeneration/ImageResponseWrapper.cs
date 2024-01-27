using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public record ImageResponseWrapper<TOutput, TUsage>
    {
        /// <summary>
        /// The identifier corresponds to each individual request.
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; init; }

        /// <summary>
        /// The processed task status response associated with the respective request.
        /// </summary>
        [JsonPropertyName("output")]
        public TOutput Output { get; init; }

        /// <summary>
        /// Usage of the request.
        /// </summary>
        [JsonPropertyName("usage")]
        public TUsage? Usage { get; init; }
    }
}
