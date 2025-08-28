using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity1week202508.Audio.Data;
using Unity1week202508.Audio.Players;
using UnityEngine;

namespace Unity1week202508.Audio.Services
{
    /// <summary>
    /// 音声再生を統合管理するサービス
    /// </summary>
    public class AudioPlayer : IDisposable
    {
        private readonly AudioDatabase _audioDatabase;
        private readonly BgmPlayer _bgmPlayer;
        private readonly SePlayer _sePlayer;

        public AudioPlayer(
            AudioDatabase audioDatabase,
            BgmPlayer bgmPlayer,
            SePlayer sePlayer,
            AudioSettingsService audioSettingsService)
        {
            _audioDatabase = audioDatabase;
            _bgmPlayer = bgmPlayer;
            _sePlayer = sePlayer;

            _bgmPlayer.Initialize(audioSettingsService);
            _sePlayer.Initialize(audioSettingsService);
        }

        public void PlayBgm(string bgmId, float fadeInDuration = 1.0f, CancellationToken cancellationToken = default)
        {
            PlayBgmAsync(bgmId, fadeInDuration, cancellationToken).Forget();
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="bgmId">BGM ID</param>
        /// <param name="fadeInDuration">フェードイン時間（秒）</param>
        public async UniTask PlayBgmAsync(string bgmId, float fadeInDuration = 1.0f, CancellationToken cancellationToken = default)
        {
            var bgmData = _audioDatabase.GetAudioClipData(bgmId);
            if (bgmData == null)
            {
                Debug.LogWarning($"BGM not found: {bgmId}");
                return;
            }

            if (bgmData.AudioType != Data.AudioType.Bgm)
            {
                Debug.LogWarning($"AudioClipData '{bgmId}' is not BGM type: {bgmData.AudioType}");
                return;
            }

            await _bgmPlayer.PlayBgmAsync(bgmData.AudioClip, bgmId, fadeInDuration);
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        /// <param name="fadeOutDuration">フェードアウト時間（秒）</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        public async UniTask StopBgmAsync(float fadeOutDuration = 1.0f, CancellationToken cancellationToken = default)
        {
            await _bgmPlayer.StopBgmAsync(fadeOutDuration);
        }

        /// <summary>
        /// BGMを一時停止
        /// </summary>
        public void PauseBgm()
        {
            _bgmPlayer.PauseBgm();
        }

        /// <summary>
        /// 一時停止したBGMを再開
        /// </summary>
        public void ResumeBgm()
        {
            _bgmPlayer.ResumeBgm();
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="seId">SE ID</param>
        /// <param name="volume">個別音量（指定しない場合は設定音量を使用）</param>
        /// <param name="pitch">ピッチ（デフォルト1.0）</param>
        /// <returns>再生に使用されたAudioSource（失敗した場合はnull）</returns>
        public void PlaySe(string seId, float? volume = null, float pitch = 1.0f)
        {
            var seData = _audioDatabase.GetAudioClipData(seId);
            if (seData == null)
            {
                Debug.LogWarning($"SE not found: {seId}");
                return;
            }

            if (seData.AudioType != Data.AudioType.Se)
            {
                Debug.LogWarning($"AudioClipData '{seId}' is not SE type: {seData.AudioType}");
                return;
            }

            _sePlayer.PlaySe(seData.AudioClip, volume, pitch);
        }

        public void PlaySe(AudioClip audioClip, float? volume = null, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                Debug.LogWarning("AudioClip is null");
                return;
            }

            _sePlayer.PlaySe(audioClip, volume, pitch);
        }

        /// <summary>
        /// 指定したSEを停止
        /// </summary>
        /// <param name="seId">SE ID</param>
        public void StopSe(string seId)
        {
            var seData = _audioDatabase.GetAudioClipData(seId);
            if (seData == null)
            {
                Debug.LogWarning($"SE not found: {seId}");
                return;
            }

            _sePlayer.StopSe(seData.AudioClip);
        }

        /// <summary>
        /// 全てのSEを停止
        /// </summary>
        public void StopAllSe()
        {
            _sePlayer.StopAllSe();
        }

        /// <summary>
        /// 現在再生中のBGM ID
        /// </summary>
        public string CurrentBgmId => _bgmPlayer.CurrentBgmId;

        /// <summary>
        /// BGMが再生中かどうか
        /// </summary>
        public bool IsBgmPlaying => _bgmPlayer.IsPlaying;

        public void Dispose()
        {
            _bgmPlayer?.Dispose();
            _sePlayer?.Dispose();
        }
    }
}