﻿using System;
using System.Diagnostics;

namespace FclEx.Helpers
{
    public static class DebuggerHepler
    {
        public static void Write(string msg)
        {
            if (Debugger.IsLogging())
                Debugger.Log(1, "", msg);
        }

        public static void WriteLine(string msg)
        {
            if (Debugger.IsLogging())
                Debugger.Log(1, "", msg + Environment.NewLine);
        }
    }
}
