using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Unity1week202508.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity1week202508.Lottery
{
    /// <summary>
    /// クリック可能なクジオブジェクト
    /// </summary>
    public class LotteryObject : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly int AnimationKeyOpen = Animator.StringToHash("Open");

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Collider2D _collider;
        
        [SerializeField]
        private Animator _animator;

        private PrizeMasterData _prizeMasterData;
        private long _uniqueId;
        private readonly Subject<LotteryClickedEventData> _onClickSubject = new();

        private Vector3 _originalScale;

        public Observable<LotteryClickedEventData> OnClickAsObservable() => _onClickSubject;

        private void Awake()
        {
            _originalScale = _spriteRenderer.transform.localScale;
        }

        public void Initialize(PrizeMasterData prizeData, long uniqueId)
        {
            _prizeMasterData = prizeData;
            _uniqueId = uniqueId;
            gameObject.name = $"LotteryObject_{uniqueId}_{prizeData.Id}";
        }

        public void SetInteractable(bool interactable)
        {
            _collider.enabled = interactable;
        }

        private void OnDestroy()
        {
            _onClickSubject?.Dispose();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_prizeMasterData == null) return;

            Debug.Log($"クジクリック: UniqueID={_uniqueId}, PrizeID={_prizeMasterData.Id}");
            _onClickSubject.OnNext(new LotteryClickedEventData(_uniqueId, this, _prizeMasterData));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_prizeMasterData == null) return;

            // 少し拡大してハイライト
            _spriteRenderer.transform.localScale = _originalScale * 1.1f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_prizeMasterData == null) return;

            // 元のサイズに戻す
            _spriteRenderer.transform.localScale = _originalScale;
        }

        public void PlayInAnimation()
        {
            transform.localScale = Vector3.zero;
            // 入場アニメーションを再生
            LMotion.Create(Vector3.zero, Vector3.one, 0.2f)
                .WithEase(Ease.OutBack)
                .BindToLocalScale(transform)
                .AddTo(this);
        }

        public async UniTask PlayOutAnimationAsync(CancellationToken cancellation)
        {
            _animator.SetTrigger(AnimationKeyOpen);
            await UniTask.Delay(TimeSpan.FromSeconds(1.333f), cancellationToken: cancellation);
        }
    }
}