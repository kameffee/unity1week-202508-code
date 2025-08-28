using System;
using System.Collections.Generic;
using R3;
using Unity1week202508.Data;
using Unity1week202508.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity1week202508.InGame.UI
{
    /// <summary>
    /// 景品コレクションダイアログのView
    /// </summary>
    public class PrizeCollectionDialogView : MonoBehaviour, IDisposable
    {
        [Header("UI Document")]
        [SerializeField]
        private UIDocument _uiDocument;
        
        [Header("UI Templates")]
        [SerializeField]
        private VisualTreeAsset _prizeItemTemplate;
        
        [SerializeField]
        private StyleSheet _prizeItemStyleSheet;

        // UI Elements
        private VisualElement _modalOverlay;
        private VisualElement _prizeCollectionWindow;
        private Button _closeButton;
        private Button _footerCloseButton;
        private Label _prizeCountLabel;
        private VisualElement _prizeGrid;

        // Observables
        private readonly Subject<Unit> _onShowRequestedSubject = new();
        private readonly CompositeDisposable _disposable = new();

        private void Awake()
        {
            InitializeUI();
            SetupEventHandlers();
        }

        /// <summary>
        /// UI要素を初期化
        /// </summary>
        private void InitializeUI()
        {
            var root = _uiDocument.rootVisualElement;
            Assert.IsNotNull(root, "Root VisualElement is null");

            // UI Elements取得
            _modalOverlay = root.Q<VisualElement>("modal-overlay");
            _prizeCollectionWindow = root.Q<VisualElement>("prize-collection-window");
            _closeButton = root.Q<Button>("close-button");
            _footerCloseButton = root.Q<Button>("footer-close-button");
            _prizeCountLabel = root.Q<Label>("prize-count-label");
            _prizeGrid = root.Q<VisualElement>("prize-grid");

            // 初期状態は非表示
            _modalOverlay?.SetVisible(false);
        }

        /// <summary>
        /// イベントハンドラーを設定
        /// </summary>
        private void SetupEventHandlers()
        {
            // 閉じるボタン
            _closeButton.OnClickAsObservable()
                .Subscribe(_ => HideDialog())
                .AddTo(this);

            _footerCloseButton.OnClickAsObservable()
                .Subscribe(_ => HideDialog())
                .AddTo(this);

            // モーダル背景クリックで閉じる
            _modalOverlay.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target == _modalOverlay)
                {
                    HideDialog();
                }
            });
        }

        /// <summary>
        /// 景品リストを更新
        /// </summary>
        /// <param name="allPrizes">全景品データ</param>
        /// <param name="acquiredPrizeIds">獲得済み景品ID</param>
        public void UpdatePrizeList(IReadOnlyList<PrizeMasterData> allPrizes, IReadOnlyList<int> acquiredPrizeIds)
        {
            // グリッドをクリア
            _prizeGrid.Clear();

            // 獲得済みIDのHashSetを作成（高速検索用）
            var acquiredSet = new HashSet<int>(acquiredPrizeIds);

            // 景品アイテムを生成
            foreach (var prize in allPrizes)
            {
                var prizeItem = CreatePrizeItem(prize, acquiredSet.Contains(prize.Id));
                _prizeGrid.Add(prizeItem);
            }

            // 獲得数を更新
            UpdatePrizeCount(acquiredPrizeIds.Count, allPrizes.Count);
        }

        /// <summary>
        /// 景品アイテムを作成
        /// </summary>
        private VisualElement CreatePrizeItem(PrizeMasterData prize, bool isAcquired)
        {
            if (_prizeItemTemplate == null)
            {
                Debug.LogError("PrizeItemTemplate が設定されていません");
                return new VisualElement();
            }

            // テンプレートからアイテムを生成
            var itemElement = _prizeItemTemplate.CloneTree();
            var prizeItemRoot = itemElement.Q<VisualElement>("prize-item");
            
            if (prizeItemRoot == null)
            {
                Debug.LogError("prize-item要素が見つかりません");
                return itemElement;
            }

            // PrizeItemViewを作成してデータを設定
            var prizeItemView = new PrizeItemView(prizeItemRoot);
            prizeItemView.SetPrizeData(prize, isAcquired);

            return prizeItemRoot;
        }

        /// <summary>
        /// 獲得数表示を更新
        /// </summary>
        private void UpdatePrizeCount(int acquired, int total)
        {
            if (_prizeCountLabel != null)
            {
                _prizeCountLabel.text = $"獲得済み: {acquired} / {total}";
            }
        }

        /// <summary>
        /// ダイアログを表示
        /// </summary>
        public void ShowDialog()
        {
            _modalOverlay.SetVisible(true);
            _prizeCollectionWindow?.FadeIn();
        }

        /// <summary>
        /// ダイアログを非表示
        /// </summary>
        private void HideDialog()
        {
            _prizeCollectionWindow.FadeOut(onComplete: () => _modalOverlay?.SetVisible(false));
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _onShowRequestedSubject.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}