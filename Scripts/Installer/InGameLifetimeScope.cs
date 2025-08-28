using Unity1week202508.Data;
using Unity1week202508.InGame;
using Unity1week202508.InGame.UI;
using Unity1week202508.Lottery;
using Unity1week202508.Manual;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202508.Installer
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private PrizeAcquisitionDialogView _prizeAcquisitionDialogViewPrefab;

        [SerializeField]
        private LotteryFieldManager _lotteryFieldManagerPrefab;

        [SerializeField]
        private PrizeMasterDataSource _prizeMasterDataSource;

        [Header("UI Components")]
        [SerializeField]
        private PrizeCollectionDialogView _prizeCollectionDialogViewPrefab;

        [Header("Manual Dialog")]
        [SerializeField]
        private ManualDialogView _manualDialogViewPrefab;

        [SerializeField]
        private TextAsset _manualTextAsset;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InGameLoop>();

            // データ層
            builder.RegisterInstance(_prizeMasterDataSource);
            builder.Register<PrizeMasterDataRepository>(Lifetime.Singleton);
            builder.Register<PrizeAcquisitionRepository>(Lifetime.Singleton);
            builder.Register<LotteryRandomSelector>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            // フィールド管理
            builder.RegisterComponentInNewPrefab(_lotteryFieldManagerPrefab, Lifetime.Singleton)
                .UnderTransform(transform);

            // サービス層
            builder.Register<LotterySelectionService>(Lifetime.Singleton);

            // フェーズ管理
            builder.Register<LotteryResultPhase>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_prizeAcquisitionDialogViewPrefab, Lifetime.Singleton);

            // UI Components
            builder.RegisterComponentInHierarchy<InGameUIController>();
            builder.RegisterComponentInNewPrefab(_prizeCollectionDialogViewPrefab, Lifetime.Singleton);

            // Manual Dialog
            builder.RegisterComponentInNewPrefab(_manualDialogViewPrefab, Lifetime.Singleton);
            builder.RegisterInstance(_manualTextAsset);
            builder.Register<ManualDialogPresenter>(Lifetime.Singleton);

            builder.RegisterEntryPoint<PrizeCollectionPresenter>();
        }
    }
}