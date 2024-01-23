﻿using FaceMan.SemanticHub.ConverterHelper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.ImageGeneration
{
    [JsonConverter(typeof(TaskStatusConverter))]
    public enum ImageTaskStatusEnum
    {
        /// <summary>
        /// The job is queued and waiting to be processed.
        /// </summary>
        Pending,

        /// <summary>
        /// The job is currently being processed.
        /// </summary>
        Running,

        /// <summary>
        /// The job has been successfully completed.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The job has failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The job either does not exist or its status is unknown.
        /// </summary>
        Unknown,
    }
}
