
namespace BusJam.Scripts.Runtime.Models
{
    public class LoadingModel : BaseModel<LoadingModel>
    {
        public float Delay { get;}
        
        public LoadingModel()
        {
            Delay = 1;
        }
    }
}