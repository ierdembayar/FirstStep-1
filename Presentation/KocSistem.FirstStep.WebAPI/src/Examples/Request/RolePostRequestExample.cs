// -----------------------------------------------------------------------
// <copyright file="RolePostRequestExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;

namespace KocSistem.FirstStep.WebAPI
{
    internal class RolePostRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RolePostRequest
            {
                Name = "Admin",
                Description = "Admin description",
            };
        }
    }
}
