using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters.GameplayPresenter
{
    public class CurrencyHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinLabel;
        
        private int _currentCoin;
        
        public void SetCoin(int coin)
        {
            _currentCoin = coin;
            coinLabel.text = $"{_currentCoin}";
        }

        public void IncreaseCoinWithAnimation(int coin)
        {
            DOTween.To(() => _currentCoin, x => _currentCoin = x, coin, 0.5f)
                .OnUpdate(() => coinLabel.text = $"{_currentCoin}")
                .SetEase(Ease.OutBounce);
        }
    }
}