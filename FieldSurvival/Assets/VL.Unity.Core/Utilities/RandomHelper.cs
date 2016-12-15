using UnityEngine;

namespace Assets.VL.VL.Unity.Core.Utilities
{
    public static class RandomHelper
    {
        public static GameObject GetRandomOne(this GameObject[] objects)
        {
            return objects[Random.Range(0, objects.Length)];
        }
        public static AudioClip GetRandomOne(this AudioClip[] objects)
        {
            return objects[Random.Range(0, objects.Length)];
        }
        public static int Next(int min, int max)
        {
            return Random.Range(min, max);
        }
    }
}
