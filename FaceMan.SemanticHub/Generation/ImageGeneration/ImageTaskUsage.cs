using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public class ImageTaskUsage
    {
        /// <summary>
        /// Image count
        /// </summary>
        [JsonPropertyName("image_count")]
        public int ImageCount { get; set; }
    }
}
