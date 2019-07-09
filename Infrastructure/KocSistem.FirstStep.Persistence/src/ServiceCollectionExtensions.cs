// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.Data.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KocSistem.FirstStep.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDatabase(this DbContextOptionsBuilder builder, string connectionString, string migrationAssembly)
        {
            builder
                    .UseSqlServer(
                       connectionString,
                       b => b.MigrationsAssembly(migrationAssembly));
        }

        public static void AddPersistenceInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddRelationalData<MainDbContext>(
                (options) =>
                {
                    options.ConfigureDatabase(
                        configuration["Data:MainDbContext:ConnectionString"],
                        configuration["Data:MainDbContext:MigrationsAssembly"]);
                });
        }
    }
}
