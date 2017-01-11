using System.Collections.Generic;
using UnityEngine;

namespace Assets.VL.Unity.Core.Utilities
{
    public abstract class IJar<T>
    {
        public List<T> Values;

        public T Roll()
        {
            var value = Values[Random.Range(0, Values.Count)];
            Values.Remove(value);
            return value;
        }
    }
    public class IntJar : IJar<int>
    {
        public IntJar(int minValue, int maxValue)
        {
            Values = new List<int>();
            for (int i = minValue; i <= maxValue; i++)
            {
                Values.Add(i);
            }
        }
    }
    public class IntPair
    {
        public int X;
        public int Y;

        public IntPair(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class IntPairJar : IJar<IntPair>
    {
        public IntPairJar(int widthMin, int widthMax, int heightMin, int heightMax)
        {
            Values = new List<IntPair>();
            for (int w = widthMin; w <= widthMax; w++)
            {
                for (int h = heightMin; h < heightMax; h++)
                {
                    Values.Add(new IntPair(w, h));
                }
            }
        }
    }
    public static class JarHelper
    {
        public static IntJar GetJarOfInt(int minValue, int maxValue)
        {
            return new IntJar(minValue, maxValue);
        }
        public static IntPairJar GetJarOfIntPair(int widthMin, int widthMax, int heightMin, int heightMax)
        {
            return new IntPairJar(widthMin, widthMax, heightMin, heightMax);
        }
    }
}
