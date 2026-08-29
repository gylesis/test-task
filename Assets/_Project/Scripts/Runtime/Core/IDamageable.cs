using UnityEngine;

namespace Project.Core
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        Transform Transform { get; }
        void TakeDamage(float amount);
    }
}
