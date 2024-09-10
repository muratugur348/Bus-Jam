using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;

namespace BusJam.Scripts.Runtime.Models
{
    public class LevelFailedModel: BaseModel<LevelFailedModel>
    {
        private readonly ILevelManager _levelManager;
        private readonly ISoundManager _soundManager;
        
        public LevelFailedModel()
        {
            _levelManager = Locator.Instance.Resolve<ILevelManager>();
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            _soundManager.PlaySound("level_failed");
        }
        
        public void RestartLevel() => _levelManager.RestartLevel();
    }
}