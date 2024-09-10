using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.InputManager
{
    public interface IInputManager
    {
        public bool ShouldProcessInput();
        public void ProcessRaycastInteraction();

        public bool TrySphereCast(Ray ray, out RaycastHit hitInfo, LayerMask layer);
        public void TrySelectStickman(RaycastHit hitInfo);
    }
}