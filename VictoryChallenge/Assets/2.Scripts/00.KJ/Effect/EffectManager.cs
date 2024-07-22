using UnityEngine;
using System.Collections.Generic;

namespace VictoryChallenge.KJ.Effect
{
    public class EffectManager : SingletonLazy<EffectManager>
    {
        public List<ParticleSystem> fireworkEffects;
        public List<ParticleSystem> sparkEffects;

        void Start()
        {
            StopFirework();
            StopSpark();
        }

        public void PlayFirework()
        {
            foreach (var fireworkEffect in fireworkEffects)
            {
                fireworkEffect.Play();
            }
        }

        public void PlaySpark()
        {
            foreach (var sparkEffect in sparkEffects)
            {
                sparkEffect.Play();
            }
        }

        public void StopFirework()
        {
            foreach (var fireworkEffect in fireworkEffects)
            {
                fireworkEffect.Stop();
            }
        }

        public void StopSpark()
        {
            foreach (var sparkEffects in sparkEffects)
            {
                sparkEffects.Stop();
            }
        }
    }
}
