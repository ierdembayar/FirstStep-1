// -----------------------------------------------------------------------
// <copyright file="RoleControllerTest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using KocSistem.FirstStep.WebAPI;
using KocSistem.FirstStep.WebAPI.Model;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using KocSistem.OneFrame.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace KocSistem.FirstStep.Tests
{
    public class RoleControllerTest
    {
        private readonly Mock<IKsI18N> localizationMock;
        private readonly Mock<IMapper> mapperMock;

        private readonly Mock<RoleManager<ApplicationRole>> roleManagerMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;

        public RoleControllerTest()
        {
            this.localizationMock = new Mock<IKsI18N>();
            this.mapperMock = new Mock<IMapper>();

            this.roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
                   new Mock<IRoleStore<ApplicationRole>>().Object,
                   Array.Empty<IRoleValidator<ApplicationRole>>(),
                   new Mock<ILookupNormalizer>().Object,
                   new Mock<IdentityErrorDescriber>().Object,
                   new Mock<ILogger<RoleManager<ApplicationRole>>>().Object);

            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    new Mock<IUserStore<ApplicationUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    Array.Empty<IUserValidator<ApplicationUser>>(),
                    Array.Empty<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public void Get_AllRoles_ReturnsOkResult()
        {
            var applicationRoles = new List<ApplicationRole>().AsQueryable();
            this.roleManagerMock.SetupGet(roleManagerMock => roleManagerMock.Roles).Returns(applicationRoles);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = controller.Get();

            Assert.IsAssignableFrom<OkObjectResult>(response);
            OkObjectResult result = response as OkObjectResult;
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Get_ExistingRole_ReturnsOkResult()
        {
            var role = new ApplicationRole();
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(role);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.Get("rolename").ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);
            OkObjectResult result = response as OkObjectResult;
            Assert.IsAssignableFrom<ApplicationRole>(result.Value);
            var roleResult = result.Value as ApplicationRole;
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Get_NonExistingRole_ReturnsNotFoundResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.Get(string.Empty).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Post_ValidModel_ReturnsCreatedAtRouteResult()
        {
            string roleName = "admin";
            string roleDescription = "admin role desc";

            RolePostRequest model = new RolePostRequest { Name = roleName, Description = roleDescription };

            this.roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            controller.BindViewModel(model);

            var response = await controller.Post(model).ConfigureAwait(false);
            Assert.IsAssignableFrom<CreatedAtRouteResult>(response);

            CreatedAtRouteResult result = response as CreatedAtRouteResult;
            Assert.Equal("RoleGet", result.RouteName);
            Assert.True(result.RouteValues.Keys.Contains("roleName"));
            Assert.True(result.RouteValues.Values.Contains("admin"));
            Assert.Equal(result.Value, model);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Post_CreateRoleFailed_ReturnsOneFrameWebException()
        {
            string roleName = "admin";
            string roleDescription = "admin role desc";

            RolePostRequest model = new RolePostRequest { Name = roleName, Description = roleDescription };

            this.roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Failed());

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var ex = await Record.ExceptionAsync(() => controller.Post(model)).ConfigureAwait(false);
            Assert.IsAssignableFrom<OneFrameWebException>(ex);
            Assert.Equal(expected: (ex as OneFrameWebException).HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Patch_ValidModel_ReturnsOkResult()
        {
            string roleName = "admin";
            string roleDescription = "admin role desc";

            RolePatchRequest model = new RolePatchRequest { Description = roleDescription };
            this.roleManagerMock.Setup(r => r.FindByNameAsync("admin")).ReturnsAsync(new ApplicationRole());
            this.roleManagerMock.Setup(r => r.UpdateAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            controller.BindViewModel(model);

            var response = await controller.Patch(roleName, model).ConfigureAwait(false);
            Assert.IsAssignableFrom<OkObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Put_NonExistingRole_ReturnsNotFoundResult()
        {
            var rolePutModel = new RolePatchRequest { Description = "desc" };
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.Patch(null, rolePutModel).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Delete_ExistingRole_ReturnsNoContentResult()
        {
            string roleName = "admin";
            string roleDescription = "admin role desc";

            var role = new ApplicationRole { Name = roleName, Description = roleDescription };
            this.roleManagerMock.Setup(r => r.FindByNameAsync(roleName)).ReturnsAsync(role);
            this.roleManagerMock.Setup(r => r.DeleteAsync(role)).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.Delete(roleName).ConfigureAwait(false);

            Assert.IsAssignableFrom<NoContentResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Delete_NonExistingRole_ReturnsNotFoundResult()
        {
            string roleName = "admin";
            string roleDescription = "admin role desc";
            var role = new ApplicationRole { Name = roleName, Description = roleDescription };
            this.roleManagerMock.Setup(r => r.DeleteAsync(role)).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.Delete(roleName).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task Delete_DeleteRoleFailed_ReturnsOneFrameWebException()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.roleManagerMock.Setup(r => r.DeleteAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Failed());

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var ex = await Record.ExceptionAsync(() => controller.Delete(null)).ConfigureAwait(false);
            Assert.IsAssignableFrom<OneFrameWebException>(ex);
            var oneFrameWebException = ex as OneFrameWebException;
            Assert.Equal(expected: oneFrameWebException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task GetUsersInRole_NonExistingRole_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.GetUsersInRole(null).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task GetUsersInRole_RoleHasNoUsers_ReturnsEmptyList()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync((List<ApplicationUser>)null);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.GetUsersInRole("testrole").ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task GetUsersInRole_RoleHasUsers_ReturnsUsersInRole()
        {
            ApplicationUser firstUser = new ApplicationUser
            {
                UserName = "firstuser",
                Email = "firstuser@kocsistem.com.tr",
            };

            ApplicationUser secondUser = new ApplicationUser
            {
                UserName = "seconduser",
                Email = "seconduser@kocsistem.com.tr",
            };

            List<ApplicationUser> users = new List<ApplicationUser>
            {
                firstUser,
                secondUser,
            };

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(users);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.GetUsersInRole("testrole").ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);

            OkObjectResult okResult = response as OkObjectResult;
            Assert.IsAssignableFrom<List<RoleUserResponse>>(okResult.Value);
            var responseUsers = okResult.Value as List<RoleUserResponse>;

            var firstResponseUserList = responseUsers.Where(u => u.Username == firstUser.UserName);
            Assert.Single(firstResponseUserList);
            Assert.Equal(firstUser.UserName, firstResponseUserList.First().Username);
            Assert.Equal(firstUser.Email, firstResponseUserList.First().Email);

            var secondResponseUserList = responseUsers.Where(u => u.Username == secondUser.UserName);
            Assert.Single(secondResponseUserList);
            var secondResponseUser = responseUsers.Where(u => u.Username == secondUser.UserName).First();
            Assert.Equal(secondUser.UserName, secondResponseUserList.First().Username);
            Assert.Equal(secondUser.Email, secondResponseUserList.First().Email);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task AddUserToRole_NonExistingRole_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);
            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.AddUserToRole("testrole", "testuser").ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task AddUserToRole_NonExistingUser_ReturnsBadRequestObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var ex = await Record.ExceptionAsync(() => controller.AddUserToRole("testrole", "testuser")).ConfigureAwait(false);
            Assert.IsAssignableFrom<OneFrameWebException>(ex);
            var oneFrameWebException = ex as OneFrameWebException;
            Assert.Equal(expected: oneFrameWebException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task AddUserToRole_AddToRoleIsFailed_ReturnsBadRequestObjectResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMock.Setup(u => u.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var ex = await Record.ExceptionAsync(() => controller.AddUserToRole("testrole", "testuser")).ConfigureAwait(false);
            Assert.IsAssignableFrom<OneFrameWebException>(ex);
            var oneFrameWebException = ex as OneFrameWebException;
            Assert.Equal(expected: oneFrameWebException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task AddUserToRole_AddToRoleIsSuccessful_ReturnsOkResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMock.Setup(u => u.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var result = await controller.AddUserToRole(null, null).ConfigureAwait(false);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveUserFromRole_NonExistingRole_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.RemoveUserFromRole("testrole", "testuser").ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveUserFromRole_NonExistingUser_ReturnsNotFoundResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var result = await controller.RemoveUserFromRole(null, null).ConfigureAwait(false);
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveUserFromRole_AddToRoleIsFailed_ReturnsBadRequestObjectResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMock.Setup(u => u.RemoveFromRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var ex = await Record.ExceptionAsync(() => controller.RemoveUserFromRole("testrole", "testuser")).ConfigureAwait(false);
            Assert.IsAssignableFrom<OneFrameWebException>(ex);
            var oneFrameWebException = ex as OneFrameWebException;
            Assert.Equal(oneFrameWebException.HttpStatusCode.ToString(), Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveUserFromRole_AddToRoleIsSuccessful_ReturnsNoContentResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMock.Setup(u => u.RemoveFromRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var result = await controller.RemoveUserFromRole(null, null).ConfigureAwait(false);

            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task GetClaimsInRole_NonExistingRole_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.GetClaimsInRole(null).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task GetClaimsInRole_RoleHasNoClaims_ReturnsEmptyList()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            this.roleManagerMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationRole>())).ReturnsAsync((List<Claim>)null);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.GetClaimsInRole("testrole").ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async void AddClaimToRole_ReturnsOkResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());

            this.roleManagerMock.Setup(r => r.AddClaimAsync(It.IsAny<ApplicationRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var response = await controller.AddClaimToRole("menu", new RoleClaimPostRequest() { Name = "admin" }).ConfigureAwait(false);

            Assert.IsAssignableFrom<OkResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async void AddClaimToRole_ReturnsNotFoundResult()
        {
            var user = new ApplicationUser();

            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            this.roleManagerMock.Setup(r => r.AddClaimAsync(It.IsAny<ApplicationRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var response = await controller.AddClaimToRole("menu", new RoleClaimPostRequest() { Name = "admin" }).ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveClaimFromRole_NonExistingRole_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationRole)null);

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);
            var response = await controller.RemoveClaimFromRole("testrole", "testclaim").ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveClaimFromRole_NonExistingClaim_ReturnsNotFoundObjectResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());

            this.roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(new List<Claim>());

            this.localizationMock.SetupGet(x => x.GetLocalizer<RoleController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var response = await controller.RemoveClaimFromRole("testrole", "testclaim").ConfigureAwait(false);

            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "RoleController")]
        public async Task RemoveClaimFromRole_AddToRoleIsSuccessful_ReturnsNoContentResult()
        {
            this.roleManagerMock.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationRole());
            var claimList = new List<Claim> { new Claim("testclaimType", "testclaimValue") };
            this.roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(claimList);

            this.roleManagerMock.Setup(u => u.RemoveClaimAsync(It.IsAny<ApplicationRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new RoleController(this.roleManagerMock.Object, this.userManagerMock.Object, this.localizationMock.Object, this.mapperMock.Object);

            var response = await controller.RemoveClaimFromRole("testrole", "testclaimValue").ConfigureAwait(false);

            Assert.IsAssignableFrom<NoContentResult>(response);
        }
    }
}