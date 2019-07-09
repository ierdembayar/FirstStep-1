// -----------------------------------------------------------------------
// <copyright file="CrudServiceBase.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.Mapper;

namespace KocSistem.FirstStep.Application
{
    public abstract class CrudServiceBase<TEntity, TDto> : CrudServiceBase<TEntity, TDto, int>
     where TEntity : class, IEntityHasId<int>, new()
     where TDto : class, IDtoHasId<int>, new()
    {
        protected CrudServiceBase(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceBase<TEntity, TDto, TPrimaryKey> : CrudServiceBase<TEntity, TDto, TPrimaryKey, TDto>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected CrudServiceBase(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceBase<TEntity, TDto, TPrimaryKey, TCreateInput> : CrudServiceBase<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
    {
        protected CrudServiceBase(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public abstract class CrudServiceBase<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : IDtoHasId<TPrimaryKey>
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly IRepository<TEntity> repository;
#pragma warning restore CA1051 // Do not declare visible instance fields

#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly IDataManager dataManager;
#pragma warning restore CA1051 // Do not declare visible instance fields

#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly IMapper mapper;
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected CrudServiceBase(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
        {
            this.repository = repository;
            this.dataManager = dataManager;
            this.mapper = mapper;
        }
    }
}
