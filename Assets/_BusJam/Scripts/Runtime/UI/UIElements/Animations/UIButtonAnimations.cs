using DG.Tweening;
using UnityEngine;

namespace BusJam.Scripts.Runtime.UI.UIElements.Animations
{
    public class UIButtonAnimations : MonoBehaviour
    {
        record ScaleLoopAnimationConfig
        {
            public float speed = 0.8f;
            public Ease ease = Ease.InOutSine;
            public Vector3 targetScale = Vector3.one * 0.95f;
        }
        
        record PointerDownAnimationConfig
        {
            public float speed = 0.25f;
            public Ease ease = Ease.OutBack;
            public Vector3 targetScale = Vector3.one * 0.9f;
        }

        record PointerUpAnimationConfig
        {
            public float speed = 0.25f;
            public Ease ease = Ease.OutBack;
            public Vector3 targetScale = Vector3.one * 1f;
        }

        record PointerEnterAnimationConfig
        {
            public float speed = 0.15f;
            public Ease ease = Ease.OutBack;
            public Vector3 targetScale = Vector3.one * 0.95f;
        }

        record PointerExitAnimationConfig
        {
            public float speed = 0.15f;
            public Ease ease = Ease.OutBack;
            public Vector3 targetScale = Vector3.one * 1f;
        }

        private PointerDownAnimationConfig _pointerDownAnimationConfig;
        private PointerUpAnimationConfig _pointerUpAnimationConfig;
        private PointerEnterAnimationConfig _pointerEnterAnimationConfig;
        private PointerExitAnimationConfig _pointerExitAnimationConfig;
        private ScaleLoopAnimationConfig _scaleLoopAnimationConfig;
        
        private Sequence _loopSequence;

        private void Start()
        {
            _pointerUpAnimationConfig = new PointerUpAnimationConfig();
            _pointerDownAnimationConfig = new PointerDownAnimationConfig();
            _pointerEnterAnimationConfig = new PointerEnterAnimationConfig();
            _pointerExitAnimationConfig = new PointerExitAnimationConfig();
            _scaleLoopAnimationConfig = new ScaleLoopAnimationConfig();
        }

        public void PlayOnPointerDownAnimation()
        {
            transform.DOScale(_pointerDownAnimationConfig.targetScale, _pointerDownAnimationConfig.speed)
                .SetEase(_pointerDownAnimationConfig.ease);
        }

        public void PlayOnPointerUpAnimation()
        {
            transform.DOScale(_pointerUpAnimationConfig.targetScale, _pointerUpAnimationConfig.speed)
                .SetEase(_pointerUpAnimationConfig.ease);
        }

        public void PlayOnPointerEnterAnimation()
        {
            transform.DOScale(_pointerEnterAnimationConfig.targetScale, _pointerEnterAnimationConfig.speed)
                .SetEase(_pointerEnterAnimationConfig.ease);
        }

        public void PlayOnPointerExitAnimation()
        {
            transform.DOScale(_pointerExitAnimationConfig.targetScale, _pointerExitAnimationConfig.speed)
                .SetEase(_pointerExitAnimationConfig.ease);
        }
        
        public void StopLoopAnimation()
        {
            _loopSequence?.Kill();
        }
        
        public void PlayLoopAnimation()
        {
            _loopSequence = DOTween.Sequence();
            _loopSequence.Append(transform.DOScale(_scaleLoopAnimationConfig.targetScale, _scaleLoopAnimationConfig.speed).SetEase(_scaleLoopAnimationConfig.ease)).SetLoops(-1, LoopType.Yoyo);
        }
    }

}