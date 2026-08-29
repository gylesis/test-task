using System.Collections;
using UnityEngine;

namespace Project.Infrastructure
{
    public sealed class CoroutineRunner : MonoBehaviour
    {
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void Cancel(Coroutine routine)
        {
            if (routine != null)
                StopCoroutine(routine);
        }
    }
}
