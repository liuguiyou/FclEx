using System;
using System.Threading.Tasks;
using FclEx.Http.Core;
using FclEx.Http.Event;
using FclEx.Http.Services;

namespace FclEx.Http.Actions
{
    public class DownloadAction : AbstractHttpAction
    {
        protected readonly string _url;

        [Obsolete]
        public DownloadAction(IHttpService httpService, string url, ActionEventListener listener = null) : base(httpService, listener)
        {
            _url = url;
        }

        public DownloadAction(string url, IHttpService httpService, ActionEventListener listener = null) : base(httpService, listener)
        {
            _url = url;
        }

        protected override HttpRequestItem BuildRequest()
        {
            var req = HttpRequestItem.CreateGetRequest(_url);
            req.Compress();
            req.ResultType = HttpResultType.Byte;
            ModifyRequest(req);
            return req;
        }

        protected virtual void ModifyRequest(HttpRequestItem req) { }

        protected override ValueTask<ActionEvent> HandleResponse(HttpResponseItem response)
        {
            return NotifyOkEventAsync(response.ResponseBytes);
        }
    }
}
