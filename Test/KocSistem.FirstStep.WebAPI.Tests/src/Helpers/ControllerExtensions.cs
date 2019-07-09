// -----------------------------------------------------------------------
// <copyright file="ControllerExtensions.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KocSistem.FirstStep.Tests
{
    public static class ControllerExtensions
    {
        public static void BindViewModel<T>(this Controller controller, T model)
        {
            if (model == null)
            {
                return;
            }

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                controller.ModelState.Clear();
                foreach (ValidationResult result in results)
                {
                    var key = result.MemberNames.FirstOrDefault() ?? string.Empty;
                    controller.ModelState.AddModelError(key, result.ErrorMessage);
                }
            }
        }
    }
}
