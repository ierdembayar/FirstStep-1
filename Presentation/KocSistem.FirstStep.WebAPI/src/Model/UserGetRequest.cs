// <copyright file="UserGetRequest.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

namespace KocSistem.FirstStep.WebAPI.Model
{
    public class UserSearchRequest : PagedRequest
    {
        public string Username { get; set; }
    }
}
