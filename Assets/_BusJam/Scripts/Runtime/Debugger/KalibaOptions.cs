using System.ComponentModel;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters;
#if !DISABLE_SRDEBUGGER
using SRDebugger;
using SRDebugger.Services;
#endif

namespace BusJam.Scripts.Runtime.Debugger
{
    public class KalibaDebugOptions
    {
        private int _selectedLevelIndex = 1;
        
        [NumberRange(1, 30)]
        [Category("Select Level")]
        public int SelectedLevelIndex 
        {
            get => _selectedLevelIndex;
            set => _selectedLevelIndex = value;
        }

        [Category("Level Test")]
        public void LoadLevel()
        {
            var levelManager = Locator.Instance.Resolve<ILevelManager>();
            levelManager.LoadLevelByIndexForSrDebugger(_selectedLevelIndex);
        }

        [Category("Complete Level")]
        public void CompleteLevel()
        {
            Locator.Instance.Resolve<IGameManager>().ChangeGameState(GameState.LevelComplete);
            Locator.Instance.Resolve<IViewManager>().LoadView(new LoadViewParams<LevelCompletedPresenter>("LevelCompletedView"));
        }
        
        [Category("Fail Level")]
        public void FailLevel()
        {
            Locator.Instance.Resolve<IGameManager>().ChangeGameState(GameState.LevelFailed);
            Locator.Instance.Resolve<IViewManager>().LoadView(new LoadViewParams<LevelCompletedPresenter>("LevelFailedView"));
        }
    }
}