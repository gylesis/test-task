using System.Collections.Generic;
using UnityEngine;

namespace Project.Ragdoll
{
    public sealed class Ragdoll : MonoBehaviour
    {
        [SerializeField] private float _maxLinearVelocity = 8f;
        [SerializeField] private float _maxAngularVelocity = 14f;
        [SerializeField] private float _maxDepenetrationVelocity = 2.5f;
        [SerializeField, Min(1)] private int _solverIterations = 16;
        [SerializeField, Min(1)] private int _solverVelocityIterations = 6;

        private readonly List<Rigidbody> _bodies = new();
        private readonly List<Collider> _colliders = new();
        private readonly List<BonePose> _poses = new();

        private Animator _animator;
        private bool _initialized;

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            if (_initialized)
                return;

            _animator = GetComponentInParent<Animator>();

            GetComponentsInChildren(true, _bodies);
            GetComponentsInChildren(true, _colliders);

            for (var i = 0; i < _bodies.Count; i++)
            {
                var body = _bodies[i];

                body.maxDepenetrationVelocity = _maxDepenetrationVelocity;
                body.solverIterations = _solverIterations;
                body.solverVelocityIterations = _solverVelocityIterations;
                body.maxLinearVelocity = _maxLinearVelocity;
                body.maxAngularVelocity = _maxAngularVelocity;

                _poses.Add(new BonePose(body.transform));
            }

            _initialized = true;
            Deactivate();
        }

        public void Activate(Vector3 impulse, Vector3 impulsePosition)
        {
            Initialize();

            if (_animator != null)
                _animator.enabled = false;

            Physics.SyncTransforms();

            for (var i = 0; i < _colliders.Count; i++)
                _colliders[i].enabled = true;

            for (var i = 0; i < _bodies.Count; i++)
            {
                var body = _bodies[i];
                body.isKinematic = false;
                body.detectCollisions = true;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.WakeUp();
            }

            IsActive = true;

            if (impulse.sqrMagnitude > 0.0001f)
                ApplyImpulse(impulse, impulsePosition);
        }

        public void Deactivate()
        {
            for (var i = 0; i < _bodies.Count; i++)
            {
                var body = _bodies[i];
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.isKinematic = true;
                body.detectCollisions = false;
            }

            for (var i = 0; i < _colliders.Count; i++)
                _colliders[i].enabled = false;

            if (_animator != null)
                _animator.enabled = true;

            IsActive = false;
        }

        public void Restore()
        {
            Deactivate();

            for (var i = 0; i < _poses.Count; i++)
                _poses[i].Apply();

            if (_animator != null)
                _animator.Rebind();
        }

        private void ApplyImpulse(Vector3 impulse, Vector3 position)
        {
            var closest = FindClosestBody(position);
            var shared = impulse * 0.35f / Mathf.Max(1, _bodies.Count);

            for (var i = 0; i < _bodies.Count; i++)
                _bodies[i].AddForce(shared, ForceMode.VelocityChange);

            if (closest != null)
                closest.AddForceAtPosition(impulse * 0.65f, position, ForceMode.VelocityChange);
        }

        private Rigidbody FindClosestBody(Vector3 position)
        {
            Rigidbody best = null;
            var bestSqr = float.MaxValue;

            for (var i = 0; i < _bodies.Count; i++)
            {
                var sqr = (_bodies[i].worldCenterOfMass - position).sqrMagnitude;

                if (sqr >= bestSqr)
                    continue;

                bestSqr = sqr;
                best = _bodies[i];
            }

            return best;
        }

        private readonly struct BonePose
        {
            private readonly Transform _bone;
            private readonly Vector3 _localPosition;
            private readonly Quaternion _localRotation;

            public BonePose(Transform bone)
            {
                _bone = bone;
                _localPosition = bone.localPosition;
                _localRotation = bone.localRotation;
            }

            public void Apply()
            {
                if (_bone == null)
                    return;

                _bone.localPosition = _localPosition;
                _bone.localRotation = _localRotation;
            }
        }
    }
}
