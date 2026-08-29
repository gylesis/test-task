using System;
using UnityEngine;

namespace Project.Vfx
{
    public class Effect : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField, Min(0f)] private float _lifetime = 1.5f;

        private float _timeLeft;
        private bool _playing;

        public string Id => _id;
        public float Lifetime => _lifetime;

        public event Action<Effect> Finished;

        public void Play(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            _timeLeft = ResolveLifetime();
            _playing = true;

            OnPlay();
        }

        public void StopImmediate()
        {
            if (!_playing)
                return;

            _playing = false;
            OnStop();
            Finished?.Invoke(this);
        }

        protected virtual void OnPlay() { }

        protected virtual void OnStop() { }

        protected virtual float ResolveLifetime()
        {
            return _lifetime;
        }

        protected void Finish()
        {
            StopImmediate();
        }

        protected virtual void Update()
        {
            if (!_playing)
                return;

            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f)
                Finish();
        }
    }
}
