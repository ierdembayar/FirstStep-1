// <copyright file="DesignTimeMainDbContextFactory.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace KocSistem.FirstStep.WebAPI
{
    public class DesignTimeMainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            var builder = new DbContextOptionsBuilder<MainDbContext>();

            builder.ConfigureDatabase(
                configuration["Data:MainDbContext:ConnectionString"],
                configuration["Data:MainDbContext:MigrationsAssembly"]);

            return new MainDbContext(builder.Options);
        }
    }
}
