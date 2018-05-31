using System.ComponentModel;

namespace FclEx.Http.Event
{
    [Description("动作事件类型")]
    public enum ActionEventType
    {
        [Description("成功")]
        EvtOk,
        [Description("错误")]
        EvtError,
        [Description("取消")]
        EvtCanceled,
        [Description("重试")]
        EvtRetry,
        [Description("再次执行")]
        EvtRepeat
    }
}
