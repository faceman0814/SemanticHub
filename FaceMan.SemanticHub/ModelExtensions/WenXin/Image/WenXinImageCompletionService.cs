using DocumentFormat.OpenXml.EMMA;

using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;

using Microsoft.SemanticKernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin.Image
{
    public class WenXinImageCompletionService
    {
        private readonly string _secret;
        private readonly string _key;
        private readonly string _model;
        private readonly ModelClient client;
        public WenXinImageCompletionService(string key, string secret, string model, string url = null)
        {
            _model = model;
            _secret = secret;
            _key = key;
            client = new(key, ModelType.TongYi, url);
        }
        public async Task<List<string>> GetImageMessageContentsAsync(string prompt, ImageParameters parameters, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var results = new List<string>();
            parameters.Token = await client.WenXin.GetAccessToken(_key, _secret);
            var result = await client.WenXin.GetImageMessageContentsAsync(_model, prompt, parameters);
            return result;
        }
    }
}
