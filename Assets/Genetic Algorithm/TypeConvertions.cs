using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KurdifyEngine.GA
{
    public static class TypeConversions
    {
        public static T ConvertValue<T>(this string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}