using DG.Tweening;
using Services.ObjectPool;
using TMPro;
using UnityEngine;

namespace Gameplay.GUIWidgets
{
    public class FloatingNumberText : BasePoolDestroyable
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _lifetime = 0.7f;
        [SerializeField] private float _floatHeight = 0.75f;
        [SerializeField] private float _sideDrift = 0.08f;
        [SerializeField] private float _settledScale = 0.55f;
        [SerializeField] private float _visibleAlpha = 1f;
        [SerializeField] private float _hiddenAlpha = 0f;
        [SerializeField] private float _popDuration = 0.14f;
        [SerializeField] private float _popHeight = 0.2f;
        [SerializeField] private float _holdDuration = 0.06f;
        [SerializeField] private float _fadeDurationOffset = 0.2f;
        [SerializeField] private float _finalScaleMultiplier = 0.85f;

        private Sequence _sequence;

        private void OnDisable()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        public void Show(Vector3 position, string value, Color color)
        {
            _sequence?.Kill();
            _sequence = null;

            transform.position = position;
            transform.localScale = Vector3.zero;

            _text.text = value;
            _text.color = new Color(color.r, color.g, color.b, _visibleAlpha);

            Vector2 drift = Random.insideUnitCircle * _sideDrift;
            Vector3 endPosition = position + new Vector3(drift.x, _floatHeight, drift.y);
            Color transparentColor = new Color(color.r, color.g, color.b, _hiddenAlpha);
            float fadeDuration = Mathf.Max(0f, _lifetime - _fadeDurationOffset);

            _sequence = DOTween.Sequence();

            _sequence.Append(transform.DOScale(_settledScale, _popDuration).SetEase(Ease.OutBack));
            _sequence.Join(transform.DOMove(position + Vector3.up * _popHeight, _popDuration).SetEase(Ease.OutQuad));
            _sequence.AppendInterval(_holdDuration);
            _sequence.Append(transform.DOMove(endPosition, fadeDuration).SetEase(Ease.OutCubic));
            _sequence.Join(DOTween.To(() => _text.color, value => _text.color = value, transparentColor, fadeDuration)
                .SetEase(Ease.InQuad));
            _sequence.Join(transform.DOScale(_settledScale * _finalScaleMultiplier, fadeDuration).SetEase(Ease.InSine));
            _sequence.OnComplete(DestroyObject);
        }
    }
}
