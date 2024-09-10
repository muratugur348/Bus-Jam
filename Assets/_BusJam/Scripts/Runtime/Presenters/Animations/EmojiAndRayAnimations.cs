using DG.Tweening;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Presenters.Animations
{
    public class EmojiAndRayAnimations : MonoBehaviour
    {
        [SerializeField] private RectTransform ray1;
        [SerializeField] private RectTransform ray2;
        [SerializeField] private RectTransform ray3;
        [SerializeField] private RectTransform emoji;
        
        private Sequence _raySequence;
        private Sequence _emojiMoveSequence;
        private Sequence _emojiRotateSequence;

        [SerializeField] private float radius;
        [SerializeField] private float moveDuration;

        private Vector2 _centerPoint;
        
        private void Start()
        {
            _raySequence = DOTween.Sequence();
            _raySequence.Join(ray1.DORotate(new Vector3(0,0,360f), 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            _raySequence.Join(ray2.DORotate(new Vector3(0,0,360f), 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            _raySequence.Join(ray3.DORotate(new Vector3(0,0,360f), 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            _raySequence.SetLoops(-1).SetRelative();

            _centerPoint = emoji.transform.position;
            MoveToRandomPointOnCircle();
            ShakeEmoji();
        }

        private void ShakeEmoji()
        {
            _emojiRotateSequence = DOTween.Sequence();
            _emojiRotateSequence.SetDelay(0.5f);
            _emojiRotateSequence.Append(emoji.transform.DORotate(new Vector3(0, 0, -2f), 0.2f).SetEase(Ease.Linear));
            _emojiRotateSequence.Append(emoji.transform.DORotate(new Vector3(0, 0, 2f), 0.2f).SetEase(Ease.Linear));
            _emojiRotateSequence.Append(emoji.transform.DORotate(new Vector3(0, 0, -2f), 0.2f).SetEase(Ease.Linear));
            _emojiRotateSequence.Append(emoji.transform.DORotate(new Vector3(0, 0, 2f), 0.2f).SetEase(Ease.Linear));
            _emojiRotateSequence.Append(emoji.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear));
            _emojiRotateSequence.OnComplete(ShakeEmoji);
        }
        
        private void MoveToRandomPointOnCircle()
        {
            float angle = Random.Range(0f, 360f);
            Vector2 targetPoint = _centerPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            _emojiMoveSequence = DOTween.Sequence();
            _emojiMoveSequence.Append(emoji.transform.DOMove(targetPoint, moveDuration).SetEase(Ease.InOutQuad));
            _emojiMoveSequence.OnComplete(MoveToRandomPointOnCircle);
        }
        
        private void OnDestroy()
        {
            _emojiMoveSequence?.Kill();
            _raySequence?.Kill();
        }
    }
}