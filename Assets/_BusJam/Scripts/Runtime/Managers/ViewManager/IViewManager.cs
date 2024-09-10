using System;

namespace BusJam.Scripts.Runtime.Managers.ViewManager
{
    public class LoadViewParams<T> : EventArgs
    {
        public readonly string ViewName;
        public Action<T> OnLoad;

        readonly string _viewPresenterType;

        public LoadViewParams(string viewName)
        {
            ViewName = viewName;
            _viewPresenterType = typeof(T).Name;
        }
        
        public string GetViewPresenterType()
        {
            return _viewPresenterType;
        }
    }
    
    public interface IViewManager
    {
        void LoadView<T>(LoadViewParams<T> loadViewParams, bool closeAllViews = true);
        void DestroyView<T>();
        void DestroyAllViews();
    }
}