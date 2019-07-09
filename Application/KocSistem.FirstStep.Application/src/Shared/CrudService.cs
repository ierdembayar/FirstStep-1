// -----------------------------------------------------------------------
// <copyright file="CrudService.cs" company="KocSistem">
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

namespace KocSistem.FirstStep.Application
{
    public abstract class CrudService<TEntity, TDto>
      : CrudService<TEntity, TDto, int>
          where TEntity : class, IEntityHasId<int>, new()
          where TDto : class, IDtoHasId<int>, new()
    {
        protected CrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
           : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudService<TEntity, TDto, TPrimaryKey>
        : CrudService<TEntity, TDto, TPrimaryKey, TDto>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected CrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudService<TEntity, TDto, TPrimaryKey, TCreateInput>
        : CrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
            where TCreateInput : class, TDto, new()
    {
        protected CrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
           : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
       : CrudServiceBase<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>, ICrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
            where TCreateInput : class, IDto, new()
            where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected CrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }

        public virtual IServiceResult<TDto> GetById(TPrimaryKey id)
        {
            var entity = this.repository.GetFirstOrDefault(predicate: t => t.Id.Equals(id));
            return new ServiceResult<TDto>(this.mapper.Map<TDto>(entity));
        }

        public virtual IServiceResult<IEnumerable<TDto>> GetList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            var entities = this.repository.GetList(
                                                    predicate: predicate,
                                                    orderBy: orderBy,
                                                    include: include);

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual IServiceResult<PagedDtoList<TDto>> GetPagedList(
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

        public virtual IServiceResult<TDto> Create(TCreateInput model)
        {
            var entity = this.mapper.Map<TEntity>(model);

            try
            {
                this.repository.Add(entity);
            }
            catch (Exception ex)
            {
                return new ServiceResult<TDto>(new ErrorSummary(ex.Message));
            }

            return new ServiceResult<TDto>(this.mapper.Map<TDto>(this.repository.GetDbSetByQuery(disableTracking: false).dbSet.AsQueryable().FirstOrDefault(x => x.Id.Equals(entity.Id))));
        }

        public virtual IServiceResult<IEnumerable<TDto>> CreateRange(IEnumerable<TCreateInput> model)
        {
            var entities = this.mapper.Map<List<TEntity>>(model);

            try
            {
                this.repository.AddRange(entities);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<TDto>>(new ErrorSummary(ex.Message));
            }

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual IServiceResult<IEnumerable<TDto>> CreateRangeAsync(IEnumerable<TCreateInput> model)
        {
            var entities = this.mapper.Map<List<TEntity>>(model);
            this.repository.AddRangeAsync(entities);
            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual IServiceResult<TDto> Update(TUpdateInput model, bool checkExist = true)
        {
            TEntity entity = null;
            if (checkExist)
            {
                entity = this.repository.GetFirstOrDefault(predicate: x => x.Id.Equals(model.Id));

                if (entity == null)
                {
                    return new ServiceResult<TDto>(new ErrorSummary("Entity not found"));
                }
            }

            entity = this.mapper.Map<TUpdateInput, TEntity>(model, entity);

            try
            {
                this.repository.Update(entity);
            }
            catch (Exception ex)
            {
                return new ServiceResult<TDto>(new ErrorSummary(ex.Message));
            }

            return new ServiceResult<TDto>(this.mapper.Map<TDto>(this.repository.GetDbSetByQuery(true).dbSet.AsQueryable().FirstOrDefault(x => x.Id.Equals(entity.Id))));
        }

        public virtual IServiceResult<IEnumerable<TDto>> UpdateRange(IEnumerable<TUpdateInput> model)
        {
            var entityIds = model.Select(x => x.Id).ToList();

            var entities = this.repository.GetList(predicate: x => entityIds.Contains(x.Id)).ToList();

            var mappedEntities = new List<TEntity>();

            foreach (TEntity entity in entities)
            {
                var item = model.FirstOrDefault(x => x.Id.Equals(entity.Id));
                mappedEntities.Add(this.mapper.Map<TUpdateInput, TEntity>(item, entity));
            }

            if (mappedEntities.Count == 0)
            {
                return new ServiceResult<IEnumerable<TDto>>(new ErrorSummary("Entity not found"));
            }

            try
            {
                this.repository.UpdateRange(mappedEntities);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<TDto>>(new ErrorSummary(ex.Message));
            }

            return new ServiceResult<IEnumerable<TDto>>(this.mapper.Map<IEnumerable<TDto>>(entities));
        }

        public virtual IServiceResult<bool> Delete(TPrimaryKey id)
        {
            try
            {
                var entity = this.repository.GetFirstOrDefault(predicate: x => x.Id.Equals(id));

                this.repository.Delete(entity);

                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(new ErrorSummary(ex.Message));
            }
        }

        public virtual IServiceResult<bool> DeleteRange(IEnumerable<TPrimaryKey> ids)
        {
            try
            {
                var entities = this.repository.GetList(predicate: x => ids.Contains(x.Id));

                this.repository.DeleteRange(entities);

                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(new ErrorSummary(ex.Message));
            }
        }
    }
}