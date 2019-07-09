// -----------------------------------------------------------------------
// <copyright file="IdentityMockHelpers.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.Tests
{
    public static class IdentityMockHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>()
            where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null)
            where TRole : class
        {
            store = store ?? new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>();

            roles.Add(new RoleValidator<TRole>());

            return new Mock<RoleManager<TRole>>(
                store,
                roles,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null);
        }

        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>()
    where TUser : class
        {
            var context = new Mock<HttpContext>();
            var manager = MockUserManager<TUser>();

            return new Mock<SignInManager<TUser>>(
                manager.Object,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null,
                null,
                null)
            {
                CallBase = true,
            };
        }
    }
}
