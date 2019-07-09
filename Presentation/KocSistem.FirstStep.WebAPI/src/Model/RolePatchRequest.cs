// <copyright file="RolePatchRequest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace KocSistem.FirstStep.WebAPI
{
    public class RolePatchRequest
    {
        [Required]
        public string Description { get; set; }
    }
}
