// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Domain;
using KocSistem.FirstStep.WebAPI.Model;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.DesignObjects;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using KocSistem.OneFrame.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.WebAPI
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ITokenHelper tokenHelper;
        private readonly IKsI18N i18n;
        private readonly IKsStringLocalizer<UserController> localize;
        private readonly IMapper mapper;

        public UserController(
            SignInManager<ApplicationUser> signInManager,
            ILogger<UserController> logger,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ITokenHelper tokenHelper,
            IKsI18N i18n,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenHelper = tokenHelper;
            this.i18n = i18n;
            this.localize = this.i18n.GetLocalizer<UserController>();
            this.mapper = mapper;
        }

        /// <summary>
        /// User registration.
        /// </summary>
        /// <param name="model">RegisterModel.</param>
        /// <returns>Return LoginResponseModel. Returns http status codes(200,400,500).</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerRequestExample(typeof(UserPostRequest), typeof(UserPostRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(LoginResponseExample))]
        [SwaggerResponse(200, type: typeof(LoginResponse), description: "Return LoginResponseModel")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Register([FromBody] UserPostRequest model)
        {
            var createdUser = await this.CreateUser(model).ConfigureAwait(false);

            await this.signInManager.SignInAsync(createdUser, true).ConfigureAwait(false);

            IList<Claim> claims = await this.userManager.GetClaimsAsync(createdUser).ConfigureAwait(false);
            var roleNames = await this.userManager.GetRolesAsync(createdUser).ConfigureAwait(false);
            var roles = this.roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToArray();
            IList<Claim> roleClaims;
            foreach (var role in roles)
            {
                roleClaims = await this.roleManager.GetClaimsAsync(role).ConfigureAwait(false);

                foreach (var roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }

            var token = this.tokenHelper.BuildToken(claims);

            return this.Ok(new LoginResponse(token, claims));
        }

        /// <summary>
        /// Login user.
        /// </summary>
        /// <param name="model">LoginModel.</param>
        /// <returns>Return LoginResponseModel. Returns http status codes(200,400,500).</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(LoginResponseExample))]
        [SwaggerResponse(200, type: typeof(LoginResponse), description: "Return LoginResponseModel")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Login([FromBody]LoginRequest model)
        {
            var user = await this.userManager.FindByNameAsync(model.Email).ConfigureAwait(false);

            if (user == null || !await this.userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["InvalidLoginCredentials"]);
            }

            await this.signInManager.SignInAsync(user, true).ConfigureAwait(false);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = this.userManager.GetRolesAsync(user).Result;

            if (userRoles.Any())
            {
                foreach (var roleName in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));

                    var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

                    var userRoleClaims = await this.roleManager.GetClaimsAsync(role).ConfigureAwait(false);

                    if (userRoleClaims != null)
                    {
                        foreach (var claim in userRoleClaims)
                        {
                            claims.Add(new Claim(claim.Type, claim.Value));
                        }
                    }
                }
            }

            var token = this.tokenHelper.BuildToken(claims);

            return this.Ok(new LoginResponse(token, claims));
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="model">ForgotPasswordModel.</param>
        /// <returns>Return Return resetUrl. Returns http status codes(200,400,500).</returns>
        [HttpPost("forgotpassword")]
        [SwaggerRequestExample(typeof(ForgotPasswordRequest), typeof(ForgotPasswordRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(LoginResponseExample))]
        [SwaggerResponse(200, type: typeof(string), description: "Return resetUrl")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordRequest model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user != null)
            {
                var token = await this.userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                return this.Ok(token);
            }
            else
            {
                // email user and inform them that they do not have an account
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["AccountDoesNotExist", model.Email]);
            }
        }

        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="model">ResetPasswordModel.</param>
        /// <returns>Return  this.localize["PasswordResetSuccessful"].</returns>
        [HttpPost("resetpassword")]
        [SwaggerRequestExample(typeof(ResetPasswordRequest), typeof(ResetPasswordRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(ResetPasswordResponseExample))]
        [SwaggerResponse(200, type: typeof(string), description: "Return resetUrl")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["UserNotFound"]);
            }

            var result = await this.userManager.ResetPasswordAsync(user, model.Token, model.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var error = new ErrorSummary(this.localize["UnableToResetPassword"]);
                foreach (var er in result.Errors)
                {
                    error.Items.Add(new ErrorItem(this.localize[er.Code]));
                }

                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, error);
            }

            await this.UnlockUser(user).ConfigureAwait(false);

            return this.Ok(this.localize["PasswordResetSuccessful"]);
        }

        /// <summary>
        /// Add claim to user.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="model">UserClaimPostModel.</param>
        /// <returns>Return Return resetUrl. Returns http status codes(200,400,404,500).</returns>
        [HttpPost("{username}/claims")]
        [RoleAuthorize(Roles.Admin, Roles.Guest)]
        [SwaggerResponse(200, type: typeof(UserClaimPostRequest), description: "Claim added to user successfully")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> AddClaimToUser(string username, [FromBody] UserClaimPostRequest model)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, "Model state is invalid.");
            }

            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["UserNotFound"]);
            }

            var result = await this.userManager.AddClaimAsync(user, new Claim("userClaim", model.Name)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.Ok();
        }

        /// <summary>
        /// Get claim(s) in user.
        /// </summary>
        /// <param name="username">User namme.</param>
        /// <returns>Returns ClaimResponseModel item. Returns http status codes(200,404,500).</returns>
        [HttpGet("{username}/claims")]
        [RoleAuthorize(Roles.Admin, Roles.Guest)]
        [SwaggerResponseExample(200, typeof(ClaimListResponseExample))]
        [SwaggerResponse(200, type: typeof(string), description: "Claim(s) successfully found")]
        [SwaggerResponse(404, type: typeof(void), description: "Claim(s) was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> GetClaimsInUser(string username)
        {
            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["UserNotFound"]);
            }

            var claims = await this.userManager.GetClaimsAsync(user).ConfigureAwait(false);

            var response = new List<ClaimResponse>();

            if (claims != null && claims.Count > 0)
            {
                foreach (var claim in claims)
                {
                    response.Add(claim.MapToClaimResponseModel());
                }
            }

            return this.Ok(response);
        }

        /// <summary>
        /// Remove claim from user.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="claimvalue">claim value.</param>
        /// <returns>Returns http status codes(204,404,500).</returns>
        [HttpDelete("{username}/claims/{claimvalue}")]
        [RoleAuthorize(Roles.Admin, Roles.Guest)]
        [SwaggerResponse(204, type: typeof(void), description: "Claim successfully removed from user.")]
        [SwaggerResponse(404, type: typeof(void), description: "Claim(s) or User was not found.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> RemoveClaimFromUser(string username, string claimvalue)
        {
            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["UserNotFound"]);
            }

            var claims = await this.userManager.GetClaimsAsync(user).ConfigureAwait(false);

            if (claims == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["ClaimNotFound"]);
            }

            if (claims.Count(c => c.Value == claimvalue) == 0)
            {
                throw new OneFrameWebException((int)HttpStatusCode.NotFound, this.localize["ClaimNotFound"]);
            }

            var result = await this.userManager.RemoveClaimAsync(user, claims.First(c => c.Value == claimvalue)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.Ok(result.Succeeded);
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="user">user Model.</param>
        /// <returns>Returns http status codes(200,404,500).</returns>
        [HttpPost]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerRequestExample(typeof(UserPostRequest), typeof(UserPostRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(ApplicationUserResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationUser), description: "User is created successfully.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        public async Task<IActionResult> Post([FromBody] UserPostRequest user)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["InvalidModel"]);
            }

            var createdUser = await this.CreateUser(user).ConfigureAwait(false);

            return this.CreatedAtRoute("UserGet", new { username = createdUser.UserName }, new { id = createdUser.Id, username = createdUser.UserName });
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <returns>Returns http status codes(204,404,500).</returns>
        [HttpDelete("{username}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(204, type: typeof(void), description: "User is successfully deleted.")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user == null)
            {
                return this.NotFound();
            }

            var result = await this.userManager.DeleteAsync(user).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.NoContent();
        }

        /// <summary>
        /// Update application role partially.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="user">User Model.</param>
        /// <returns>Returns ApplicationUser item.Returns http status codes(200,404,500).</returns>
        [HttpPatch("{username}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ApplicationUserResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationUser), description: "User successfully updated")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Patch(string username, [FromBody] UserPatchRequest user)
        {
            var userToUpdate = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (userToUpdate == null)
            {
                return this.NotFound();
            }

            userToUpdate.Email = user.Email;
            userToUpdate.PhoneNumber = user.PhoneNumber;

            var result = await this.userManager.UpdateAsync(userToUpdate).ConfigureAwait(false);

            return this.Ok(userToUpdate);
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="pagedRequest">Request object for fetching users with paging properties.</param>
        /// <returns>Returns found ApplicationUser list. Returns http status codes(200,404,500).</returns>
        [HttpGet]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(200, type: typeof(IList<ApplicationUser>), description: "Return all users.")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(404, type: typeof(void), description: "Users not found.")]
        [SwaggerResponse(500, "An error occurred while processing your request.")]
        public async Task<IActionResult> Get([FromQuery]PagedRequest pagedRequest)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["InvalidModel"]);
            }

            var users = await this.userManager.Users.OrderBy(u => u.UserName)
                                                    .ToPagedListAsync(pagedRequest.PageIndex, pagedRequest.PageSize)
                                                    .ConfigureAwait(false);

            var userGetResponse = this.mapper.Map<IPagedList<ApplicationUser>, PagedResult<UserGetResponse>>(users);
            return this.Ok(userGetResponse);
        }

        /// <summary>
        /// Get a user by username.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>Returns found ApplicationUser list. Returns http status codes(200,404,500).</returns>
        [HttpGet("{username}", Name = "UserGet")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ApplicationUserResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationUser), description: "User is found.")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Get(string username)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["InvalidModel"]);
            }

            var applicationUser = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (applicationUser == null)
            {
                return this.NotFound();
            }

            var userGetResponse = this.mapper.Map<ApplicationUser, UserGetResponse>(applicationUser);
            return this.Ok(userGetResponse);
        }

        /// <summary>
        /// Search a user by username.
        /// </summary>
        /// <param name="userGetRequest">Request object for fetching users with paging properties.</param>
        /// <returns>Returns found ApplicationUser list. Returns http status codes(200,404,500).</returns>
        [HttpGet("search")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ApplicationUserResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationUser), description: "User is found.")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Search([FromQuery] UserSearchRequest userGetRequest)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["InvalidModel"]);
            }

            var users = await this.userManager.Users.Where(u => u.UserName.StartsWith(userGetRequest.Username, StringComparison.InvariantCulture))
                                                    .ToPagedListAsync(userGetRequest.PageIndex, userGetRequest.PageSize).ConfigureAwait(false);

            var userGetResponse = this.mapper.Map<IPagedList<ApplicationUser>, PagedResult<UserGetResponse>>(users);
            return this.Ok(userGetResponse);
        }

        private async Task UnlockUser(ApplicationUser user)
        {
            if (await this.userManager.IsLockedOutAsync(user).ConfigureAwait(false))
            {
                await this.userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow).ConfigureAwait(false);
            }
        }

        private async Task<ApplicationUser> CreateUser(UserPostRequest userPostRequest)
        {
            var user = new ApplicationUser() { UserName = userPostRequest.Email, Email = userPostRequest.Email, PhoneNumber = userPostRequest.PhoneNumber };
            var result = await this.userManager.CreateAsync(user, userPostRequest.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errorSummary = new ErrorSummary(this.localize["UnableToCreateUser"]);
                foreach (var er in result.Errors)
                {
                    errorSummary.Items.Add(new ErrorItem(this.localize[er.Code]));
                }

                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, errorSummary);
            }

            return user;
        }
    }
}