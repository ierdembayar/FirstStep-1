// -----------------------------------------------------------------------
// <copyright file="IFullAuditModel{T}.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace KocSistem.FirstStep.Application.Abstractions
{
    public interface IFullAuditModel<T> : IDtoHasId<T>, ICreatedDto, IUpdatedDto
    {
    }
}
