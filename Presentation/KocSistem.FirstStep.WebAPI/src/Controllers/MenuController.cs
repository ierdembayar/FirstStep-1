// -----------------------------------------------------------------------
// <copyright file="MenuController.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.OneFrame.Mapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.WebAPI
{
    [Route("menu")]
    public class MenuController : Controller
    {
        private readonly IMenuService menuService;
        private readonly IMapper mapper;

        public MenuController(IMenuService menuService, IMapper mapper)
        {
            this.menuService = menuService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns menu items.
        /// </summary>
        /// <returns>Returns list of MenuModel items. Returns http status codes(200,500).</returns>
        [HttpGet]
        [SwaggerResponse(200, type: typeof(IList<MenuResponse>), description: "Return menus")]
        [SwaggerResponse(404, type: typeof(void), description: "Menu item(s) was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Get()
        {
            var result = await this.menuService.GetMenuAsync().ConfigureAwait(false);

            var menu = result.Value;
            var userMenuIds = this.User.Claims.Where(w => w.Type == "Menu").Select(s => int.Parse(s.Value, CultureInfo.InvariantCulture));
            var userMenu = menu.Where(w => userMenuIds.Contains(w.Id)).ToArray();
            var menuModel = this.mapper.Map<IList<MenuDto>, IList<MenuResponse>>(userMenu);

            if (menuModel == null)
            {
                return this.NotFound();
            }

            var treeHelper = new TreeHelper(menuModel);

            return this.Ok(treeHelper.BuildMenuTree());
        }
    }
}
