using Microsoft.SemanticKernel;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions
{
    public static class SemanticHubVerify
    {
        private static readonly Regex s_asciiLettersDigitsUnderscoresRegex = new Regex("^[0-9A-Za-z_]*$");

        //
        // 摘要:
        //     Equivalent of ArgumentNullException.ThrowIfNull
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNull([NotNull] object? obj, [CallerArgumentExpression("obj")] string? paramName = null)
        {
            if (obj == null)
            {
                ThrowArgumentNullException(paramName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNullOrWhiteSpace([NotNull] string? str, [CallerArgumentExpression("str")] string? paramName = null)
        {
            NotNull(str, paramName);
            if (string.IsNullOrWhiteSpace(str))
            {
                ThrowArgumentWhiteSpaceException(paramName);
            }
        }

        internal static void NotNullOrEmpty<T>(IList<T> list, [CallerArgumentExpression("list")] string? paramName = null)
        {
            NotNull(list, paramName);
            if (list.Count == 0)
            {
                throw new ArgumentException("The value cannot be empty.", paramName);
            }
        }

        public static void True(bool condition, string message, [CallerArgumentExpression("condition")] string? paramName = null)
        {
            if (!condition)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        internal static void ValidPluginName([NotNull] string? pluginName, IReadOnlyKernelPluginCollection? plugins = null, [CallerArgumentExpression("pluginName")] string? paramName = null)
        {
            NotNullOrWhiteSpace(pluginName, "pluginName");
            if (!s_asciiLettersDigitsUnderscoresRegex.IsMatch(pluginName))
            {
                ThrowArgumentInvalidName("plugin name", pluginName, paramName);
            }

            if (plugins != null && plugins.Contains(pluginName))
            {
                throw new ArgumentException("A plugin with the name '" + pluginName + "' already exists.");
            }
        }

        internal static void ValidFunctionName([NotNull] string? functionName, [CallerArgumentExpression("functionName")] string? paramName = null)
        {
            NotNullOrWhiteSpace(functionName, "functionName");
            if (!s_asciiLettersDigitsUnderscoresRegex.IsMatch(functionName))
            {
                ThrowArgumentInvalidName("function name", functionName, paramName);
            }
        }

        public static void ValidateUrl(string url, bool allowQuery = false, [CallerArgumentExpression("url")] string? paramName = null)
        {
            NotNullOrWhiteSpace(url, paramName);
            if (!Uri.TryCreate(url, UriKind.Absolute, out var result) || string.IsNullOrEmpty(result.Host))
            {
                throw new ArgumentException("The `" + url + "` is not valid.", paramName);
            }

            if (!allowQuery && !string.IsNullOrEmpty(result.Query))
            {
                throw new ArgumentException("The `" + url + "` is not valid: it cannot contain query parameters.", paramName);
            }

            if (!string.IsNullOrEmpty(result.Fragment))
            {
                throw new ArgumentException("The `" + url + "` is not valid: it cannot contain URL fragments.", paramName);
            }
        }

        internal static void StartsWith(string text, string prefix, string message, [CallerArgumentExpression("text")] string? textParamName = null)
        {
            NotNullOrWhiteSpace(text, textParamName);
            if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(textParamName, message);
            }
        }

        internal static void DirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Directory '" + path + "' could not be found.");
            }
        }

        //
        // 摘要:
        //     Make sure every function parameter name is unique
        //
        // 参数:
        //   parameters:
        //     List of parameters
        internal static void ParametersUniqueness(IReadOnlyList<KernelParameterMetadata> parameters)
        {
            int count = parameters.Count;
            if (count <= 0)
            {
                return;
            }

            HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < count; i++)
            {
                KernelParameterMetadata kernelParameterMetadata = parameters[i];
                if (string.IsNullOrWhiteSpace(kernelParameterMetadata.Name))
                {
                    string paramName = string.Format("{0}[{1}].{2}", "parameters", i, kernelParameterMetadata.Name);
                    if (kernelParameterMetadata.Name == null)
                    {
                        ThrowArgumentNullException(paramName);
                    }
                    else
                    {
                        ThrowArgumentWhiteSpaceException(paramName);
                    }
                }

                if (!hashSet.Add(kernelParameterMetadata.Name))
                {
                    throw new ArgumentException("The function has two or more parameters with the same name '" + kernelParameterMetadata.Name + "'");
                }
            }
        }

        [DoesNotReturn]
        private static void ThrowArgumentInvalidName(string kind, string name, string? paramName)
        {
            throw new ArgumentException("A " + kind + " can contain only ASCII letters, digits, and underscores: '" + name + "' is not a valid name.", paramName);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentNullException(string? paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentWhiteSpaceException(string? paramName)
        {
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException<T>(string? paramName, T actualValue, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }
    }
}
