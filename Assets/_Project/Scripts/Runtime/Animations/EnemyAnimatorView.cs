namespace Project.Animations
{
    public sealed class EnemyAnimatorView : CharacterAnimatorView
    {
        public void PlaySpawn()
        {
            ResetTriggerSafe(AnimatorParameters.Death);
            ResetSpeed();
            Trigger(AnimatorParameters.Spawn);
        }

        public void PlayAttack()
        {
            Trigger(AnimatorParameters.Attack);
        }

        public void PlayDeath()
        {
            ResetSpeed();
            Trigger(AnimatorParameters.Death);
        }
    }
}
