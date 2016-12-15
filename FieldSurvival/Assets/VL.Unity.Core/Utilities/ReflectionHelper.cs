using System;

namespace Assets.VL.VL.Unity.Core.Utilities
{

    public class ReflectionHelper
    {
        public static string GetClassName(Type @class)
        {
            return @class.Name;
        }
        public static string GetFunctionName(Action method)
        {
            return method.Method.Name;
        }
    }
}
