using Project.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI
{
    public sealed class JoystickView : MonoBehaviour, IJoystick, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField, Range(0.1f, 1f)] private float _handleRangeFactor = 0.45f;
        [SerializeField] private bool _hideWhenIdle;

        private Canvas _canvas;
        private Camera _eventCamera;
        private Vector2 _direction;

        public Vector2 Direction => _direction;

        private float HandleRange
        {
            get
            {
                if (_background == null)
                    return 0f;

                var size = _background.rect.size;
                return Mathf.Min(size.x, size.y) * 0.5f * _handleRangeFactor;
            }
        }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_hideWhenIdle && _background != null)
                _background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _eventCamera = _canvas != null && _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : eventData.pressEventCamera;

            if (_hideWhenIdle && _background != null)
            {
                _background.gameObject.SetActive(true);
                _background.position = eventData.position;
            }

            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_background == null)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, _eventCamera, out var localPoint);

            var range = HandleRange;

            if (range <= 0.0001f)
                return;

            var offset = Vector2.ClampMagnitude(localPoint - _background.rect.center, range);

            _direction = offset / range;

            if (_handle != null)
                _handle.anchoredPosition = offset;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _direction = Vector2.zero;

            if (_handle != null)
                _handle.anchoredPosition = Vector2.zero;

            if (_hideWhenIdle && _background != null)
                _background.gameObject.SetActive(false);
        }
    }
}
