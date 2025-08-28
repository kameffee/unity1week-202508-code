using System;
using UnityEngine;

namespace Unity1week202508.Audio.Data
{
    /// <summary>
    /// 音量値を管理する構造体（0.0～1.0の範囲）
    /// </summary>
    [Serializable]
    public struct AudioVolume : IEquatable<AudioVolume>
    {
        [SerializeField] 
        private float _value;

        /// <summary>
        /// 音量値（0.0～1.0）
        /// </summary>
        public float Value => _value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">音量値（0.0～1.0の範囲外の場合はClampされる）</param>
        public AudioVolume(float value)
        {
            _value = Mathf.Clamp01(value);
        }

        /// <summary>
        /// 無音の音量
        /// </summary>
        public static AudioVolume Zero => new(0.0f);

        /// <summary>
        /// 最大音量
        /// </summary>
        public static AudioVolume Max => new(1.0f);

        /// <summary>
        /// デフォルトBGM音量（0.3）
        /// </summary>
        public static AudioVolume DefaultBgm => new(0.3f);

        /// <summary>
        /// デフォルトSE音量（0.6）
        /// </summary>
        public static AudioVolume DefaultSe => new(0.6f);

        /// <summary>
        /// デフォルト音量（0.5）
        /// </summary>
        public static AudioVolume Default => new(0.5f);

        /// <summary>
        /// float値から暗黙的に変換
        /// </summary>
        public static implicit operator AudioVolume(float value) => new(value);

        /// <summary>
        /// AudioVolumeからfloat値に暗黙的に変換
        /// </summary>
        public static implicit operator float(AudioVolume volume) => volume._value;

        public bool Equals(AudioVolume other)
        {
            return Mathf.Approximately(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is AudioVolume other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(AudioVolume left, AudioVolume right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AudioVolume left, AudioVolume right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"AudioVolume({_value:F2})";
        }
    }
}