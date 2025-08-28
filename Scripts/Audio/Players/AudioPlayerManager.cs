using UnityEngine;

namespace Unity1week202508.Audio.Players
{
    /// <summary>
    /// AudioPlayerManagerコンポーネント - BgmPlayerとSePlayerを管理
    /// </summary>
    public class AudioPlayerManager : MonoBehaviour
    {
        [SerializeField] 
        private BgmPlayer _bgmPlayer;
        
        [SerializeField] 
        private SePlayer _sePlayer;

        /// <summary>
        /// BGMプレイヤー
        /// </summary>
        public BgmPlayer BgmPlayer
        {
            get
            {
                if (_bgmPlayer == null)
                {
                    _bgmPlayer = GetComponentInChildren<BgmPlayer>();
                    if (_bgmPlayer == null)
                    {
                        var bgmObject = new GameObject("BgmPlayer");
                        bgmObject.transform.SetParent(transform);
                        _bgmPlayer = bgmObject.AddComponent<BgmPlayer>();
                    }
                }
                return _bgmPlayer;
            }
        }

        /// <summary>
        /// SEプレイヤー
        /// </summary>
        public SePlayer SePlayer
        {
            get
            {
                if (_sePlayer == null)
                {
                    _sePlayer = GetComponentInChildren<SePlayer>();
                    if (_sePlayer == null)
                    {
                        var seObject = new GameObject("SePlayer");
                        seObject.transform.SetParent(transform);
                        _sePlayer = seObject.AddComponent<SePlayer>();
                    }
                }
                return _sePlayer;
            }
        }

        private void Awake()
        {
            // シーンを跨いで持続させる
            DontDestroyOnLoad(gameObject);
        }
    }
}