using System;
using System.Collections.Generic;
using Stormpath.SDK.Impl.Http.Support;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceDataResult : IResourceDataResult
    {
        private readonly ResourceAction action;
        private readonly Type type;
        private readonly CanonicalUri uri;
        private readonly IDictionary<string, object> body;

        public DefaultResourceDataResult(ResourceAction action, Type returnType, CanonicalUri uri, IDictionary<string, object> body)
        {
            this.action = action;
            this.type = returnType;
            this.uri = uri;
            this.body = body;
        }

        ResourceAction IResourceDataResult.Action => this.action;

        IDictionary<string, object> IResourceDataResult.Body => this.body;

        Type IResourceDataResult.Type => this.type;

        CanonicalUri IResourceDataResult.Uri => this.uri;
    }
}
