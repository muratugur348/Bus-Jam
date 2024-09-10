using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using UnityEngine;
using BusJam.Scripts.Runtime.LevelCreator;

namespace BusJam.Scripts.Runtime
{
    public class Bus : MonoBehaviour
    {
        public GameObject[] stickmanPositions;
        public List<StickmanParent> stickmanParents = new List<StickmanParent>();
        public GameObject doorEnterPoint;
        public GameObject doorLeft, doorRight;
        private Vector3 _doorLeftStartPos, _doorRightStartPos;

        [Header("Colored Textures\nOrder is important and must be same with the StackColorType enum's order")]
        [SerializeField]
        private GameColors gameColors;

        [Header("References")]

        [SerializeField] private MeshRenderer[] _meshRenderers;

        private Vector3 _middlePosition;
        private ISoundManager _soundManager;
        private IHapticManager _hapticManager;

        [Header("Movement Parameters")]
        [SerializeField]
        private float movementSpeed;

        [SerializeField] private Ease movementEase;


        [Header("Info - No Touch")] public bool isActiveBus;
        public ColorType color;
        public int currentStickmanAmount;
        [SerializeField] private int currentPositionIndex;

        public event Action OnBusMoved;

        private void Start()
        {
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            _hapticManager = Locator.Instance.Resolve<IHapticManager>();
            _middlePosition = BusManager.Instance.busPoints[2].position;

            _doorLeftStartPos = doorLeft.transform.localPosition;
            _doorRightStartPos = doorRight.transform.localPosition;
        }

        public void InitBus(ColorType _color)
        {
            color = _color;
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                meshRenderer.material = gameColors.ActiveMaterials[(int)_color - 2];
                meshRenderer.material.SetFloat("_OutlineSize", 0);
            }


            transform.position = BusManager.Instance.busSpawnPoint.position;
        }

        public void AddStickman(StickmanParent stickman)
        {
            stickmanParents.Add(stickman);
            currentStickmanAmount++;
            _soundManager.PlaySound("sit" + currentStickmanAmount);
            DoorAnimation();
            if (currentStickmanAmount == BusManager.Instance.stickmanCapacity)
            {
                BusManager.Instance.activeBus = null;
                RunMovementLogic();
            }
        }
        private void DoorAnimation()
        {
            doorLeft.transform.DOKill();
            doorRight.transform.DOKill();

            Vector3 leftDoorTargetPos = doorLeft.transform.localPosition;
            leftDoorTargetPos.z = 1.12f;

            Vector3 rightDoorTargetPos = doorLeft.transform.localPosition;
            rightDoorTargetPos.z = -1.43f;

            doorLeft.transform.DOLocalMove(leftDoorTargetPos, 0.25f);
            doorRight.transform.DOLocalMove(rightDoorTargetPos, 0.25f).OnComplete(() =>
            {
                doorLeft.transform.DOLocalMove(_doorLeftStartPos, 0.25f);
                doorRight.transform.DOLocalMove(_doorRightStartPos, 0.25f);
            });
        }

        private async void RunMovementLogic()
        {

            _hapticManager.TriggerHaptic(HapticType.MediumImpact);
      
            Vector3 startScale = transform.localScale;
            transform.DOScale(startScale * 1.05f, .2f).SetDelay(0.5f).OnComplete(() =>
            {
                transform.DOScale(startScale, .2f);
            });
            await UniTask.WaitForSeconds(BusManager.Instance.busWaitTime);
            BusManager.Instance.activeBus = this;
            OnBusMoved?.Invoke();
            _soundManager.PlaySound("explosion");
            transform.DOMoveX(transform.position.x + 15, BusManager.Instance.destroyAfterMovementDelay)
                .OnComplete(
                    () => { Destroy(gameObject, BusManager.Instance.destroyAfterMovementDelay); });
        }

        public async void MoveNextLine()
        {
            BusManager.Instance.activeBus = null;
            currentPositionIndex++;

            await transform
                .DOMove(BusManager.Instance.busPoints[currentPositionIndex].position, movementSpeed)
                .SetEase(movementEase)
                .SetSpeedBased();

            if (currentPositionIndex == 2)
            {
              
                BusManager.Instance.activeBus = this;
                GridManager.Instance.TryToMoveStickmanToBus(color);
              
            }
        }

        public bool IsAvailableAndSameWithTargetStickman(ColorType stackColorType)
        {
            if (currentStickmanAmount >= 3)
                return false;
            return stackColorType == color;
        }
    }
}