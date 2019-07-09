// <copyright file="ApplicationCrudServiceAsync.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.Mapper;

namespace KocSistem.FirstStep.Application
{
    public class ApplicationCrudServiceAsync<TEntity, TDto>
       : CrudServiceAsync<TEntity, TDto, int>, IApplicationCrudServiceAsync<TEntity, TDto>
           where TEntity : class, IEntityHasId<int>, new()
           where TDto : class, IDtoHasId<int>, new()
    {
        protected ApplicationCrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey>
        : CrudServiceAsync<TEntity, TDto, TPrimaryKey>, IApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected ApplicationCrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput>
     : CrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>, IApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput>
         where TEntity : class, IEntityHasId<TPrimaryKey>, new()
         where TDto : class, IDtoHasId<TPrimaryKey>, new()
         where TCreateInput : class, IDto, new()
    {
        protected ApplicationCrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
    : CrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>, IApplicationCrudServiceAsync<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected ApplicationCrudServiceAsync(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager)
            : base(repository, mapper, dataManager)
        {
        }
    }
}
