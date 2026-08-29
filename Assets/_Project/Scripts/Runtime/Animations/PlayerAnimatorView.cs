namespace Project.Animations
{
    public sealed class PlayerAnimatorView : CharacterAnimatorView
    {
        public void PlayShoot()
        {
            Trigger(AnimatorParameters.Shoot);
        }

        public void PlayDeath()
        {
            ResetSpeed();
            Trigger(AnimatorParameters.Death);
        }
    }
}
