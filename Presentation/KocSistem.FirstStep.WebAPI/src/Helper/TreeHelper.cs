// -----------------------------------------------------------------------
// <copyright file="TreeHelper.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace KocSistem.FirstStep.WebAPI
{
    public class TreeHelper
    {
        private readonly IList<MenuResponse> userMenu;

        public TreeHelper(IList<MenuResponse> userMenu)
        {
            this.userMenu = userMenu;
        }

        public IList<MenuResponse> BuildMenuTree()
        {
            var response = new List<MenuResponse>();
            var menuSet = new Dictionary<int, MenuResponse>();
            foreach (var menuItem in this.userMenu)
            {
                var treeModel = new MenuResponse()
                {
                    Id = menuItem.Id,
                    Name = menuItem.Name,
                    Url = menuItem.Url,
                    ParentId = menuItem.ParentId,
                };
                menuSet.Add(menuItem.Id, treeModel);
                if (!menuItem.ParentId.HasValue)
                {
                    response.Add(treeModel);
                }
            }

            MenuResponse parent;
            foreach (var m in menuSet)
            {
                if (m.Value.ParentId.HasValue)
                {
                    parent = menuSet[m.Value.ParentId.Value];
                    parent.Children.Add(m.Value);
                }
            }

            return response;
        }
    }
}
