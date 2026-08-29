using UnityEngine;

namespace Project.Vfx
{
    public sealed class ParticleEffect : Effect
    {
        [SerializeField] private ParticleSystem _particles;

        private void Awake()
        {
            if (_particles == null)
                _particles = GetComponentInChildren<ParticleSystem>();
        }

        protected override float ResolveLifetime()
        {
            if (_particles == null)
                return base.ResolveLifetime();

            var main = _particles.main;
            return main.duration + main.startLifetime.constantMax;
        }

        protected override void OnPlay()
        {
            if (_particles == null)
                return;

            _particles.Clear(true);
            _particles.Play(true);
        }

        protected override void OnStop()
        {
            if (_particles != null)
                _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
