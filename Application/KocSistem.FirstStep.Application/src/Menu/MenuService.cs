// -----------------------------------------------------------------------
// <copyright file="MenuService.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.FirstStep.Domain;
using KocSistem.OneFrame.Data;
using KocSistem.OneFrame.Data.Relational;
using KocSistem.OneFrame.DesignObjects;
using KocSistem.OneFrame.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KocSistem.FirstStep.Application
{
    public class MenuService : IMenuService
    {
        private readonly IDataManager dataManager;

        private readonly IRepository<Menu> menuRepository;
        private readonly IMapper mapper;

        // constructors

        public MenuService(IDataManager dataManager, IRepository<Menu> menuRepository, IMapper mapper)
        {
            this.dataManager = dataManager;
            this.menuRepository = menuRepository;
            this.mapper = mapper;
        }

        public IServiceResult<MenuDto> Add(MenuDto menu)
        {
            var entity = this.MapToEntity(menu);

            this.menuRepository.Add(entity);

            var result = this.MapToDto(entity);

            return this.Ok(result);
        }

        public async Task<IServiceResult<IList<MenuDto>>> GetMenuAsync()
        {
            var menu = (IList<Menu>)await this.menuRepository.GetListAsync().ConfigureAwait(false);

            var result = this.mapper.Map<IList<Menu>, IList<MenuDto>>(menu);

            return this.Ok(result);
        }

        public async Task<IServiceResult<MenuDto>> GetMenuItemAsync(string name)
        {
            Menu menu = await this.menuRepository.GetFirstOrDefaultAsync(selector: m => m, predicate: m => m.Name == name).ConfigureAwait(false);

            var result = this.mapper.Map<Menu, MenuDto>(menu);

            return this.Ok(result);
        }

        private MenuDto MapToDto(Menu entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new MenuDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                Url = entity.Url,
                Name = entity.Name,
            };
        }

        private Menu MapToEntity(MenuDto dto)
        {
            return new Menu()
            {
                Id = dto.Id,
                ParentId = dto.ParentId,
                Url = dto.Url,
                Name = dto.Name,
            };
        }
    }
}
