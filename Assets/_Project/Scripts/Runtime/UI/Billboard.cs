using UnityEngine;

namespace Project.UI
{
    public sealed class Billboard : MonoBehaviour
    {
        [SerializeField] private bool _lockYAxis;

        private Transform _cameraTransform;

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                var main = Camera.main;
                if (main == null)
                    return;

                _cameraTransform = main.transform;
            }

            var forward = _cameraTransform.forward;

            if (_lockYAxis)
            {
                forward.y = 0f;
                if (forward.sqrMagnitude <= 0.0001f)
                    return;
            }

            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}
