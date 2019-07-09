// -----------------------------------------------------------------------
// <copyright file="IdentityResultMoq.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;

namespace KocSistem.FirstStep.Tests
{
    public class IdentityResultMoq : IdentityResult
    {
        public IdentityResultMoq(bool succeded = false)
        {
            this.Succeeded = succeded;
        }
    }
}
