using UnityEngine;

namespace Project.Animations
{
    public class CharacterAnimatorView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speedDamping = 0.12f;

        protected virtual void Awake()
        {
            _animator.applyRootMotion = false;
            _animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }

        public void SetSpeed(float normalized)
        {
            _animator.SetFloat(AnimatorParameters.Speed, normalized, _speedDamping, Time.deltaTime);
        }

        public void ResetSpeed()
        {
            _animator.SetFloat(AnimatorParameters.Speed, 0f);
        }

        public void SetPlaybackSpeed(float value)
        {
            _animator.speed = value;
        }

        protected void Trigger(int parameter)
        { 
            _animator.SetTrigger(parameter);
        }

        protected void ResetTriggerSafe(int parameter)
        { 
            _animator.ResetTrigger(parameter);
        }
    }
}
