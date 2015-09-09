using System;
using System.Collections.Generic;
using Stormpath.SDK.Impl.Http.Support;

namespace Stormpath.SDK.Impl.DataStore
{
    internal interface IResourceDataResult
    {
        ResourceAction Action { get; }

        Type Type { get; }

        CanonicalUri Uri { get; }

        IDictionary<string, object> Body { get; }
    }
}
