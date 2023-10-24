// Copyright (c) Laserfiche
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Newtonsoft.Json;

namespace Laserfiche.Api.Client.APIServer
{
    /// <summary>
    /// The API client for the Laserfiche Self-Hosted token route.
    /// </summary>
    public partial interface ITokenClient
    {
    }

    /// <summary>
    /// The API client for the Laserfiche Self-Hosted token route.
    /// </summary>
    partial class TokenClient
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.MaxDepth = 128;
        }
    }
}
