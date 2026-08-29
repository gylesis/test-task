using System.Collections.Generic;
using Project.Configs;
using Project.Enemies.States;
using UnityEngine;

namespace Project.Enemies
{
    public static class EnemyStatesFactory
    {
        public static IReadOnlyList<EnemyState> Build(EnemyContext context)
        {
            var states = new List<EnemyState>
            {
                new EnemySpawnState(context),
                new EnemyChaseState(context),
                new EnemyDeathState(context)
            };

            states.Add(CreateCombatState(context));
            return states;
        }

        public static void EnterCombat(EnemyContext context)
        {
            switch (context.Config.AttackType)
            {
                case EnemyAttackType.Ranged:
                    context.Controller.Machine.Enter<EnemyRangedAttackState>();
                    break;

                default:
                    context.Controller.Machine.Enter<EnemyAttackState>();
                    break;
            }
        }

        private static EnemyState CreateCombatState(EnemyContext context)
        {
            switch (context.Config.AttackType)
            {
                case EnemyAttackType.Ranged:
                    return new EnemyRangedAttackState(context);

                case EnemyAttackType.Melee:
                    return new EnemyAttackState(context);

                default:
                    Debug.LogWarning($"Unhandled attack type '{context.Config.AttackType}', falling back to melee.");
                    return new EnemyAttackState(context);
            }
        }
    }
}
