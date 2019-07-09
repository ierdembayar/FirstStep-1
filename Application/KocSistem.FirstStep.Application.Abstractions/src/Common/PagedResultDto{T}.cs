// -----------------------------------------------------------------------
// <copyright file="PagedResultDto{T}.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public class PagedResultDto<T>
    {
        public PagedResultDto()
        {
        }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }

        public IEnumerable<T> PagedList { get; set; }
    }
}
