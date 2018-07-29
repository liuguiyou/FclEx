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

        protected abstract HttpReq BuildRequest();

        protected abstract ValueTask<ActionEvent> HandleResponse(HttpRes response);

        protected virtual void PreCheckResponse(HttpRes response)
        {
            response.EnsureSuccessStatusCode();
        }

        protected override async ValueTask<ActionEvent> ExecuteInternalAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return await NotifyCancelEventAsync().DonotCapture();

            HttpReq req = null;
            try
            {
                req = BuildRequest();
                var response = await HttpService.ExecuteAsync(req, token).DonotCapture();
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
                    if (req != null)
                    {
                        var url = req.GetUrl();
                        var header = req.GetRequestHeader(HttpService.GetCookies(req.Uri.ToString()));
                        DebuggerHepler.WriteLine(ex.ToString());
                        DebuggerHepler.WriteLine(url);
                        DebuggerHepler.WriteLine(header);
                    }
                }

                return await HandleExceptionAsync(ObjectException.Create(req, ex.Message, ex))
                    .DonotCapture();
            }
            finally
            {
                ++ExcuteTimes;
            }
        }
    }
}
