// -----------------------------------------------------------------------
// <copyright file="MenuMappings.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KocSistem.FirstStep.Persistence
{
    public static class MenuMappings
    {
        public static void OnModelCreating(EntityTypeBuilder<Menu> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(m => m.ParentMenu)
                .WithMany(m => m.ChildMenu)
                .HasForeignKey(f => f.ParentId);
        }
    }
}
