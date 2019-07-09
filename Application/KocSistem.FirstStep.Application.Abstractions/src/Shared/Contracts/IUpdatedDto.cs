// -----------------------------------------------------------------------
// <copyright file="IUpdatedDto.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface IUpdatedDto
    {
        DateTime UpdatedDate { get; set; }
    }
}
