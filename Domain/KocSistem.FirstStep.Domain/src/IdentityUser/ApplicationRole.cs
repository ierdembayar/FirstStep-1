// -----------------------------------------------------------------------
// <copyright file="ApplicationRole.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace KocSistem.FirstStep.Domain
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
            : base()
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
        }

        [Description("Description of role")]
        public virtual string Description { get; set; }
    }
}
