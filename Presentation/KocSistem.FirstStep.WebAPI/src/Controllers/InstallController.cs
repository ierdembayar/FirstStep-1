// <copyright file="InstallController.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KocSistem.FirstStep.WebAPI
{
    [Route("install")]
    public class InstallController : Controller
    {
        private readonly IInitialDataSeedService identitySeedService;

        public InstallController(IInitialDataSeedService identitySeedService)
        {
            this.identitySeedService = identitySeedService;
        }

        /// <summary>
        /// İnitial data seed.
        /// </summary>
        /// <returns>Returns http status codes(200,500).</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, "Seed successfully completed.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public IActionResult Index()
        {
            this.identitySeedService.Seed();
            return this.Ok();
        }
    }
}
