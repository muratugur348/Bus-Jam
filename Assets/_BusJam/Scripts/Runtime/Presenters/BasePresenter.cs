using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.UI.UIElements;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters
{
    public abstract class BasePresenter<T> : MonoBehaviour
    {
        [SerializeField] protected RectTransform container;
        [SerializeField] private UIButton closeButton;

        private IViewManager _viewManager;

        protected virtual void Start()
        {
            _viewManager = Locator.Instance.Resolve<IViewManager>();
            closeButton?.onClick.AddListener(OnCloseButtonClicked);
        }
        
        // For testing purposes
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                container.gameObject.SetActive(!container.gameObject.activeSelf);
            }
        }

        private void OnCloseButtonClicked()
        {
            Close();
        }

        protected virtual void Close()
        {
            _viewManager?.DestroyView<T>();
        }

        protected virtual void OnDestroy()
        {
            closeButton?.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}