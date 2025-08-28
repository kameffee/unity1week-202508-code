using System;
using UnityEngine;

namespace Unity1week202508.Audio.Data
{
    /// <summary>
    /// 音声クリップデータ
    /// </summary>
    [Serializable]
    public class AudioClipData
    {
        [SerializeField] 
        private string _id;
        
        [SerializeField] 
        private AudioClip _audioClip;
        
        [SerializeField] 
        private AudioType _audioType;

        /// <summary>
        /// 音声ID
        /// </summary>
        public string Id => _id;
        
        /// <summary>
        /// 音声クリップ
        /// </summary>
        public AudioClip AudioClip => _audioClip;
        
        /// <summary>
        /// 音声タイプ
        /// </summary>
        public AudioType AudioType => _audioType;

        public AudioClipData(string id, AudioClip audioClip, AudioType audioType)
        {
            _id = id;
            _audioClip = audioClip;
            _audioType = audioType;
        }
    }
}