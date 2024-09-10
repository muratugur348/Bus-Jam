using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters;
using UnityEditor;
using UnityEngine;
using BusJam.Scripts.Runtime.LevelCreator;

namespace BusJam.Scripts.Runtime
{
    public class BusManager : MonoBehaviour
    {
        private ISoundManager _soundManager;

        [Header("Bus References")] public ColorType[] busColors;
        [SerializeField] private GameObject busPrefab;
        public Transform busSpawnPoint;
        public Transform[] busPoints;

        [SerializeField]
        private float openingMovementWaitTime;
        public float busWaitTime;
        public float destroyAfterMovementDelay;

        public int stickmanCapacity;


        [Header("Bus Scale Up Parameters")]
        public float scaleUpTime;

        public Ease scaleUpEase;
        public Vector3 targetScale;

        [Header("Info - No Touch")] public Bus activeBus;
        [SerializeField] public int activeBusIndex;
        [SerializeField] public Bus[] buses;


        private IGameManager GameManager;

        public static BusManager Instance;

        private void Awake()
        {
            MakeSingleton();
        }

        private void MakeSingleton()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public async void Start()
        {
            _soundManager = Locator.Instance.Resolve<ISoundManager>();
            GameManager = Locator.Instance.Resolve<IGameManager>();

            var busAmountInLevel = busColors.Length;

            buses = new Bus[busAmountInLevel];

            for (var i = 0; i < busAmountInLevel; i++)
            {
                var bus = Instantiate(busPrefab.gameObject);
                buses[i] = bus.GetComponent<Bus>();

                buses[i].InitBus(busColors[i]);

                if (i < 2)
                {
                    buses[i].MoveNextLine();
                    await UniTask.WaitForSeconds(openingMovementWaitTime);

                    if (i == 0)
                    {
                        activeBus = buses[i];
                        buses[i].isActiveBus = true;

                        buses[i].MoveNextLine();
                        buses[i].OnBusMoved += MoveNextBus;
                    }
                }
            }
        }

        private void MoveNextBus()
        {
            activeBus.OnBusMoved -= MoveNextBus;
            activeBusIndex++;

            //---LEVEL COMPLETED---//
            if (activeBusIndex == buses.Length)
            {
                _soundManager.PlaySound("win");
                GameManager.CompleteLevel();
                ConfettiManager.Instance.PlayConfettiParticles();
                return;
            }

            activeBus = buses[activeBusIndex];
            activeBus.isActiveBus = true;
            activeBus.OnBusMoved += MoveNextBus;
            activeBus.MoveNextLine();

            if (activeBusIndex != buses.Length - 1) buses[activeBusIndex + 1].MoveNextLine();
        }

        public bool CheckStickmanCanGoToBus(ColorType colorType)
        {

            if (!activeBus) return false;
            return activeBus.IsAvailableAndSameWithTargetStickman(colorType);
        }

        public bool CanNextBusTakeStickmanFromMatchArea()
        {
            foreach (StickmanParent stickmanParent in GridManager.Instance.stickmanParentsOnMatchAreaList)
            {

                if (stickmanParent.colorType.Equals(
                        buses[activeBusIndex + 1]
                            .color))
                    return true;

            }

            return false;
        }
    }
}