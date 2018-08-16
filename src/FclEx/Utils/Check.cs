﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FclEx.Utils
{
    [DebuggerStepThrough]
    public static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }

        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }

        public static int AtLeast(int value, string parameterName, int min)
        {
            if (value < min)
                throw new ArgumentOutOfRangeException(nameof(value), value, "value cannot be less than " + min);
            return value;
        }

        public static int AtMost(int value, string parameterName, int max)
        {
            if (value > max)
                throw new ArgumentOutOfRangeException(nameof(value), value, "value cannot be greater than " + max);
            return value;
        }

        public static int Between(int value, string parameterName, int min, int max)
        {
            if (value > max || value < min)
                throw new ArgumentOutOfRangeException(nameof(value), value, 
                    $"value cannot be less than {min} nor greater than {max}");
            return value;
        }
    }
}
