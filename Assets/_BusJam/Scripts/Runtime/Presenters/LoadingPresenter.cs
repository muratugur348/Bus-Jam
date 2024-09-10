using DG.Tweening;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BusJam.Scripts.Runtime.Presenters
{
    public class LoadingPresenter : BasePresenter<LoadingPresenter>
    {
        [SerializeField] private Slider progressSlider;
        private LoadingModel _model;

        private new void Start()
        {
            base.Start();
            _model = new LoadingModel();
            float val = 0;
            DOTween.To(() => val, x => val = x, 1, _model.Delay).OnUpdate(() =>
            {
                progressSlider.value = val;
            }).OnComplete(() =>
            {
                SceneManager.LoadScene(Locator.Instance.Resolve<ILevelManager>().GetCurrentLevelIndex());
                //SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
                Close();
            });
        }
    }
}