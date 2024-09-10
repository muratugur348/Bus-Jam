using System.Collections;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Utilities
{
    public class Optimization : MonoBehaviourSingleton<Optimization>
    {
        private Light _sceneLight;
        
        private float[] _frameDeltaTimeArray;
        private float[] _lastFPSValues;
        private bool _isFPSControlDone;
        private int _lastFrameIndex;
        private int _currentIndex;
        private string _fps;

        protected override void Awake()
        {
            base.Awake();
            _sceneLight = FindObjectOfType<Light>();
            _frameDeltaTimeArray = new float[50];
        }

        private IEnumerator Start()
        {
            if (PlayerPrefs.GetInt("isFPSBelowMinValue", 0) == 1)
            {
                _isFPSControlDone = true;
                yield break;
            }

            _lastFPSValues = new float[10];
            yield return new WaitForSeconds(3);
            StartCoroutine(SetFPSValue());
        }

        private void Update()
        {
            if (_isFPSControlDone)
                return;
            _frameDeltaTimeArray[_lastFrameIndex] = Time.deltaTime;
            _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimeArray.Length;
        }

        private float CalculateFPS()
        {
            float total = 0f;
            for (int i = 0; i < _frameDeltaTimeArray.Length; i++)
            {
                total += _frameDeltaTimeArray[i];
            }

            return _frameDeltaTimeArray.Length / total;
        }

        private IEnumerator SetFPSValue()
        {
            yield return new WaitForSeconds(1);
            _lastFPSValues[_currentIndex] = CalculateFPS();
            _currentIndex++;
            if (_currentIndex >= _lastFPSValues.Length)
            {
                _currentIndex = 0;

                CheckFPSValues();
            }
            else
            {
                StartCoroutine(SetFPSValue());
            }
        }

        private void CheckFPSValues()
        {
            _isFPSControlDone = true;
            float total = 0;
            for (int i = 0; i < _lastFPSValues.Length; i++)
            {
                total += _lastFPSValues[i];
            }

            total /= _lastFPSValues.Length;


            if (total < 28)
            {
                _sceneLight.shadows = LightShadows.None;
                PlayerPrefs.SetInt("isFPSBelowMinValue", 1);
                StopAllCoroutines();
            }
        }
    }
}