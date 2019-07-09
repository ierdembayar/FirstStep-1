// <copyright file="PagedDtoList.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public class PagedDtoList<T>
    {
        public int IndexFrom { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public IList<T> Items { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }
    }
}
