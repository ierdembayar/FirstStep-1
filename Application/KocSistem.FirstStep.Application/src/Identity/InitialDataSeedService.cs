// <copyright file="IdentitySeedService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.FirstStep.Domain;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace KocSistem.FirstStep.Application
{
    public class InitialDataSeedService : IInitialDataSeedService
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Menu> menuRepository;

        private readonly string securePass = "123456";

        public InitialDataSeedService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IRepository<Menu> menuRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.menuRepository = menuRepository;
        }

        public void Seed()
        {
            var user = this.SeedUser("adminuser");
            var role = this.SeedRole(Roles.Admin.ToString());
            this.SeedRoleUser(user, role);
            user = this.SeedUser("guestuser");
            role = this.SeedRole(Roles.Guest.ToString());
            this.SeedRoleUser(user, role);
            user = this.SeedUser("oneframe");
            role = this.SeedRole(Roles.Admin.ToString());
            this.SeedRoleUser(user, role);

            this.SeedMenuItems();
            this.SeedRoleClaimForMenu();
        }

        private ApplicationUser SeedUser(string username)
        {
            var guestuser = this.userManager.FindByNameAsync(username).Result;

            if (guestuser != null)
            {
                return guestuser;
            }

            guestuser = new ApplicationUser() { UserName = username + "@kocsistem.com.tr", Email = username + "@kocsistem.com.tr" };
            var userResult = this.userManager.CreateAsync(guestuser, this.securePass).Result;

            if (!userResult.Succeeded)
            {
                throw new DataSeedException("Could not create seed user.");
            }

            return guestuser;
        }

        private ApplicationRole SeedRole(string rolename)
        {
            ApplicationRole role = this.roleManager.FindByNameAsync(rolename).Result;
            if (role != null)
            {
                return role;
            }

            role = new ApplicationRole() { Name = rolename, Description = rolename + " desc" };
            var roleResult = this.roleManager.CreateAsync(role).Result;
            if (!roleResult.Succeeded)
            {
                throw new DataSeedException("Could not create seed role.");
            }

            return role;
        }

        private void SeedRoleUser(ApplicationUser user, ApplicationRole role)
        {
            var roles = this.userManager.GetRolesAsync(user).Result;

            if (roles.Count == 0)
            {
                var roleUserResult = this.userManager.AddToRoleAsync(user, role.Name).Result;

                if (!roleUserResult.Succeeded)
                {
                    throw new DataSeedException("Could not create seed role user relation.");
                }
            }
        }

        private void SeedMenuItems()
        {
            var usersParentMenu = new Menu()
            {
                ParentId = null,
                Name = "User Management",
                Url = null,
            };

            this.menuRepository.Add(usersParentMenu);

            var usersChildMenu = new Menu()
            {
                ParentId = usersParentMenu.Id,
                Name = "Users",
                Url = "/User",
            };

            this.menuRepository.Add(usersChildMenu);

            var rolesParentMenu = new Menu()
            {
                ParentId = null,
                Name = "Role Management",
                Url = null,
            };

            this.menuRepository.Add(rolesParentMenu);

            var rolesChildMenu = new Menu()
            {
                ParentId = rolesParentMenu.Id,
                Name = "Roles",
                Url = "/Role",
            };

            this.menuRepository.Add(rolesChildMenu);
        }

        private void SeedRoleClaimForMenu()
        {
            ApplicationRole role = this.roleManager.FindByNameAsync(Roles.Admin.ToString()).Result;

            if (role == null)
            {
                throw new DataSeedException($"Role: {Roles.Admin.ToString()} could not find.");
            }

            var menuList = this.menuRepository.GetList();

            Claim claim;

            foreach (var menu in menuList)
            {
                claim = new Claim("Menu", menu.Id.ToString());

                var result = this.roleManager.AddClaimAsync(role, claim).Result;

                if (!result.Succeeded)
                {
                    throw new DataSeedException("Could not create seed role claim relation for menu items.");
                }
            }
        }
    }
}
