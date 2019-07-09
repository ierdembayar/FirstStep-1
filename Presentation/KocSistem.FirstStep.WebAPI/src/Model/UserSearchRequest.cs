// <copyright file="UserSearchRequest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using KocSistem.FirstStep.WebAPI.Model;

namespace KocSistem.FirstStep.WebAPI
{
    public class UserSearchRequest : PagedRequest
    {
        public string Username { get; set; }
    }
}