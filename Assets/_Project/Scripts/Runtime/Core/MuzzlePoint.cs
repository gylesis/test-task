using UnityEngine;

namespace Project.Core
{
    public sealed class MuzzlePoint : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.06f);
            Gizmos.DrawRay(transform.position, transform.forward * 0.3f);
        }
    }
}
