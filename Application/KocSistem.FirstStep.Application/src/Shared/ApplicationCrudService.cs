// -----------------------------------------------------------------------
// <copyright file="ApplicationCrudService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace KocSistem.FirstStep.Application
{
    public class ApplicationCrudService<TEntity, TDto>
       : CrudService<TEntity, TDto, int>, IApplicationCrudService<TEntity, TDto>
           where TEntity : class, IEntityHasId<int>, new()
           where TDto : class, IDtoHasId<int>, new()
    {
        protected ApplicationCrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager) : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudService<TEntity, TDto, TPrimaryKey>
        : CrudService<TEntity, TDto, TPrimaryKey>, IApplicationCrudService<TEntity, TDto, TPrimaryKey>
            where TEntity : class, IEntityHasId<TPrimaryKey>, new()
            where TDto : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected ApplicationCrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager) : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput>
     : CrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TDto>, IApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput>
         where TEntity : class, IEntityHasId<TPrimaryKey>, new()
         where TDto : class, IDtoHasId<TPrimaryKey>, new()
         where TCreateInput : class, IDto, new()
    {
        protected ApplicationCrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager) : base(repository, mapper, dataManager)
        {
        }
    }

    public class ApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
    : CrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>, IApplicationCrudService<TEntity, TDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntityHasId<TPrimaryKey>, new()
        where TDto : class, IDtoHasId<TPrimaryKey>, new()
        where TCreateInput : class, IDto, new()
        where TUpdateInput : class, IDtoHasId<TPrimaryKey>, new()
    {
        protected ApplicationCrudService(IRepository<TEntity> repository, IMapper mapper, IDataManager dataManager) : base(repository, mapper, dataManager)
        {
        }
    }
}
