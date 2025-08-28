using System;
using System.Linq;
using R3;
using Unity1week202508.Data;
using Unity1week202508.Extensions;
using VContainer.Unity;

namespace Unity1week202508.InGame.UI
{
    /// <summary>
    /// 景品コレクションのPresenter
    /// </summary>
    public class PrizeCollectionPresenter : Presenter, IStartable
    {
        private readonly InGameUIController _inGameUiController;
        private readonly PrizeCollectionDialogView _collectionDialogView;
        private readonly PrizeMasterDataRepository _prizeRepository;
        private readonly PrizeAcquisitionRepository _acquisitionRepository;

        public PrizeCollectionPresenter(
            InGameUIController inGameUiController,
            PrizeCollectionDialogView collectionDialogView,
            PrizeMasterDataRepository prizeRepository,
            PrizeAcquisitionRepository acquisitionRepository)
        {
            _inGameUiController = inGameUiController;
            _collectionDialogView = collectionDialogView;
            _prizeRepository = prizeRepository;
            _acquisitionRepository = acquisitionRepository;
        }

        public void Start()
        {
            // コレクションボタンのクリックイベントを購読
            _inGameUiController.OnClickCollectionButtonAsObservable()
                .Subscribe(_ =>
                {
                    UpdatePrizeDisplay();
                    _collectionDialogView.ShowDialog();
                })
                .AddTo(this);

            // マニュアルボタンのクリックイベントを購読
            _inGameUiController.OnClickManualButtonAsObservable()
                .Subscribe(_ => { _inGameUiController.ShowManualDialog(); })
                .AddTo(this);
        }

        /// <summary>
        /// 景品表示を更新
        /// </summary>
        private void UpdatePrizeDisplay()
        {
            // 全景品データを取得
            var allPrizeMasterDatas = _prizeRepository.GetAll();

            // DisplayOrder順にソート（同じ値の場合はID順）
            var sortedPrizes = allPrizeMasterDatas
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Id)
                .ToList();

            // 獲得済み景品IDを取得
            var acquiredPrizeIds = _acquisitionRepository.GetAcquiredPrizeIds();

            // Viewを更新
            _collectionDialogView.UpdatePrizeList(sortedPrizes, acquiredPrizeIds);
        }
    }
}