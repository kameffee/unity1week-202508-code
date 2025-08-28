using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity1week202508.Audio.Data
{
    /// <summary>
    /// 音声データベース
    /// </summary>
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Unity1week202508/Audio/AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeField]
        private AudioClipData[] _audioClips;

        /// <summary>
        /// IDから音声データを取得
        /// </summary>
        /// <param name="id">音声ID</param>
        /// <returns>音声データ（見つからない場合はnull）</returns>
        public AudioClipData GetAudioClipData(string id)
        {
            return _audioClips?.FirstOrDefault(data => data.Id == id);
        }

        /// <summary>
        /// 指定したタイプの音声データを全て取得
        /// </summary>
        /// <param name="audioType">音声タイプ</param>
        /// <returns>指定したタイプの音声データ一覧</returns>
        public IEnumerable<AudioClipData> GetAudioClipsByType(AudioType audioType)
        {
            return _audioClips?.Where(data => data.AudioType == audioType) ?? Enumerable.Empty<AudioClipData>();
        }
    }
}