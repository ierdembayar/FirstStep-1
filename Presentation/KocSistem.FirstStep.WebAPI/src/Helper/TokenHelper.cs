// -----------------------------------------------------------------------
// <copyright file="TokenHelper.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.ErrorHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KocSistem.FirstStep.WebAPI
{
    public class TokenHelper : ITokenHelper
    {
        private readonly SymmetricSecurityKey symmetricSecurityKey;
        private readonly string issuer;
        private readonly IConfiguration configuration;

        public TokenHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Identity:Jwt:Key"]));
            this.issuer = this.configuration["Identity:Jwt:Issuer"];
        }

        public string BuildToken(IList<Claim> claims)
        {
            int expireInMinutes;
            if (!int.TryParse(this.configuration["Identity:Jwt:ExpireInMinutes"], out expireInMinutes))
            {
                throw new OneFrameWebException("Invalid ExpireInMinutes in web.config");
            }

            var credentials = new SigningCredentials(this.symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                this.issuer,
                this.issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
