// -----------------------------------------------------------------------
// <copyright file="IMenuService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.DesignObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface IMenuService : IApplicationService
    {
        Task<IServiceResult<IList<MenuDto>>> GetMenuAsync();

        Task<IServiceResult<MenuDto>> GetMenuItemAsync(string name);

        IServiceResult<MenuDto> Add(MenuDto menu);
    }
}
