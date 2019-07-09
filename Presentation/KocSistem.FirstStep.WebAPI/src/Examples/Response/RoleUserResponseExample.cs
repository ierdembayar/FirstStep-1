// -----------------------------------------------------------------------
// <copyright file="RoleUserResponseExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;

namespace KocSistem.FirstStep.WebAPI
{
    internal class RoleUserResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RoleUserResponse
            {
                Email = "ghostbusters@kocsistem.com.tr",
                Username = "oneframe",
            };
        }
    }
}
