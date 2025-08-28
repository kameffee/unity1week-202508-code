using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202508.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity1week202508.Lottery
{
    public class PrizeAcquisitionDialogView : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _uiDocument;

        [SerializeField]
        private float _fadeInDuration = 0.3f;

        private VisualElement _modalOverlay;
        private VisualElement _window;
        private VisualElement _prizeImage;
        private Label _prizeNameLabel;
        private Button _okButton;

        private readonly Subject<Unit> _onClickOkSubject = new();

        private void Awake()
        {
            // UI要素の取得
            var root = _uiDocument.rootVisualElement;
            _modalOverlay = root.Q<VisualElement>("modal-overlay");
            _window = root.Q<VisualElement>("prize-acquisition-window");
            _prizeImage = root.Q<VisualElement>("prize-image");
            _prizeNameLabel = root.Q<Label>("prize-name");
            _okButton = root.Q<Button>("ok-button");

            // ボタンのイベント設定
            _okButton.clicked += () => _onClickOkSubject.OnNext(Unit.Default);

            // 初期状態は非表示
            Hide();
        }

        private void OnDestroy()
        {
            _onClickOkSubject?.Dispose();
        }

        public Observable<Unit> OnClickOkAsObservable() => _onClickOkSubject;

        public void SetPrizeData(PrizeMasterData prizeData)
        {
            Assert.IsNotNull(prizeData);
            Assert.IsNotNull(prizeData.Image);

            // 景品画像の設定
            _prizeImage.style.backgroundImage = new StyleBackground(prizeData.Image);

            // 景品名の設定
            _prizeNameLabel.text = prizeData.PrizeName;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            // モーダルを表示
            _modalOverlay.style.display = DisplayStyle.Flex;

            // 初期状態を設定（透明・縮小）
            _modalOverlay.style.opacity = 0f;
            _window.style.scale = new Scale(new Vector3(0.8f, 0.8f, 1f));

            // 少し待機
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cancellationToken);

            // フェードインとスケールアニメーション
            float elapsedTime = 0f;
            while (elapsedTime < _fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / _fadeInDuration);

                // イージング関数（ease-out-back）
                float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
                float scaleProgress = easedProgress * 1.02f - 0.02f * Mathf.Sin(easedProgress * Mathf.PI);

                _modalOverlay.style.opacity = progress;
                float scale = Mathf.Lerp(0.8f, 1f, scaleProgress);
                _window.style.scale = new Scale(new Vector3(scale, scale, 1f));

                await UniTask.Yield(cancellationToken);
            }

            // 最終値を設定
            _modalOverlay.style.opacity = 1f;
            _window.style.scale = new Scale(Vector3.one);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            // フェードアウト
            float elapsedTime = 0f;
            float fadeOutDuration = 0.2f;

            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeOutDuration);
                _modalOverlay.style.opacity = 1f - progress;

                await UniTask.Yield(cancellationToken);
            }

            Hide();
        }

        private void Hide()
        {
            _modalOverlay.style.display = DisplayStyle.None;
            _modalOverlay.style.opacity = 1f;
            _window.style.scale = new Scale(Vector3.one);
        }
    }
}