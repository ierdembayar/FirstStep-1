// -----------------------------------------------------------------------
// <copyright file="ApplicationUserMappings.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KocSistem.FirstStep.Persistence
{
    public static class ApplicationUserMappings
    {
        public static void OnModelCreating(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("User");
        }
    }
}
