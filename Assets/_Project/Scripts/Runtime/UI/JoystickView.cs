using Project.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI
{
    public sealed class JoystickView : MonoBehaviour, IJoystick, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _handleRange = 60f;
        [SerializeField] private bool _hideWhenIdle;

        private Canvas _canvas;
        private Camera _eventCamera;
        private Vector2 _direction;

        public Vector2 Direction => _direction;

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

            var offset = Vector2.ClampMagnitude(localPoint, _handleRange);
            _direction = offset / _handleRange;

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
