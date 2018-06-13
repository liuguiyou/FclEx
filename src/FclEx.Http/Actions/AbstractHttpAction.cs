using System;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Helpers;
using FclEx.Http.Core;
using FclEx.Http.Event;
using FclEx.Http.Services;
using FclEx.Utils;

namespace FclEx.Http.Actions
{
    public abstract class AbstractHttpAction : AbstractAction
    {
        protected IHttpService HttpService { get; set; }

        protected AbstractHttpAction(IHttpService httpService, ActionEventListener listener = null) : base(listener)
        {
            HttpService = httpService;
        }

        protected abstract HttpRequestItem BuildRequest();

        protected abstract ValueTask<ActionEvent> HandleResponse(HttpResponseItem response);

        protected virtual void PreCheckResponse(HttpResponseItem response)
        {
            response.EnsureSuccessStatusCode();
        }

        protected override async ValueTask<ActionEvent> ExecuteInternalAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return await NotifyCancelEventAsync().DonotCapture();

            HttpRequestItem requestItem = null;
            try
            {
                requestItem = BuildRequest();
                var response = await HttpService.ExecuteHttpRequestAsync(requestItem, token).DonotCapture();
                PreCheckResponse(response);
                var result = await HandleResponse(response).DonotCapture();
                return result;
            }
            catch (TaskCanceledException)
            {
                return await NotifyCancelEventAsync().DonotCapture();
            }
            catch (Exception ex)
            {
                if (RuntimeInfo.IsDebuggerAttached)
                {
                    // 此处用于生成请求信息，然后用fiddler等工具测试
                    if (requestItem != null)
                    {
                        var url = requestItem.GetUrl();
                        var header = requestItem.GetRequestHeader(HttpService.GetCookies(requestItem.RawUrl));
                        DebuggerHepler.WriteLine(ex.ToString());
                        DebuggerHepler.WriteLine(url);
                        DebuggerHepler.WriteLine(header);
                    }
                }

                return await HandleExceptionAsync(ObjectException.Create(requestItem, ex.Message, ex))
                    .DonotCapture();
            }
            finally
            {
                ++ExcuteTimes;
            }
        }
    }
}
