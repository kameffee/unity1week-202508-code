using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202508.Title
{
    /// <summary>
    /// ライセンス表示ウィンドウ
    /// </summary>
    public class LicenseWindowView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _windowRoot;
        
        [SerializeField]
        private Button _closeButton;
        
        [SerializeField]
        private Button _backgroundButton;
        
        [SerializeField]
        private TextMeshProUGUI _licenseText;
        
        [Header("License Content")]
        [SerializeField]
        private TextAsset _licenseTextAsset;

        private void Awake()
        {
            // 初期状態は非表示
            Hide();
            
            // TextAssetからライセンステキストを読み込み
            LoadLicenseText();
        }
        
        private void LoadLicenseText()
        {
            if (_licenseText == null) return;
            
            if (_licenseTextAsset != null)
            {
                _licenseText.text = _licenseTextAsset.text;
            }
            else
            {
                _licenseText.text = "ライセンス情報が設定されていません。\nLicense.txtファイルをTextAssetに設定してください。";
                Debug.LogWarning("LicenseWindowView: TextAssetが設定されていません");
            }
        }

        /// <summary>
        /// 閉じるボタンのクリックイベント
        /// </summary>
        public Observable<Unit> OnCloseButtonClickedAsObservable()
        {
            // 閉じるボタンと背景ボタン両方のクリックで閉じる
            return Observable.Merge(
                _closeButton.OnClickAsObservable(),
                _backgroundButton != null ? _backgroundButton.OnClickAsObservable() : Observable.Empty<Unit>()
            );
        }

        /// <summary>
        /// ウィンドウを表示
        /// </summary>
        public void Show()
        {
            _windowRoot?.SetActive(true);
        }

        /// <summary>
        /// ウィンドウを非表示
        /// </summary>
        public void Hide()
        {
            _windowRoot?.SetActive(false);
        }
        
    }
}