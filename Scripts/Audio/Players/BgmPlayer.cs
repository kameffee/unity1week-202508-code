using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Unity1week202508.Audio.Services;
using UnityEngine;

namespace Unity1week202508.Audio.Players
{
    /// <summary>
    /// BGM再生を管理するコンポーネント
    /// </summary>
    public class BgmPlayer : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private AudioSource _audioSource;

        private readonly CompositeDisposable _disposables = new();
        private MotionHandle _fadeMotion;

        /// <summary>
        /// 現在再生中のBGM ID
        /// </summary>
        public string CurrentBgmId { get; private set; }

        /// <summary>
        /// BGMが再生中かどうか
        /// </summary>
        public bool IsPlaying => _audioSource != null && _audioSource.isPlaying;

        private void Awake()
        {
            // AudioSourceが設定されていない場合は自動作成
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            // BGM用の設定
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
        }

        /// <summary>
        /// 音量設定サービスを設定
        /// </summary>
        /// <param name="audioSettingsService">音声設定サービス</param>
        public void Initialize(AudioSettingsService audioSettingsService)
        {
            // BGM音量の変更を監視
            audioSettingsService.BgmVolume
                .Subscribe(volume => _audioSource.volume = volume.Value)
                .AddTo(_disposables);
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="bgmClip">再生するBGMクリップ</param>
        /// <param name="bgmId">BGM ID</param>
        /// <param name="fadeInDuration">フェードイン時間（秒）</param>
        public async UniTask PlayBgmAsync(AudioClip bgmClip, string bgmId, float fadeInDuration = 1.0f)
        {
            if (bgmClip == null)
            {
                Debug.LogWarning($"BGM clip is null for ID: {bgmId}");
                return;
            }

            // 既に同じBGMが再生中の場合は何もしない
            if (CurrentBgmId == bgmId && IsPlaying)
            {
                return;
            }

            // 現在のBGMをフェードアウトしてから新しいBGMを再生
            if (IsPlaying)
            {
                await FadeOutAsync(0.5f);
            }

            CurrentBgmId = bgmId;
            _audioSource.clip = bgmClip;
            _audioSource.Play();

            // フェードイン
            if (fadeInDuration > 0)
            {
                await FadeInAsync(fadeInDuration);
            }
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        /// <param name="fadeOutDuration">フェードアウト時間（秒）</param>
        public async UniTask StopBgmAsync(float fadeOutDuration = 1.0f)
        {
            if (!IsPlaying) return;

            if (fadeOutDuration > 0)
            {
                await FadeOutAsync(fadeOutDuration);
            }

            _audioSource.Stop();
            CurrentBgmId = null;
        }

        /// <summary>
        /// BGMを一時停止
        /// </summary>
        public void PauseBgm()
        {
            if (IsPlaying)
            {
                _audioSource.Pause();
            }
        }

        /// <summary>
        /// 一時停止したBGMを再開
        /// </summary>
        public void ResumeBgm()
        {
            _audioSource.UnPause();
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        private async UniTask FadeInAsync(float duration)
        {
            if (_fadeMotion.IsActive())
            {
                _fadeMotion.Cancel();
            }

            var startVolume = 0f;
            var targetVolume = _audioSource.volume;

            _audioSource.volume = startVolume;

            var motion = LMotion.Create(startVolume, targetVolume, duration)
                .Bind(volume => _audioSource.volume = volume)
                .AddTo(this);
            _fadeMotion = motion;

            await motion.ToUniTask();
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        private async UniTask FadeOutAsync(float duration)
        {
            if (_fadeMotion.IsActive())
            {
                _fadeMotion.Cancel();
            }

            var startVolume = _audioSource.volume;
            var targetVolume = 0f;

            var motion = LMotion.Create(startVolume, targetVolume, duration)
                .Bind(volume => _audioSource.volume = volume);
            _fadeMotion = motion;

            await motion.ToUniTask();
        }

        public void Dispose()
        {
            if (_fadeMotion.IsActive())
                _fadeMotion.Cancel();

            _disposables?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}