using System;
using System.Threading;

namespace FclEx.Fm.Logging
{
    public class ColorConsoleLoggerScope
    {
        private static readonly AsyncLocal<ColorConsoleLoggerScope> _scope = new AsyncLocal<ColorConsoleLoggerScope>();
        private readonly string _name;
        private readonly object _state;

        public ColorConsoleLoggerScope Parent { get; private set; }

        public static ColorConsoleLoggerScope Current
        {
            get => _scope.Value;
            set => _scope.Value = value;
        }

        internal ColorConsoleLoggerScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public static IDisposable Push(string name, object state)
        {
            var current = Current;
            Current = new ColorConsoleLoggerScope(name, state) {Parent = current};
            return new DisposableScope();
        }

        public override string ToString()
        {
            var state = _state;
            return state.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
