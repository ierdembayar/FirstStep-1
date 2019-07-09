// -----------------------------------------------------------------------
// <copyright file="MenuControllerTest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.FirstStep.WebAPI;
using KocSistem.OneFrame.DesignObjects;
using KocSistem.OneFrame.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace KocSistem.FirstStep.Tests
{
    public class MenuControllerTest
    {
        private readonly Mock<IMenuService> menuServiceMock;
        private readonly Mock<IMapper> mapperMock;

        public MenuControllerTest()
        {
            this.menuServiceMock = new Mock<IMenuService>();
            this.mapperMock = new Mock<IMapper>();
        }

        [Fact]
        [Trait("Category", "MenuController")]
        public async Task Get_Succeeded_ReturnsOkResult()
        {
            var menuDtoList = new List<MenuDto>()
            {
                new MenuDto() { Id = 1, Name = "ParentMenu", ParentId = null, Url = "parentmenuurl" },
                new MenuDto() { Id = 2, Name = "ChildMenu1", ParentId = 1, Url = "childmenu1url" },
                new MenuDto() { Id = 3, Name = "ChildMenu2", ParentId = 1, Url = "childmenu2url" },
            };

            var menuResult = new ServiceResult<IList<MenuDto>>(menuDtoList);

            var menuModelList = new List<MenuResponse>()
            {
                new MenuResponse() { Id = 1, Name = "ParentMenu", ParentId = null, Url = "parentmenuurl" },
                new MenuResponse() { Id = 2, Name = "ChildMenu1", ParentId = 1, Url = "childmenu1url" },
                new MenuResponse() { Id = 3, Name = "ChildMenu2", ParentId = 1, Url = "childmenu2url" },
            };

            var claims = new List<Claim>()
            {
                new Claim("Menu1", "1"),
                new Claim("Menu2", "2"),
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = claimsPrincipal,
                },
            };

            this.menuServiceMock.Setup(mock => mock.GetMenuAsync()).ReturnsAsync(menuResult);
            this.mapperMock.Setup(mock => mock.Map<IList<MenuDto>, IList<MenuResponse>>(It.IsAny<IList<MenuDto>>())).Returns(menuModelList);

            var controller = new MenuController(this.menuServiceMock.Object, this.mapperMock.Object)
            {
                ControllerContext = context,
            };

            var response = await controller.Get().ConfigureAwait(false);
            Assert.IsAssignableFrom<OkObjectResult>(response);

            var okResult = (OkObjectResult)response;
            Assert.IsAssignableFrom<IList<MenuResponse>>(okResult.Value);

            var menuModelResponseList = (List<MenuResponse>)okResult.Value;
            Assert.True(menuModelResponseList.Where(m => m.ParentId == null).Count() == 1);

            var root = menuModelResponseList.Where(m => m.ParentId == null).First();
            Assert.True(root.Children.Count == 2);

            Assert.True(root.Children.Distinct().Count() == root.Children.Count());
        }


        [Fact]
        [Trait("Category", "MenuController")]
        public async Task Get_InvalidMenu_ReturnsKeyNotFoundException()
        {
            var menuDtoList = new List<MenuDto>()
            {
                new MenuDto() { Id = 1, Name = "ParentMenu1", ParentId = null, Url = "parentmenu1url" },
                new MenuDto() { Id = 2, Name = "ChildMenu1", ParentId = 1, Url = "childmenu1url" },
                new MenuDto() { Id = 4, Name = "ChildMenu2", ParentId = 3, Url = "childmenu2url" },
            };

            var menuResult = new ServiceResult<IList<MenuDto>>(menuDtoList);

            var menuModelList = new List<MenuResponse>()
            {
                new MenuResponse() { Id = 1, Name = "ParentMenu", ParentId = null, Url = "parentmenuurl" },
                new MenuResponse() { Id = 2, Name = "ChildMenu1", ParentId = 1, Url = "childmenu1url" },
                new MenuResponse() { Id = 4, Name = "ChildMenu2", ParentId = 3, Url = "childmenu2url" },
            };

            var claims = new List<Claim>()
            {
                new Claim("Menu1", "1"),
                new Claim("Menu2", "2"),
                new Claim("Menu4", "4"),
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = claimsPrincipal,
                },
            };

            this.menuServiceMock.Setup(mock => mock.GetMenuAsync()).ReturnsAsync(menuResult);
            this.mapperMock.Setup(mock => mock.Map<IList<MenuDto>, IList<MenuResponse>>(It.IsAny<IList<MenuDto>>())).Returns(menuModelList);

            var controller = new MenuController(this.menuServiceMock.Object, this.mapperMock.Object)
            {
                ControllerContext = context,
            };
            var ex = await Record.ExceptionAsync(() => controller.Get()).ConfigureAwait(false);

            Assert.IsAssignableFrom<KeyNotFoundException>(ex);
        }

        [Fact]
        [Trait("Category", "MenuController")]
        public async Task Get_NonExistingMenu_ReturnsOkResult()
        {
            var menuDtoList = new List<MenuDto>();

            var menuResult = new ServiceResult<IList<MenuDto>>(menuDtoList);

            var menuModelList = new List<MenuResponse>();

            var claims = new List<Claim>();

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = claimsPrincipal,
                },
            };

            this.menuServiceMock.Setup(mock => mock.GetMenuAsync()).ReturnsAsync(menuResult);
            this.mapperMock.Setup(mock => mock.Map<IList<MenuDto>, IList<MenuResponse>>(It.IsAny<IList<MenuDto>>())).Returns(menuModelList);

            var controller = new MenuController(this.menuServiceMock.Object, this.mapperMock.Object)
            {
                ControllerContext = context,
            };
            var response = await controller.Get().ConfigureAwait(false);
            Assert.IsAssignableFrom<OkObjectResult>(response);
        }
    }
}
