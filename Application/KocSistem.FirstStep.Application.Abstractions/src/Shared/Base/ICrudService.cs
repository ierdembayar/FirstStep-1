// -----------------------------------------------------------------------
// <copyright file="ICrudService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using KocSistem.OneFrame.DesignObjects;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface ICrudService<TEntity, TDto>
      : ICrudService<TEntity, TDto, int>
      where TDto : class, IDtoHasId<int>, new()
    {
    }

    public interface ICrudService<TEntity, TDto, TPrimaryKey>
        : ICrudService<TEntity, TDto, TPrimaryKey, TDto>
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
    }

    public interface ICrudService<TEntity, TDto, TPrimaryKey, in TCreateInput>
        : ICrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
    {
    }

    public interface ICrudService<TEntity, TDto, TPrimaryKey, in TCreateInput, in TUpdateInput>
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        IServiceResult<TDto> GetById(TPrimaryKey id);

        IServiceResult<IEnumerable<TDto>> GetList(
                                                Expression<Func<TEntity, bool>> predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

        IServiceResult<PagedDtoList<TDto>> GetPagedList(
                                                        Expression<Func<TEntity, bool>> predicate = null,
                                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                        int pageIndex = 0,
                                                        int pageSize = 20);

        IServiceResult<TDto> Create(TCreateInput model);

        IServiceResult<IEnumerable<TDto>> CreateRange(IEnumerable<TCreateInput> model);

        IServiceResult<IEnumerable<TDto>> CreateRangeAsync(IEnumerable<TCreateInput> model);

        IServiceResult<TDto> Update(TUpdateInput model, bool checkExist = true);

        IServiceResult<IEnumerable<TDto>> UpdateRange(IEnumerable<TUpdateInput> model);

        IServiceResult<bool> Delete(TPrimaryKey id);

        IServiceResult<bool> DeleteRange(IEnumerable<TPrimaryKey> ids);
    }
}