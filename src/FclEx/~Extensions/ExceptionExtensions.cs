﻿using System;

namespace FclEx
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception ex)
        {
            return ex.InnerException != null ? $"{ex.Message}[{GetAllMessages(ex.InnerException)}]" : ex.Message;
        }
    }
}