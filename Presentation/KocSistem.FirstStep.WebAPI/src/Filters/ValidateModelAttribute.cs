// <copyright file="ValidateModelAttribute.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace KocSistem.FirstStep.WebAPI
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly IKsI18N i18n;
        private readonly IKsStringLocalizer<ValidateModelAttribute> localize;

        public ValidateModelAttribute(IKsI18N i18n)
        {
            this.i18n = i18n;
            this.localize = this.i18n.GetLocalizer<ValidateModelAttribute>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, localize["InvalidModel"]);
            }
        }
    }
}