using System;
using System.Collections.Generic;
using R3;
using Unity1week202508.Audio.Data;
using Unity1week202508.Audio.Services;
using UnityEngine;

namespace Unity1week202508.Audio.Players
{
    /// <summary>
    /// SE再生を管理するコンポーネント
    /// </summary>
    public class SePlayer : MonoBehaviour, IDisposable
    {
        [SerializeField] 
        private int _maxAudioSources = 10;

        private readonly List<AudioSource> _audioSources = new();
        private readonly CompositeDisposable _disposables = new();
        private AudioVolume _currentVolume = AudioVolume.Default;

        private void Awake()
        {
            // AudioSourceプールを作成
            for (int i = 0; i < _maxAudioSources; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                _audioSources.Add(audioSource);
            }
        }

        /// <summary>
        /// 音量設定サービスを設定
        /// </summary>
        /// <param name="audioSettingsService">音声設定サービス</param>
        public void Initialize(AudioSettingsService audioSettingsService)
        {
            // SE音量の変更を監視
            audioSettingsService.SeVolume
                .Subscribe(volume =>
                {
                    _currentVolume = volume;
                    // 全てのAudioSourceの音量を更新
                    foreach (var audioSource in _audioSources)
                    {
                        audioSource.volume = volume.Value;
                    }
                })
                .AddTo(_disposables);
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="seClip">再生するSEクリップ</param>
        /// <param name="volume">個別音量（指定しない場合は設定音量を使用）</param>
        /// <param name="pitch">ピッチ（デフォルト1.0）</param>
        /// <returns>再生に使用されたAudioSource（失敗した場合はnull）</returns>
        public void PlaySe(AudioClip seClip, float? volume = null, float pitch = 1.0f)
        {
            if (seClip == null)
            {
                Debug.LogWarning("SE clip is null");
                return;
            }

            // 利用可能なAudioSourceを取得
            var audioSource = GetAvailableAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available AudioSource for SE playback");
                return;
            }

            // AudioSourceを設定して再生
            audioSource.clip = seClip;
            audioSource.volume = volume ?? _currentVolume.Value;
            audioSource.pitch = pitch;
            audioSource.Play();
        }

        /// <summary>
        /// 指定したSEクリップと同じ音を全て停止
        /// </summary>
        /// <param name="seClip">停止するSEクリップ</param>
        public void StopSe(AudioClip seClip)
        {
            if (seClip == null) return;

            foreach (var audioSource in _audioSources)
            {
                if (audioSource.clip == seClip && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        /// <summary>
        /// 全てのSEを停止
        /// </summary>
        public void StopAllSe()
        {
            foreach (var audioSource in _audioSources)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        /// <summary>
        /// 利用可能なAudioSourceを取得
        /// </summary>
        /// <returns>利用可能なAudioSource（見つからない場合はnull）</returns>
        private AudioSource GetAvailableAudioSource()
        {
            // 再生していないAudioSourceを探す
            foreach (var audioSource in _audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    return audioSource;
                }
            }

            // 全て再生中の場合は最も古いものを使用（最初の要素）
            return _audioSources.Count > 0 ? _audioSources[0] : null;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}