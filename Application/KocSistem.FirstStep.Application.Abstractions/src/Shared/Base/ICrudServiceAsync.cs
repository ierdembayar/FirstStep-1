// -----------------------------------------------------------------------
// <copyright file="ICrudServiceAsync.cs" company="KocSistem">
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
using System.Threading.Tasks;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface ICrudServiceAsync<TEntity, TDto>
    : ICrudServiceAsync<TEntity, TDto, int>
      where TDto : class, IDtoHasId<int>, new()
    {
    }

    public interface ICrudServiceAsync<TEntity, TDto, TPrimaryKey>
        : ICrudServiceAsync<TEntity, TDto, TPrimaryKey, TDto>
         where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
    }

    public interface ICrudServiceAsync<TEntity, TDto, TPrimaryKey, in TCreateInput>
        : ICrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
    {
    }

    public interface ICrudServiceAsync<TEntity, TDto, TPrimaryKey, in TCreateInput, in TUpdateInput>
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        Task<IServiceResult<TDto>> GetById(TPrimaryKey id);

        Task<IServiceResult<IEnumerable<TDto>>> GetList(
                                                Expression<Func<TEntity, bool>> predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

        Task<IServiceResult<PagedDtoList<TDto>>> GetPagedList(
                                                        Expression<Func<TEntity, bool>> predicate = null,
                                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                        int pageIndex = 0,
                                                        int pageSize = 20);

        Task<IServiceResult<TDto>> Create(TCreateInput model);

        Task<IServiceResult<IEnumerable<TDto>>> CreateRange(IEnumerable<TCreateInput> model);

        Task<IServiceResult<TDto>> Update(TUpdateInput model);

        Task<IServiceResult<IEnumerable<TDto>>> UpdateRange(IEnumerable<TUpdateInput> model);

        Task<IServiceResult<bool>> Delete(TPrimaryKey id);

        Task<IServiceResult<bool>> DeleteRange(IEnumerable<TPrimaryKey> ids);
    }
}