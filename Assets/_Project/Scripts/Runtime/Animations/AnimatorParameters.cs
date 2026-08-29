using UnityEngine;

namespace Project.Animations
{
    public static class AnimatorParameters
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Shoot = Animator.StringToHash("Shoot");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Spawn = Animator.StringToHash("Spawn");
    }
}
