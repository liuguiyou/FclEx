using System;

namespace FclEx.Http.Actions
{    /// <summary>
     /// 用于按顺序执行一些action，前一个action成功则继续执行，否则则退出
     /// </summary>
    public interface IActionFuture : IActor
    {
        /// <summary>
        /// 放入一个action到执行队列末尾
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IActionFuture PushAction(IAction action);

        /// <summary>
        /// 放入一个根据上一个action执行结果生成action的委托到执行队列末尾
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IActionFuture PushAction(Func<object, IAction> func);

        /// <summary>
        /// 放入一个根据第i个action执行结果生成action的委托到执行队列末尾
        /// </summary>
        /// <param name="func"></param>
        /// <param name="dependentResultIndex"></param>
        /// <returns></returns>
        IActionFuture PushAction(Func<object, IAction> func, int dependentResultIndex);

        /// <summary>
        /// 放入一个根据前面所有action执行结果生成action的委托到执行队列末尾
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IActionFuture PushAction(Func<object[], IAction> func);
    }
}
