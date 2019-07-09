// -----------------------------------------------------------------------
// <copyright file="TreeModel.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace KocSistem.FirstStep.WebAPI
{
    public class TreeModel<T>
        where T : TreeModel<T>
    {
        public TreeModel()
            : this(new List<T>())
        {
        }

        public TreeModel(IList<T> children)
        {
            this.Children = children;
        }

        public IList<T> Children { get; set; }
    }
}
