// -----------------------------------------------------------------------
// <copyright file="ClaimListResponseExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace KocSistem.FirstStep.WebAPI
{

    internal class ClaimListResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var list = new List<ClaimResponse>
            {
                new ClaimResponse
                {
                    Name = "Claim name 1",
                },
                new ClaimResponse
                {
                    Name = "Claim name 2",
                },
            };

            return list;
        }
    }
}
