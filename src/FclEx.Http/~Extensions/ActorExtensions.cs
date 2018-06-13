using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.Actions;
using FclEx.Http.Event;

namespace FclEx.Http
{
    public static class ActorExtensions
    {
        public static ValueTask<ActionEvent> ExecuteAsync(this IActor actor) => actor.ExecuteAsync(default);

        /// <summary>
        /// 当结果是重试或重复的时候自动再次执行
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async ValueTask<ActionEvent> ExecuteAutoAsync(this IActor actor, CancellationToken token = default)
        {
            ActionEvent result;
            do
            {
                result = await actor.ExecuteAsync(token).DonotCapture();
            } while (!token.IsCancellationRequested
                     && (result.Type == ActionEventType.EvtRepeat || result.Type == ActionEventType.EvtRetry));
            return result;
        }

        /// <summary>
        /// 不断执行
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="endCondition"></param>
        /// <returns></returns>
        public static async ValueTask<ActionEvent> ExecuteForeverAsync(this IActor actor, Func<ActionEvent, bool> endCondition = null)
        {
            ActionEvent result;
            do
            {
                result = await actor.ExecuteAutoAsync().DonotCapture();
            } while (endCondition == null || !endCondition(result));
            return result;
        }

        public static async ValueTask<ActionEvent[]> Execute<T>(this ICollection<T> actors, int parallelism = 0, 
            CancellationToken token = default) 
            where T : IActor
        {
            parallelism = parallelism < 1 ? Environment.ProcessorCount : parallelism;
            var list = new List<ActionEvent>(actors.Count);
            foreach (var group in actors.Partition(parallelism))
            {
                if (token.IsCancellationRequested) break;
                var groupResult = await group.Select(m => m.ExecuteAutoAsync(token)).WhenAll().DonotCapture();
                list.AddRange(groupResult);
            }
            return list.ToArray();
        }
    }
}
