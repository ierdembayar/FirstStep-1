// <copyright file="MenuResponse.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using Newtonsoft.Json;

namespace KocSistem.FirstStep.WebAPI
{
    public class MenuResponse : TreeModel<MenuResponse>
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int? ParentId { get; set; }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        public string Name { get; set; }
    }
}
