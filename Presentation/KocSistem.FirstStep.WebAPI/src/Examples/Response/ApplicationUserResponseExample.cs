// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ApplicationUserResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "ghostbusters@kocsistem.com.tr",
                UserName = "ghostbusters@kocsistem.com.tr",
                NormalizedEmail = "GHOSTBUSTERS@KOCSISTEM.COM.TR",
                NormalizedUserName = "GHOSTBUSTERS@KOCSISTEM.COM.TR",
            };
        }
    }
}