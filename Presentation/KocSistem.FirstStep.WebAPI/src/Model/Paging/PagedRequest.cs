// <copyright file="PagedRequest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace KocSistem.FirstStep.WebAPI.Model
{
    public class PagedRequest
    {
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }

        [Range(0, int.MaxValue)]
        public int PageIndex { get; set; }
    }
}