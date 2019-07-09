// <copyright file="ModelMapExtentions.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Security.Claims;

namespace KocSistem.FirstStep.WebAPI
{
    public static class ModelMapExtensions
    {
        public static ClaimResponse MapToClaimResponseModel(this Claim claim)
        {
            return new ClaimResponse()
            {
                Name = claim.Value,
            };
        }
    }
}
