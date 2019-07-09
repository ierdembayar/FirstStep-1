// <copyright file="IdentitySeedException.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.OneFrame.ErrorHandling;
using System;

namespace KocSistem.FirstStep.Application
{
    [Serializable]
    public class DataSeedException : OneFrameException
    {
        protected DataSeedException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
        {
        }

        public DataSeedException()
        {
        }

        public DataSeedException(string message)
    : base(message)
        {
        }

        public DataSeedException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}