﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// suppress warning 4014 and do not have to use "await" for a async method
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task) { }
    }
}