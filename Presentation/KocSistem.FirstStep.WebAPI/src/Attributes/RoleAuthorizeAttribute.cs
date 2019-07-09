// -----------------------------------------------------------------------
// <copyright file="RoleAuthorizeAttribute.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Net;

namespace KocSistem.FirstStep.WebAPI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IKsI18N i18n;
        private readonly IKsStringLocalizer<ValidateModelAttribute> localize;

        public RoleAuthorizeAttribute(IKsI18N i18n)
        {
            this.i18n = i18n;
            this.localize = this.i18n.GetLocalizer<ValidateModelAttribute>();
        }

        public RoleAuthorizeAttribute(params Roles[] roles)
        {
            if (roles.Any(r => r.GetType() != typeof(Roles)))
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, localize["ParameterMustBeRole"]);
            }

            this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
    }
}