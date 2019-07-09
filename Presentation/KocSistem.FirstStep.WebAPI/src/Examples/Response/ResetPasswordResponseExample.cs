// -----------------------------------------------------------------------
// <copyright file="ResetPasswordResponseExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.I18N;
using Swashbuckle.AspNetCore.Filters;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ResetPasswordResponseExample : IExamplesProvider
    {
        private readonly IKsI18N i18n;
        private readonly IKsStringLocalizer<UserController> localize;

        public ResetPasswordResponseExample(IKsI18N i18n)
        {
            this.i18n = i18n;
            this.localize = this.i18n.GetLocalizer<UserController>();
        }

        public object GetExamples()
        {
            return this.localize["PasswordResetSuccessful"];
        }
    }
}
