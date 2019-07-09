// -----------------------------------------------------------------------
// <copyright file="ForgotPasswordRequestExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ForgotPasswordRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ForgotPasswordRequest
            {
                Email = "ghostbusters@kocsistem.com.tr",
            };
        }
    }
}
