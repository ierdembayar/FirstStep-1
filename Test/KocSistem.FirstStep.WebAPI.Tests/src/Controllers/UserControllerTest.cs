// -----------------------------------------------------------------------
// <copyright file="UserControllerTest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Domain;
using KocSistem.FirstStep.WebAPI;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using KocSistem.OneFrame.Mapper.KsMapster;
using KocSistem.OneFrame.Notification.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KocSistem.FirstStep.Tests
{
    public class UserControllerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMoq;
        private readonly Mock<IConfiguration> configurationMoq;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMoq;
        private readonly Mock<RoleManager<ApplicationRole>> roleManagerMoq;
        private readonly Mock<ITokenHelper> tokenHelperMoq;
        private readonly Mock<IUrlHelper> urlMoq;
        private readonly Mock<IKsI18N> localizationMoq;

        public UserControllerTest()
        {
            this.userManagerMoq = IdentityMockHelpers.MockUserManager<ApplicationUser>();
            this.signInManagerMoq = IdentityMockHelpers.MockSignInManager<ApplicationUser>();
            this.roleManagerMoq = IdentityMockHelpers.MockRoleManager<ApplicationRole>();
            this.configurationMoq = new Mock<IConfiguration>();
            this.tokenHelperMoq = new Mock<ITokenHelper>();
            this.urlMoq = new Mock<IUrlHelper>();
            this.localizationMoq = new Mock<IKsI18N>();
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task Register_NotSucceeded_ThrowsException()
        {
            UserPostRequest model = new UserPostRequest();
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            this.userManagerMoq.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(new IdentityResultMoq(false));

            var controller = this.CreateUserController();
            await Assert.ThrowsAsync<OneFrameWebException>(() => controller.Register(model)).ConfigureAwait(false);

            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task Register_Succeeded_ReturnsOkResult()
        {
            UserPostRequest model = new UserPostRequest()
            {
                Email = "test@test.com",
                Password = "123",
            };

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var roleName = "role1";
            var role = new ApplicationRole(roleName);

            var userClaims = new List<Claim>() { new Claim("userClaim", "user claim value 1") };
            var roleClaims = new List<Claim>() { new Claim("roleClaim", "role claim value 1") };

            this.userManagerMoq.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                .ReturnsAsync(new IdentityResultMoq(true));
            this.userManagerMoq.Setup(s => s.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(userClaims);
            this.userManagerMoq.Setup(s => s.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new string[] { roleName });
            this.roleManagerMoq.Setup(s => s.Roles)
                .Returns(new List<ApplicationRole>() { role }.AsQueryable());
            this.roleManagerMoq.Setup(s => s.GetClaimsAsync(role))
                .ReturnsAsync(roleClaims);

            this.signInManagerMoq.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), true, null)).Returns(Task.CompletedTask);

            var token = Guid.NewGuid().ToString();

            this.tokenHelperMoq.Setup(s => s.BuildToken(It.IsAny<IList<Claim>>())).Returns(token);

            var controller = this.CreateUserController();
            var response = await controller.Register(model).ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);

            var okResult = (OkObjectResult)response;

            Assert.IsAssignableFrom<LoginResponse>(okResult.Value);

            var loginResponse = (LoginResponse)okResult.Value;

            Assert.Equal(loginResponse.Token, token);
            Assert.Equal(2, loginResponse.Claims.Count);
            Assert.Equal("user claim value 1", loginResponse.Claims[0].Value);
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task Login_CheckPasswordNotSucceeded_ThrowsException()
        {
            var model = new LoginRequest()
            {
                Password = "123",
            };

            var user = new ApplicationUser();

            this.userManagerMoq.Setup(s => s.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMoq.Setup(s => s.CheckPasswordAsync(user, model.Password)).ReturnsAsync(false);
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            await Assert.ThrowsAsync<OneFrameWebException>(() => controller.Login(model)).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task Login_Succeeded_ReturnsOkResult()
        {
            LoginRequest model = new LoginRequest()
            {
                Email = "test@test.com",
                Password = "123",
            };

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var roleName = Roles.Admin.ToString();
            var role = new ApplicationRole(roleName);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, Roles.Admin.ToString()),
            };

            this.userManagerMoq.Setup(s => s.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMoq.Setup(s => s.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);

            this.userManagerMoq.Setup(s => s.CreateAsync(user, model.Password))
                .ReturnsAsync(new IdentityResultMoq(true));

            this.userManagerMoq.Setup(s => s.GetClaimsAsync(user))
                .ReturnsAsync(claims);

            this.userManagerMoq.Setup(s => s.GetRolesAsync(user)).ReturnsAsync(new string[] { roleName });
            this.roleManagerMoq.Setup(s => s.Roles)
                .Returns(new List<ApplicationRole>() { role }.AsQueryable());

            this.signInManagerMoq.Setup(s => s.SignInAsync(user, true, null)).Returns(Task.CompletedTask);

            var token = Guid.NewGuid().ToString();

            this.tokenHelperMoq.Setup(s => s.BuildToken(It.IsAny<IList<Claim>>())).Returns(token);

            var controller = this.CreateUserController();
            var response = await controller.Login(model).ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);

            var okResult = (OkObjectResult)response;

            Assert.IsAssignableFrom<LoginResponse>(okResult.Value);

            var loginResponse = (LoginResponse)okResult.Value;

            Assert.Equal(loginResponse.Token, token);
            Assert.Equal(4, loginResponse.Claims.Count);
            Assert.Equal(user.Email, loginResponse.Claims[0].Value);
            Assert.Equal(user.UserName, loginResponse.Claims[1].Value);
            Assert.Equal(Roles.Admin.ToString(), loginResponse.Claims[3].Value);
        }

        [Fact]
        [Trait("Category", "ForgotPassword")]
        public async Task ForgotPassword_CheckIfResetUrlIsCorrect_ReturnsOkResult()
        {
            ForgotPasswordRequest model = new ForgotPasswordRequest()
            {
                Email = "test@test.com",
            };
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var token = Guid.NewGuid().ToString();
            this.userManagerMoq.Setup(s => s.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);
            this.userManagerMoq.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            var controller = this.CreateUserController();
            controller.Url = this.urlMoq.Object;

            var okResult = await controller.ForgotPassword(model).ConfigureAwait(false) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(token, okResult.Value);
        }

        [Fact]
        [Trait("Category", "ResetPassword")]
        public async Task ResetPassword_InvalidModel_ThrowsException()
        {
            ResetPasswordRequest model = new ResetPasswordRequest();
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();
            var errorMessage = "Invalid Model";

            controller.ModelState.AddModelError("invalid", errorMessage);

            await Assert.ThrowsAsync<OneFrameWebException>(() => controller.ResetPassword(model)).ConfigureAwait(false);

            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());
        }

        [Fact]
        [Trait("Category", "ResetPassword")]
        public async Task Reset_Succeeded_ReturnsOkResult()
        {
            ResetPasswordRequest model = new ResetPasswordRequest()
            {
                Email = "test@test.com",
                Password = "abcdef@1",
                Token = Guid.NewGuid().ToString(),
            };

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            this.userManagerMoq.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            this.userManagerMoq.Setup(s => s.ResetPasswordAsync(user, model.Token, model.Password)).ReturnsAsync(new IdentityResultMoq(true));
            this.userManagerMoq.Setup(s => s.IsLockedOutAsync(user)).ReturnsAsync(true);
            this.userManagerMoq.Setup(s => s.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow)).ReturnsAsync(new IdentityResultMoq(true));
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            var response = await controller.ResetPassword(model).ConfigureAwait(false);
            var okResult = (OkObjectResult)response;

            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task GetClaimsInUser_NonExistingUser_ReturnsNotFoundObjectResult()
        {
            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            var response = await Record.ExceptionAsync(() => controller.GetClaimsInUser(null)).ConfigureAwait(false);
            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());

            var ksException = response as OneFrameWebException;

            Assert.Equal(expected: ksException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.NotFound).ToString());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task GetClaimsInUser_UserHasNoClaims_ReturnsEmptyList()
        {
            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            this.userManagerMoq.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync((List<Claim>)null);

            var controller = this.CreateUserController();
            var response = await controller.GetClaimsInUser("testuser").ConfigureAwait(false);

            Assert.IsAssignableFrom<OkObjectResult>(response);
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async void AddClaimToUser_ReturnsOkResult()
        {
            var user = new ApplicationUser();

            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            this.userManagerMoq.Setup(r => r.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = this.CreateUserController();

            var response = await controller.AddClaimToUser("menu", new UserClaimPostRequest() { Name = "user" }).ConfigureAwait(false);

            Assert.IsAssignableFrom<OkResult>(response);
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async void AddClaimToUser_ReturnsNotFoundResult()
        {
            var user = new ApplicationUser();

            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            this.userManagerMoq.Setup(r => r.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            var response = await Record.ExceptionAsync(() => controller.AddClaimToUser("menu", new UserClaimPostRequest() { Name = "user" })).ConfigureAwait(false);

            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());

            var ksException = response as OneFrameWebException;

            Assert.Equal(expected: ksException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.NotFound).ToString());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async void AddClaimToUser_EmptyClaimName_ReturnsBadRequestObjectResult()
        {
            var user = new ApplicationUser();

            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            this.userManagerMoq.Setup(r => r.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var model = new UserClaimPostRequest() { Name = string.Empty };

            var controller = this.CreateUserController();

            controller.BindViewModel(model);

            var ex = await Record.ExceptionAsync(() => controller.AddClaimToUser("menu", model)).ConfigureAwait(false);

            Assert.IsAssignableFrom<OneFrameWebException>(ex);

            var ksException = ex as OneFrameWebException;

            Assert.Equal(expected: ksException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.BadRequest).ToString());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task RemoveClaimFromUser_NonExistingUser_ReturnsNotFoundObjectResult()
        {
            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            var response = await Record.ExceptionAsync(() => controller.RemoveClaimFromUser("testuser", "testclaim")).ConfigureAwait(false);
            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());

            var ksException = response as OneFrameWebException;

            Assert.Equal(expected: ksException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.NotFound).ToString());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task RemoveClaimFromRole_NonExistingClaim_ReturnsNotFoundObjectResult()
        {
            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            this.userManagerMoq.Setup(r => r.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim>());
            this.localizationMoq.SetupGet(x => x.GetLocalizer<UserController>()[It.IsAny<string>()]).Returns(new LocalizedString("sometext", "sometext"));

            var controller = this.CreateUserController();

            var response = await Record.ExceptionAsync(() => controller.RemoveClaimFromUser("testuser", "testclaim")).ConfigureAwait(false);
            this.localizationMoq.Verify(x => x.GetLocalizer<UserController>()[Moq.It.IsAny<string>()], Times.Once());

            var ksException = response as OneFrameWebException;

            Assert.Equal(expected: ksException.HttpStatusCode.ToString(), actual: Convert.ToInt32(HttpStatusCode.NotFound).ToString());
        }

        [Fact]
        [Trait("Category", "UserController")]
        public async Task RemoveClaimFromRole_AddToRoleIsSuccessful_ReturnsNoContentResult()
        {
            this.userManagerMoq.Setup(r => r.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            var claimList = new List<Claim> { new Claim("testclaimType", "testclaimValue") };
            this.userManagerMoq.Setup(r => r.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claimList);

            this.userManagerMoq.Setup(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = this.CreateUserController();

            var response = await controller.RemoveClaimFromUser("testuser", "testclaimValue").ConfigureAwait(false);
            var okResult = (OkObjectResult)response;

            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        private UserController CreateUserController()
        {
            var mapper = new KsMapster();
            var controller = new UserController(this.signInManagerMoq.Object, null, this.configurationMoq.Object, this.userManagerMoq.Object, this.roleManagerMoq.Object, this.tokenHelperMoq.Object, this.localizationMoq.Object, mapper);
            return controller;
        }
    }
}
