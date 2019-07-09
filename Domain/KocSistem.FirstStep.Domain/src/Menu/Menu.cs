// -----------------------------------------------------------------------
// <copyright file="Menu.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.OneFrame.Data.Relational;
using System.Collections.Generic;

namespace KocSistem.FirstStep.Domain
{
    public class Menu : IEntity, IEntityHasId<int>
    {
        public virtual int Id { get; set; }

        public virtual int? ParentId { get; set; }

        public virtual string Name { get; set; }

#pragma warning disable CA1056 // Uri properties should not be strings
        public virtual string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        public virtual Menu ParentMenu { get; set; }

        public virtual ICollection<Menu> ChildMenu { get; set; }
    }
}
