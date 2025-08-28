using System;
using R3;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity1week202508.Manual
{
    /// <summary>
    /// マニュアルダイアログのViewクラス
    /// </summary>
    public class ManualDialogView : MonoBehaviour, IDisposable
    {
        [Header("UI Document")]
        [SerializeField]
        private UIDocument _uiDocument;

        // UI Elements
        private VisualElement _modalOverlay;
        private Button _closeButton;
        private Button _footerCloseButton;
        private Label _manualText;

        // Observable
        private readonly Subject<Unit> _onCloseClickedSubject = new();
        private readonly CompositeDisposable _disposables = new();

        public Observable<Unit> OnCloseClickedAsObservable() => _onCloseClickedSubject;

        private void Awake()
        {
            InitializeUI();
            SetupEventHandlers();
            Hide(); // 初期状態は非表示
        }

        /// <summary>
        /// UI要素を初期化
        /// </summary>
        private void InitializeUI()
        {
            var root = _uiDocument.rootVisualElement;

            _modalOverlay = root.Q<VisualElement>("modal-overlay");
            _closeButton = root.Q<Button>("close-button");
            _footerCloseButton = root.Q<Button>("footer-close-button");
            _manualText = root.Q<Label>("manual-text");
            Assert.IsNotNull(_modalOverlay, "Modal overlay not found in UI Document");
            Assert.IsNotNull(_closeButton, "Close button not found in UI Document");
            Assert.IsNotNull(_footerCloseButton, "Footer close button not found in UI Document");
            Assert.IsNotNull(_manualText, "Manual text label not found in UI Document");
        }

        /// <summary>
        /// イベントハンドラーを設定
        /// </summary>
        private void SetupEventHandlers()
        {
            _closeButton.RegisterCallback<ClickEvent>(_ => OnCloseClicked());
            _footerCloseButton.RegisterCallback<ClickEvent>(_ => OnCloseClicked());

            // モーダルオーバーレイの背景クリックで閉じる
            _modalOverlay.RegisterCallback<ClickEvent>(evt =>
            {
                // モーダルウィンドウ内のクリックは無視
                if (evt.target == _modalOverlay)
                {
                    OnCloseClicked();
                }
            });
        }

        /// <summary>
        /// マニュアルテキストを設定
        /// </summary>
        /// <param name="text">マニュアルテキスト</param>
        public void SetManualText(string text)
        {
            _manualText.text = text;
        }

        /// <summary>
        /// マニュアルダイアログを表示
        /// </summary>
        public void Show()
        {
            _modalOverlay.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// マニュアルダイアログを非表示
        /// </summary>
        public void Hide()
        {
            _modalOverlay.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 表示状態を取得
        /// </summary>
        /// <returns>表示中の場合true</returns>
        public bool IsVisible()
        {
            return _modalOverlay != null && _modalOverlay.style.display == DisplayStyle.Flex;
        }

        /// <summary>
        /// 閉じるボタンがクリックされた時の処理
        /// </summary>
        private void OnCloseClicked()
        {
            Hide();
            _onCloseClickedSubject.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _onCloseClickedSubject?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}