using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202508.Audio.Services;
using Unity1week202508.Manual;
using UnityEngine;
using VContainer.Unity;

namespace Unity1week202508.Title
{
    /// <summary>
    /// タイトル画面のプレゼンター（ビジネスロジック）
    /// </summary>
    public class TitlePresenter : IAsyncStartable, IDisposable
    {
        private readonly TitleViewController _titleViewController;
        private readonly SceneLoaderService _sceneLoader;
        private readonly AudioPlayer _audioPlayer;
        private readonly AudioSettingsService _audioSettingsService;
        private readonly ManualDialogPresenter _manualDialogPresenter;
        private readonly CompositeDisposable _disposables = new();

        private bool _isTransitioning;

        public TitlePresenter(
            TitleViewController titleViewController,
            SceneLoaderService sceneLoader,
            AudioPlayer audioPlayer,
            AudioSettingsService audioSettingsService,
            ManualDialogPresenter manualDialogPresenter)
        {
            _titleViewController = titleViewController;
            _sceneLoader = sceneLoader;
            _audioPlayer = audioPlayer;
            _audioSettingsService = audioSettingsService;
            _manualDialogPresenter = manualDialogPresenter;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            // 初期状態設定
            _isTransitioning = false;

            const string mainBgmId = "main-bgm";
            if (_audioPlayer.CurrentBgmId != mainBgmId)
            {
                _audioPlayer.PlayBgm(mainBgmId);
            }

            // ViewControllerに依存関係を注入
            _titleViewController.Initialize(_audioPlayer, _audioSettingsService, _manualDialogPresenter);

            // スタートボタンのイベントを購読
            _titleViewController.OnStartButtonClickedAsObservable()
                .Where(_ => !_isTransitioning)
                .Subscribe(_ => OnStartButtonClicked(cancellation).Forget())
                .AddTo(_disposables);

            Debug.Log("タイトル画面初期化完了 (UI Toolkit版)");

            await UniTask.CompletedTask;
        }

        private async UniTaskVoid OnStartButtonClicked(CancellationToken cancellationToken)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;
            Debug.Log("ゲーム開始");

            // Viewのボタンを無効化
            _titleViewController.SetButtonsEnabled(false);

            try
            {
                // シーン遷移前にDisposableをクリーンアップ
                _disposables?.Dispose();

                // InGameシーンへ遷移
                await _sceneLoader.LoadInGameSceneAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // キャンセル時の処理
                _isTransitioning = false;
                _titleViewController.SetButtonsEnabled(true);
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}