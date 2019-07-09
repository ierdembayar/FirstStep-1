// <copyright file="UserGetResponse.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

namespace KocSistem.FirstStep.WebAPI.Model
{
    public class UserGetResponse
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
