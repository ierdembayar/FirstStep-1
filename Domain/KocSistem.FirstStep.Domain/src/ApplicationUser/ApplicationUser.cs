// -----------------------------------------------------------------------
// <copyright file="ApplicationUser.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.Data.Relational;
using Microsoft.AspNetCore.Identity;

namespace KocSistem.FirstStep.Domain
{
    public class ApplicationUser : IdentityUser, IEntity
    {
    }
}
