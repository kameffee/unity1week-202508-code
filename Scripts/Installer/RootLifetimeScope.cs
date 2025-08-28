using Unity1week202508.Audio.Data;
using Unity1week202508.Audio.Players;
using Unity1week202508.Audio.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202508.Installer
{
    /// <summary>
    /// 全シーンで共有されるグローバルなLifetimeScope
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Audio System")]
        [SerializeField]
        private AudioDatabase _audioDatabase;

        [SerializeField]
        private AudioPlayerManager _audioPlayerManagerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // 音声システム（全シーンで共有）
            builder.RegisterInstance(_audioDatabase);
            builder.Register<AudioSettingsService>(Lifetime.Singleton);

            // AudioPlayerManagerを登録してBgmPlayer, SePlayerを取得
            builder.RegisterComponentInNewPrefab(_audioPlayerManagerPrefab, Lifetime.Singleton)
                .UnderTransform(transform);

            builder.Register<AudioPlayer>(Lifetime.Singleton)
                .WithParameter<BgmPlayer>(resolver => resolver.Resolve<AudioPlayerManager>().BgmPlayer)
                .WithParameter<SePlayer>(resolver => resolver.Resolve<AudioPlayerManager>().SePlayer);

            // Debugビルド時 or エディタ実行時にのみ、ログを有効
            Debug.unityLogger.logEnabled = Application.isEditor || Debug.isDebugBuild;
        }
    }
}