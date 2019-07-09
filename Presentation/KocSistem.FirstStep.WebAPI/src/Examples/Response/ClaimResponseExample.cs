// -----------------------------------------------------------------------
// <copyright file="ClaimResponseExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ClaimResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ClaimResponse
            {
                Name = "Claim name",
            };
        }
    }
}
