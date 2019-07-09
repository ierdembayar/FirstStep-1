// -----------------------------------------------------------------------
// <copyright file="LoginResponse.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;

namespace KocSistem.FirstStep.WebAPI
{
    public class LoginResponse
    {
        public LoginResponse(string token, IList<Claim> claims)
        {
            this.Token = token;
            this.Claims = claims;
        }

        public string Token { get; }

        public IList<Claim> Claims { get; }
    }
}
