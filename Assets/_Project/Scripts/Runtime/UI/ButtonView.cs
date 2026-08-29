using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class ButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        public event Action Clicked;

        public bool Interactable
        {
            get => _button != null && _button.interactable;
            set
            {
                if (_button != null)
                    _button.interactable = value;
            }
        }

        public void SetCaption(string caption)
        {
            if (_label != null)
                _label.text = caption;
        }

        protected virtual void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnClicked);
        }

        protected virtual void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked?.Invoke();
        }
    }
}
