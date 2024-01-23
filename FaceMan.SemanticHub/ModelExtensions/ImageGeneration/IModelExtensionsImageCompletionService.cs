using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.ImageGeneration
{
    public interface IModelExtensionsImageCompletionService
    {
        /// <summary>
        /// 生成图像
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="kernel">预留字段</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<List<string>> GetImageMessageContentsAsync(string prompt, ImageParameters size, Kernel? kernel = null, CancellationToken cancellationToken = default);
    }
}
