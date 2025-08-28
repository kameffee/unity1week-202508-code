using R3;
using Unity1week202508.Audio.Data;
using UnityEngine;

namespace Unity1week202508.Audio.Services
{
    /// <summary>
    /// 音声設定を管理するサービス
    /// </summary>
    public class AudioSettingsService
    {
        /// <summary>
        /// BGM音量の監視可能プロパティ
        /// </summary>
        public ReadOnlyReactiveProperty<AudioVolume> BgmVolume => _bgmVolume;

        /// <summary>
        /// SE音量の監視可能プロパティ
        /// </summary>
        public ReadOnlyReactiveProperty<AudioVolume> SeVolume => _seVolume;

        private readonly ReactiveProperty<AudioVolume> _bgmVolume = new(AudioVolume.DefaultBgm);
        private readonly ReactiveProperty<AudioVolume> _seVolume = new(AudioVolume.DefaultSe);

        public AudioSettingsService()
        {
            // PlayerPrefsから設定を読み込み
            LoadSettings();
        }

        /// <summary>
        /// BGM音量を設定
        /// </summary>
        /// <param name="volume">音量</param>
        public void SetBgmVolume(AudioVolume volume)
        {
            _bgmVolume.Value = volume;
            SaveBgmVolume();
        }

        /// <summary>
        /// SE音量を設定
        /// </summary>
        /// <param name="volume">音量</param>
        public void SetSeVolume(AudioVolume volume)
        {
            _seVolume.Value = volume;
            SaveSeVolume();
        }

        /// <summary>
        /// 設定をPlayerPrefsから読み込み
        /// </summary>
        private void LoadSettings()
        {
            var bgmVolume = PlayerPrefs.GetFloat("BgmVolume", AudioVolume.DefaultBgm.Value);
            var seVolume = PlayerPrefs.GetFloat("SeVolume", AudioVolume.DefaultSe.Value);

            _bgmVolume.Value = new AudioVolume(bgmVolume);
            _seVolume.Value = new AudioVolume(seVolume);
        }

        /// <summary>
        /// BGM音量をPlayerPrefsに保存
        /// </summary>
        private void SaveBgmVolume()
        {
            PlayerPrefs.SetFloat("BgmVolume", _bgmVolume.Value.Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// SE音量をPlayerPrefsに保存
        /// </summary>
        private void SaveSeVolume()
        {
            PlayerPrefs.SetFloat("SeVolume", _seVolume.Value.Value);
            PlayerPrefs.Save();
        }
    }
}