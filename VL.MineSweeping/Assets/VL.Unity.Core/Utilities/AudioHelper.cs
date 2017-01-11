using UnityEngine;

namespace Assets.VL.VL.Unity.Core.Utilities
{
    public static class AudioHelper
    {
        public static void Play(this AudioSource source,AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        public static float lowPitchRange = 0.95f;
        public static float highPitchRange = 1.05f;
        public static void Play(this AudioSource source, AudioClip[] clips)
        {
            int randomIndex = Random.Range(0, clips.Length);
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            source.pitch = randomPitch;
            source.clip = clips[randomIndex];
            source.Play();
        }
    }
}
