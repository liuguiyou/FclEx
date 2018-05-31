using System;
using System.Collections.Concurrent;
using System.Reflection;
using FclEx.Http.Core;
using FclEx.Http.Event;
using FclEx.Http.Services;

namespace FclEx.Http.Actions
{
    public abstract class CommonHttpAction : AbstractHttpAction
    {
        protected abstract string Url { get; }

        protected abstract EnumRequestType RequestType { get; }

        protected override HttpRequestItem BuildRequest()
        {
            var req = HttpRequestItem.CreateRequest(Url, RequestType)
                .Compress();
            ModifyRequest(req);
            return req;
        }

        protected virtual void ModifyRequest(HttpRequestItem req) { }

        protected string GetUrl(ConcurrentDictionary<Type, string> apiDic, Type apiType)
        {
            var actionType = GetType();
            return apiDic.GetOrAdd(actionType, key =>
            {
                var urlName = key.Name.Replace("Action", "");
                var value = apiType.GetTypeInfo().GetField(urlName)?.GetValue(null);
                if (value == null) throw new Exception(key.Name + "获取Url失败");
                return value.ToString();
            });
        }

        protected CommonHttpAction(IHttpService httpService, ActionEventListener listener = null) : base(httpService, listener)
        {
        }
    }
}
