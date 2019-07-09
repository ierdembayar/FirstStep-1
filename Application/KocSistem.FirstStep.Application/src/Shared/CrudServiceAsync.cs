// -----------------------------------------------------------------------
// <copyright file="CrudServiceAsync.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.DesignObjects;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.Mapper;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.Application
{
    public abstract class CrudServiceAsync<TEntity, TDto>
     : CrudServiceAsync<TEntity, TDto, int>
         where TEntity : class, IEntityHasId<int>, new()
         where TDto : class, IDtoHasId<int>, new()
    {
        protected CrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceAsync<TEntity, TDto, TPrimaryKey>
        : CrudServiceAsync<TEntity, TDto, TPrimaryKey, TDto, TDto>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected CrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput>
        : CrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
            where TCreateInput : class, IDto, new()
    {
        protected CrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
       : CrudServiceBase<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>,
        ICrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
            where TCreateInput : class, IDto, new()
            where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected CrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }

        public virtual async Task<IServiceResult<TDto>> GetById(TPrimaryKey id)
        {
            var entity = await repository.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(id)).ConfigureAwait(false);

            return new ServiceResult<TDto>(this.mapper.Map<TDto>(entity));
        }

        public virtual async Task<IServiceResult<IEnumerable<TDto>>> GetList(Expression<Func<TEntity, bool>> predicate = null,
                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                   Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            var entities = this.repository.GetList(
                                                   predicate: predicate,
                                                   orderBy: orderBy,
                                                   include: include);

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual async Task<IServiceResult<PagedDtoList<TDto>>> GetPagedList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20)
        {
            var entities = this.repository.GetPagedList(
                                                        predicate: predicate,
                                                        orderBy: orderBy,
                                                        include: include,
                                                        pageIndex: pageIndex,
                                                        pageSize: pageSize);

            return new ServiceResult<PagedDtoList<TDto>>(this.mapper.Map<PagedDtoList<TDto>>(entities));
        }

        public virtual async Task<IServiceResult<TDto>> Create(TCreateInput model)
        {
            var entity = mapper.Map<TEntity>(model);

            await repository.AddAsync(entity).ConfigureAwait(false);

            return new ServiceResult<TDto>(this.mapper.Map<TDto>(entity));
        }

        public virtual async Task<IServiceResult<IEnumerable<TDto>>> CreateRange(IEnumerable<TCreateInput> model)
        {
            var entities = mapper.Map<List<TEntity>>(model);

            await repository.AddRangeAsync(entities).ConfigureAwait(false);

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual async Task<IServiceResult<TDto>> Update(TUpdateInput model)
        {
            var entity = mapper.Map<TEntity>(model);

            await repository.UpdateAsync(entity).ConfigureAwait(false);

            return new ServiceResult<TDto>(this.mapper.Map<TDto>(entity));
        }

        public virtual async Task<IServiceResult<IEnumerable<TDto>>> UpdateRange(IEnumerable<TUpdateInput> model)
        {
            var entities = mapper.Map<List<TEntity>>(model);

            await repository.UpdateRangeAsync(entities).ConfigureAwait(false);

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual async Task<IServiceResult<bool>> Delete(TPrimaryKey id)
        {
            try
            {
                var entity = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id)).ConfigureAwait(false);

                if (entity == null)
                {
                    throw new ArgumentNullException("entity", "Entity Not Found");
                }

                await repository.DeleteAsync(entity).ConfigureAwait(false);

                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(new ErrorSummary(ex.Message));
            }
        }

        public virtual async Task<IServiceResult<bool>> DeleteRange(IEnumerable<TPrimaryKey> ids)
        {
            try
            {
                var entities = await repository.GetListAsync(predicate: x => ids.Contains(x.Id)).ConfigureAwait(false);

                if (!entities.Any())
                {
                    throw new ArgumentNullException("entities", "Entities Not Found");
                }

                await repository.DeleteRangeAsync(entities).ConfigureAwait(false);

                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(new ErrorSummary(ex.Message));
            }
        }
    }
}