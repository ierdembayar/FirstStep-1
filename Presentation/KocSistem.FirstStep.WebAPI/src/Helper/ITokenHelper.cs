// -----------------------------------------------------------------------
// <copyright file="ITokenHelper.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;

namespace KocSistem.FirstStep.WebAPI
{
    public interface ITokenHelper
    {
        string BuildToken(IList<Claim> claims);
    }
}
