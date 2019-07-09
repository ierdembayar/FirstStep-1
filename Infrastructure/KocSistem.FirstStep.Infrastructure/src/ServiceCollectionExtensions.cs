// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KocSistem.FirstStep.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IMenuService, MenuService>();
        }
    }
}
