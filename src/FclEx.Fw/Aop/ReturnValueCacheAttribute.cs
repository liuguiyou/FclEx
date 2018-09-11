using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using EasyCaching.Core;
using FclEx.Fw.Caching.Extensions;
using FclEx.Fw.Dependency.Extensions;
using Microsoft.Extensions.Logging;
using Overby.Extensions.Attachments;

namespace FclEx.Fw.Aop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ReturnValueCacheAttribute : AbstractInterceptorAttribute
    {
        public const string CacheName = "ReturnValueCache";
        private bool? _isStatic = null;
        private int? _expireSeconds;
        private static readonly MethodInfo _taskFromResult = typeof(Task).GetMethod(nameof(Task.FromResult));
        private static readonly MethodInfo _toValueTask = typeof(ValueTaskExtensions)
            .GetMethod(nameof(ValueTaskExtensions.ToValueTask));

        public bool IsStatic
        {
            set => _isStatic = value;
            get => _isStatic ?? default;
        }

        public int ExpireSeconds
        {
            set => _expireSeconds = value;
            get => _expireSeconds ?? default;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var method = context.ServiceMethod;
            var returnType = method.ReturnType;
            var provider = context.ServiceProvider;

            if (returnType == typeof(void)
                || returnType == typeof(Task)
                || returnType == typeof(ValueTask)
                || !provider.TryGetService<IEasyCachingProvider>(out var cache))
            {
                await context.Invoke(next);
                return;
            }

            var key = method.GetSignature();
            var parasKey = context.Parameters.ToJson().ToUtf8Bytes().ToMd5();
            key = $"{key}_{parasKey}";
            if (!method.IsStatic && _isStatic != true)
            {
                var instance = context.Proxy;
                key = $"{key}_{instance.GetReferenceId()}";
            }

            if (!cache.TryGet<object>(key, out var item))
            {
                await context.Invoke(next);
                item = context.IsAsync()
                    ? (object)(await (dynamic)context.ReturnValue)
                    : context.ReturnValue;

                var time = _expireSeconds.HasValue
                    ? TimeSpan.FromSeconds(_expireSeconds.Value)
                    : TimeSpan.FromDays(30);
                cache.Set(key, item, time);
            }
            else
            {
                if (context.IsAsync())
                {
                    var returnTypeOfGeneric = returnType.GetGenericTypeDefinition();
                    var objType = returnType.GetTypeInfo().GenericTypeArguments[0];
                    if (returnTypeOfGeneric == typeof(Task<>))
                    {
                        var m = _taskFromResult.MakeGenericMethod(objType);
                        context.ReturnValue = m.Invoke(null, new[] { item });
                    }
                    else if (returnTypeOfGeneric == typeof(ValueTask<>))
                    {
                        var m = _toValueTask.MakeGenericMethod(objType);
                        context.ReturnValue = m.Invoke(null, new[] { item });
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    context.ReturnValue = item;
                }

                var logger = provider.GetLogger<ReturnValueCacheAttribute>();
                logger.LogDebug($"[{CacheName}][缓存命中][{method.GetFullName()}]");
            }
        }
    }
}
