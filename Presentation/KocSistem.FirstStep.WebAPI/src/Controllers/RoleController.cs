// <copyright file="RoleController.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Domain;
using KocSistem.FirstStep.WebAPI.Model;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.I18N;
using KocSistem.OneFrame.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.WebAPI
{
    [Route("roles")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IKsI18N i18n;
        private readonly IKsStringLocalizer<RoleController> localize;
        private readonly IMapper mapper;

        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IKsI18N i18n, IMapper mapper)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.i18n = i18n;
            this.localize = this.i18n.GetLocalizer<RoleController>();
            this.mapper = mapper;
        }

        /// <summary>
        /// Get application roles.
        /// </summary>
        /// <returns>Return list of  ApplicationRole items. Returns http status codes(200,404,500).</returns>
        [HttpGet]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(200, type: typeof(IList<ApplicationRole>), description: "Return application roles")]
        [SwaggerResponse(404, type: typeof(void), description: "Role(s) was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public IActionResult Get()
        {
            var roles = this.roleManager.Roles.ToList();

            if (roles == null)
            {
                return this.NotFound();
            }

            return this.Ok(roles);
        }

        /// <summary>
        /// Get application roles.
        /// </summary>
        /// <returns>Return list of  ApplicationRole items. Returns http status codes(200,404,500).</returns>
        [HttpGet("GetList")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(200, type: typeof(IList<ApplicationRole>), description: "Return application roles")]
        [SwaggerResponse(404, type: typeof(void), description: "Role(s) was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Get([FromQuery]PagedRequest pagedRequest)
        {
            if (!this.ModelState.IsValid)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, "Model state is invalid.");
            }

            var roles = await this.roleManager.Roles.OrderBy(r => r.Name)
                                                    .ToPagedListAsync(pagedRequest.PageIndex, pagedRequest.PageSize)
                                                    .ConfigureAwait(false);

            if (roles == null || roles.TotalCount == 0)
            {
                return this.NotFound();
            }

            var roleGetResponse = this.mapper.Map<IPagedList<ApplicationRole>, PagedResult<RoleGetResponse>>(roles);
            return this.Ok(roleGetResponse);
        }

        /// <summary>
        /// Get application role by role name.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>Returns ApplicationRole item. Returns http status codes(200,404,500).</returns>
        [HttpGet("{roleName}", Name = "roleget")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ApplicationRoleResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationRole), description: "Role successfully found")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Get(string roleName)
        {
            var roles = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (roles == null)
            {
                return this.NotFound();
            }

            return this.Ok(roles);
        }

        /// <summary>
        /// Create a application role.
        /// </summary>
        /// <param name="role">Role Model.</param>
        /// <returns>Returns http status codes(200,404,500).</returns>
        [HttpPost]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerRequestExample(typeof(RolePostRequest), typeof(RolePostRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(200, typeof(ApplicationRoleResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationRole), description: "Role successfully created")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        public async Task<IActionResult> Post([FromBody] RolePostRequest role)
        {
            var result = await this.roleManager.CreateAsync(this.MapToApplicationRole(role)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.CreatedAtRoute("RoleGet", new { roleName = role.Name }, role);
        }

        /// <summary>
        /// Update application role partially.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <param name="role">Role Model.</param>
        /// <returns>Returns ApplicationRole item.Returns http status codes(200,404,500).</returns>
        [HttpPatch("{roleName}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ApplicationRoleResponseExample))]
        [SwaggerResponse(200, type: typeof(ApplicationRole), description: "Role successfully updated")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Patch(string roleName, [FromBody] RolePatchRequest role)
        {
            var roleToUpdate = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (roleToUpdate == null)
            {
                return this.NotFound();
            }

            roleToUpdate.Description = role.Description;

            var result = await this.roleManager.UpdateAsync(roleToUpdate).ConfigureAwait(false);

            return this.Ok(roleToUpdate);
        }

        /// <summary>
        /// Delete application role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>Returns http status codes(204,404,500).</returns>
        [HttpDelete("{roleName}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(204, type: typeof(void), description: "Role successfully deleted")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> Delete(string roleName)
        {
            var roleToDelete = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (roleToDelete == null)
            {
                return this.NotFound();
            }

            var result = await this.roleManager.DeleteAsync(roleToDelete).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.NoContent();
        }

        /// <summary>
        /// Get users by role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>Returns RoleUserResponseModel.Returns http status codes(200,404,500).</returns>
        [HttpGet("{roleName}/users")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(RoleUserResponseExample))]
        [SwaggerResponse(200, type: typeof(RoleUserResponse), description: "Return user")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var users = await this.userManager.GetUsersInRoleAsync(roleName).ConfigureAwait(false);

            var response = new List<RoleUserResponse>();

            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    response.Add(this.MapToRoleUserResponseModel(user));
                }
            }

            return this.Ok(response);
        }

        /// <summary>
        /// Add user to role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <param name="username">User name.</param>
        /// <returns>Returns http status codes(200,404,500).</returns>
        [HttpPost("{roleName}/users/{username}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(200, type: typeof(void), description: "User successfully added to role")]
        [SwaggerResponse(404, type: typeof(void), description: "User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> AddUserToRole(string roleName, string username)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user == null)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, this.localize["UserNotFound"]);
            }

            var result = await this.userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.Ok();
        }

        /// <summary>
        /// Remove user from role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <param name="username">User name.</param>
        /// <returns>Returns http status codes(204,404,500).</returns>
        [HttpDelete("{roleName}/users/{username}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(204, type: typeof(void), description: "User successfully deleted from role")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found or User was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> RemoveUserFromRole(string roleName, string username)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var user = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);
            if (user == null)
            {
                return this.NotFound(this.localize["UserNotFound"]);
            }

            var result = await this.userManager.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.NoContent();
        }

        /// <summary>
        /// Add claim to role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <param name="model">RoleClaimPostModel.</param>
        /// <returns>Returns http status codes(200,404,500).</returns>
        [HttpPost("{roleName}/claims")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(200, type: typeof(void), description: "Claims added successfully")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found")]
        [SwaggerResponse(400, type: typeof(void), description: "Model state is invalid.")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> AddClaimToRole(string roleName, [FromBody] RoleClaimPostRequest model)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var result = await this.roleManager.AddClaimAsync(role, new Claim("roleClaim", model.Name)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.Ok();
        }

        /// <summary>
        /// Get claims in role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>Returns ClaimResponseModel items. Returns http status codes(200,404,500).</returns>
        [HttpGet("{roleName}/claims")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponseExample(200, typeof(ClaimResponseExample))]
        [SwaggerResponse(200, type: typeof(ClaimResponse), description: "Return claim(s)")]
        [SwaggerResponse(404, type: typeof(void), description: "role was not found")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> GetClaimsInRole(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var claims = await this.roleManager.GetClaimsAsync(role).ConfigureAwait(false);

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
        /// Remove claim from role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <param name="claimvalue">Claim value.</param>
        /// <returns>Returns http status codes(204,404,500).</returns>
        [HttpDelete("{roleName}/claims/{claimvalue}")]
        [RoleAuthorize(Roles.Admin)]
        [SwaggerResponse(204, type: typeof(void), description: "Claim successfully deleted from role")]
        [SwaggerResponse(404, type: typeof(void), description: "Role was not found or Claims was not ")]
        [SwaggerResponse(500, "An error occurred while processing your request")]
        public async Task<IActionResult> RemoveClaimFromRole(string roleName, string claimvalue)
        {
            var role = await this.roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role == null)
            {
                return this.NotFound(this.localize["RoleNotFound", roleName]);
            }

            var claims = await this.roleManager.GetClaimsAsync(role).ConfigureAwait(false);

            if (claims == null)
            {
                return this.NotFound(this.localize["ClaimNotFound"]);
            }

            if (claims.Count(c => c.Value == claimvalue) == 0)
            {
                return this.NotFound(this.localize["ClaimNotFound"]);
            }

            var result = await this.roleManager.RemoveClaimAsync(role, claims.First(c => c.Value == claimvalue)).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new OneFrameWebException((int)HttpStatusCode.BadRequest, result.ToString());
            }

            return this.NoContent();
        }

        [NonAction]
        private ApplicationRole MapToApplicationRole(RolePostRequest role)
        {
            return new ApplicationRole()
            {
                Name = role.Name,
                Description = role.Description,
            };
        }

        [NonAction]
        private RoleUserResponse MapToRoleUserResponseModel(ApplicationUser user)
        {
            return new RoleUserResponse()
            {
                Username = user.UserName,
                Email = user.Email,
            };
        }
    }
}