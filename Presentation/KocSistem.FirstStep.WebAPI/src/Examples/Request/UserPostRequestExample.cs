// <copyright file="UserPostRequestExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using Swashbuckle.AspNetCore.Filters;
using System;

namespace KocSistem.FirstStep.WebAPI
{
    internal class UserPostRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var password = Guid.NewGuid().ToString().Remove(5);

            return new UserPostRequest
            {
                Email = "ghostbusters@kocsistem.com.tr",
                Password = password,
            };
        }
    }
}