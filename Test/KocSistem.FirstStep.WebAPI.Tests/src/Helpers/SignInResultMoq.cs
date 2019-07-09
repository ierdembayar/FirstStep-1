// -----------------------------------------------------------------------
// <copyright file="SignInResultMoq.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;

namespace KocSistem.FirstStep.Tests
{
    public class SignInResultMoq : SignInResult
    {
        public SignInResultMoq(bool succeded = false, bool requiresTwoFactor = false, bool isLocked = false)
        {
            this.Succeeded = succeded;
            this.RequiresTwoFactor = requiresTwoFactor;
            this.IsLockedOut = isLocked;
        }
    }
}
