using Unity1week202508.Manual;
using Unity1week202508.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202508.Installer
{
    public class TitleLifetimeScope : LifetimeScope
    {
        [Header("UI Toolkit Components")]
        [SerializeField]
        private TitleViewController _titleViewController;

        [Header("Manual Dialog")]
        [SerializeField]
        private ManualDialogView _manualDialogViewPrefab;

        [SerializeField]
        private TextAsset _manualTextAsset;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_titleViewController);

            // Manual Dialog
            builder.RegisterComponentInNewPrefab(_manualDialogViewPrefab, Lifetime.Singleton);
            builder.RegisterInstance(_manualTextAsset);
            builder.Register<ManualDialogPresenter>(Lifetime.Singleton);

            // Common Services
            builder.Register<SceneLoaderService>(Lifetime.Scoped);
            builder.RegisterEntryPoint<TitlePresenter>();
        }
    }
}