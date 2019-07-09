// -----------------------------------------------------------------------
// <copyright file="IApplicationCrudService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using KocSistem.OneFrame.Data.Relational;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface IApplicationCrudService<TEntity, TDto>
        : ICrudService<TEntity, TDto, int>
            where TEntity : class, IEntityHasId<int>, new()
            where TDto : class, IDtoHasId<int>, new()
    {
    }

    public interface IApplicationCrudService<TEntity, TDto, TPrimaryKey>
        : ICrudService<TEntity, TDto, TPrimaryKey>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
    }

    public interface IApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput>
        : ICrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
            where TCreateInput : class, IDto, new()
    {
    }

    public interface IApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
    : ICrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
    }
}
