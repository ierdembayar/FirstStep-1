// -----------------------------------------------------------------------
// <copyright file="ResetPasswordRequestExample.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KocSistem.FirstStep.WebAPI
{
    internal class ResetPasswordRequestExample : IExamplesProvider
    {
        private readonly ITokenHelper tokenHelper;

        public ResetPasswordRequestExample(ITokenHelper tokenHelper)
        {
            this.tokenHelper = tokenHelper;
        }

        public object GetExamples()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "ghostbusters@kocsistem.com.tr"),
                new Claim(JwtRegisteredClaimNames.UniqueName, "oneframe"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
            var token = this.tokenHelper.BuildToken(claims);

            var password = Guid.NewGuid().ToString().Remove(5);

            return new ResetPasswordRequest
            {
                Token = token,
                Email = "ghostbusters@kocsistem.com.tr",
                Password = password,
                ConfirmPassword = password,
            };
        }
    }
}