// -----------------------------------------------------------------------
// <copyright file="MainDbContext.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KocSistem.FirstStep.Persistence
{
    public class MainDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        // constructors

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        // properties

        // methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Menu>(MenuMappings.OnModelCreating);
            builder.Entity<ApplicationUser>(ApplicationUserMappings.OnModelCreating);
            builder.Entity<ApplicationRole>().ToTable("Role");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
        }
    }
}
