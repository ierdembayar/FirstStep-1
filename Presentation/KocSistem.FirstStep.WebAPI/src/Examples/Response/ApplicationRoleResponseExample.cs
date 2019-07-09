// -----------------------------------------------------------------------
// <copyright file="ApplicationRoleResponseExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ApplicationRoleResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Description = "Admin",
                Name = "Admin",
                NormalizedName = "ADMIN",
            };
        }
    }
}
