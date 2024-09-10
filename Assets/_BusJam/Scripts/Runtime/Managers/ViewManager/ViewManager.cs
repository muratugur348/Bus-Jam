using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace BusJam.Scripts.Runtime.Managers.ViewManager
{
    public class ViewManager : IViewManager
    {
        readonly Dictionary<string, GameObject> _openedViews = new();

        public void LoadView<T>(LoadViewParams<T> loadViewParams, bool closeAllViews = false)
        {
            if (closeAllViews)
            {
                DestroyAllViews();
            }
            
            if (!_openedViews.ContainsKey(loadViewParams.GetViewPresenterType()))
            {
                Addressables.LoadAssetAsync<GameObject>(loadViewParams.ViewName).Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject viewPrefab = obj.Result;
                        GameObject viewInstance = Object.Instantiate(viewPrefab);
                        viewInstance.transform.SetAsLastSibling();
                        _openedViews[loadViewParams.GetViewPresenterType()] = viewInstance;
                        loadViewParams.OnLoad?.Invoke(viewInstance.GetComponent<T>());
                    }
                };
            }
        }

        public void DestroyView<T>()
        {
            var viewName = typeof(T).Name;
            if (_openedViews.ContainsKey(viewName))
            {
                GameObject viewInstance = _openedViews[viewName];
                Object.Destroy(viewInstance);
                _openedViews.Remove(viewName);
            }
        }

        public void DestroyAllViews()
        {
            foreach (var view in _openedViews)
            {
                Object.Destroy(view.Value);
            }
            _openedViews.Clear();
        }
    }
}