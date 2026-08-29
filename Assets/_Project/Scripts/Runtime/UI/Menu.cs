using UnityEngine;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private bool _shownByDefault;

        public bool IsShown { get; private set; }

        protected CanvasGroup CanvasGroup => _canvasGroup;

        public virtual void Show()
        {
            IsShown = true;
            ApplyVisibility(true);
        }

        public virtual void Hide()
        {
            IsShown = false;
            ApplyVisibility(false);
        }

        public void SetShown(bool shown)
        {
            if (shown)
                Show();
            else
                Hide();
        }

        protected virtual void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            SetShown(_shownByDefault);
        }

        private void ApplyVisibility(bool visible)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.blocksRaycasts = visible;
            _canvasGroup.interactable = visible;
        }
    }
}
