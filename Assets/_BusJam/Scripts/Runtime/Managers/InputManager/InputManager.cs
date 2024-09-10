using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using BusJam.Scripts.Runtime.Managers.LevelManager;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;

namespace BusJam.Scripts.Runtime.Managers.InputManager
{
    public class InputManager : MonoBehaviour
    {
        public LayerMask stickmanLayer;
        public float selectRadius;


        private Camera _mainCam;

        private ILevelManager _levelManager;
        private IGameManager _gameManager;

        private void Start()
        {
            _mainCam = Camera.main;
            _levelManager = Locator.Instance.Resolve<ILevelManager>();
            _gameManager = Locator.Instance.Resolve<IGameManager>();
        }

        void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_gameManager.CurrentGameState != GameState.Gameplay) return;
            //if (Rubber.currentRubberNumberOnMove + GridManager.Instance.rubbersOnMatchAreaList.Count == GridManager.Instance.gridList.Count) return;
            //if (!ShouldProcessInput()) return;

            ProcessRaycastInteraction();
        }

        public bool ShouldProcessInput()
        {
            // Check if the game is in a state where it should process input
            return _levelManager.isGamePlayable &&
                   !_levelManager.isInMenu && !_levelManager.isGameFinished;
            // Removed the commented out code for clarity. If needed, restore it with proper condition.
        }


        public void ProcessRaycastInteraction()
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);


            // Check for interactions with RotateArea
            if (TrySphereCast(ray, out RaycastHit hitInfo, stickmanLayer))
            {
                TrySelectStickman(hitInfo);
            }
        }

        public bool TrySphereCast(Ray ray, out RaycastHit hitInfo, LayerMask layer)
        {
            // Perform the sphere cast and return if it hit something
            return Physics.SphereCast(ray, selectRadius, out hitInfo, 100, layer);
        }

        public void TrySelectStickman(RaycastHit hitInfo)
        {
            // Attempt to select a HookObject if hit
            if (hitInfo.transform.TryGetComponent(out StickmanParent stickmanParent))
            {
                stickmanParent.Selected();
            }
        }
    }
}