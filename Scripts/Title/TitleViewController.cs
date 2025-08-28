using System;
using R3;
using Unity1week202508.Audio.Data;
using Unity1week202508.Audio.Services;
using Unity1week202508.Manual;
using Unity1week202508.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity1week202508.Title
{
    /// <summary>
    /// UI ToolkitベースのタイトルViewコントローラー
    /// </summary>
    public class TitleViewController : MonoBehaviour, IDisposable
    {
        [Header("UI Document")]
        [SerializeField]
        private UIDocument _uiDocument;

        [Header("License Settings")]
        [SerializeField]
        private TextAsset _licenseTextAsset;

        // UI Elements
        private Button _startButton;
        private Button _manualButton;
        private Button _licenseButton;
        private Button _audioSettingsButton;

        // Modal Elements
        private VisualElement _modalOverlay;
        private VisualElement _licenseWindowInstance;
        private VisualElement _audioSettingsInstance;

        // License Window Elements
        private Label _licenseText;
        private Button _licenseCloseButton;
        private Button _licenseFooterCloseButton;

        // Audio Settings Elements
        private Slider _bgmSlider;
        private Slider _seSlider;
        private Label _bgmValueLabel;
        private Label _seValueLabel;
        private Button _seTestButton;
        private Button _resetButton;
        private Button _applyButton;
        private Button _audioSettingsCloseButton;

        // Observables
        private readonly Subject<Unit> _onStartButtonClickedSubject = new();
        private readonly CompositeDisposable _disposable = new();

        private AudioPlayer _audioPlayer;
        private AudioSettingsService _audioSettingsService;
        private ManualDialogPresenter _manualDialogPresenter;

        public Observable<Unit> OnStartButtonClickedAsObservable() => _onStartButtonClickedSubject;

        public void Initialize(AudioPlayer audioPlayer, AudioSettingsService audioSettingsService, ManualDialogPresenter manualDialogPresenter)
        {
            _audioPlayer = audioPlayer;
            _audioSettingsService = audioSettingsService;
            _manualDialogPresenter = manualDialogPresenter;
        }

        private void Awake()
        {
            if (_uiDocument == null)
                _uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            InitializeUI();
            SetupEventHandlers();
            LoadLicenseText();
        }

        /// <summary>
        /// UI要素を初期化
        /// </summary>
        private void InitializeUI()
        {
            var root = _uiDocument.rootVisualElement;

            // Main Buttons
            _startButton = root.Q<Button>("start-button");
            _manualButton = root.Q<Button>("manual-button");
            _licenseButton = root.Q<Button>("license-button");
            _audioSettingsButton = root.Q<Button>("audio-settings-button");

            // Modal Overlay
            _modalOverlay = root.Q<VisualElement>("modal-overlay");
            _licenseWindowInstance = root.Q<VisualElement>("license-window-instance");
            _audioSettingsInstance = root.Q<VisualElement>("audio-settings-instance");

            // License Window Elements
            _licenseText = _licenseWindowInstance?.Q<Label>("license-text");
            _licenseCloseButton = _licenseWindowInstance?.Q<Button>("close-button");
            _licenseFooterCloseButton = _licenseWindowInstance?.Q<Button>("footer-close-button");

            // Audio Settings Elements
            _bgmSlider = _audioSettingsInstance?.Q<Slider>("bgm-slider");
            _seSlider = _audioSettingsInstance?.Q<Slider>("se-slider");
            _bgmValueLabel = _audioSettingsInstance?.Q<Label>("bgm-value-label");
            _seValueLabel = _audioSettingsInstance?.Q<Label>("se-value-label");
            _seTestButton = _audioSettingsInstance?.Q<Button>("se-test-button");
            _resetButton = _audioSettingsInstance?.Q<Button>("reset-button");
            _applyButton = _audioSettingsInstance?.Q<Button>("apply-button");
            _audioSettingsCloseButton = _audioSettingsInstance?.Q<Button>("close-button");

            // 初期状態は全て非表示
            _modalOverlay?.SetVisible(false);
            _licenseWindowInstance?.SetVisible(false);
            _audioSettingsInstance?.SetVisible(false);
        }

        /// <summary>
        /// イベントハンドラーを設定
        /// </summary>
        private void SetupEventHandlers()
        {
            // Main Menu Buttons
            _startButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    _onStartButtonClickedSubject.OnNext(Unit.Default);
                })
                .AddTo(this);

            _manualButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    ShowManualDialog();
                })
                .AddTo(this);

            _licenseButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    ShowLicenseWindow();
                })
                .AddTo(this);

            _audioSettingsButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    ShowAudioSettingsWindow();
                })
                .AddTo(this);

            // License Window
            _licenseCloseButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    HideLicenseWindow();
                })
                .AddTo(this);

            _licenseFooterCloseButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    HideLicenseWindow();
                })
                .AddTo(this);

            // Audio Settings Window
            _audioSettingsCloseButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    HideAudioSettingsWindow();
                })
                .AddTo(this);

            _resetButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    ResetAudioSettings();
                })
                .AddTo(this);

            _applyButton?.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayButtonSE();
                    HideAudioSettingsWindow();
                })
                .AddTo(this);

            // Audio Settings - Volume Changes
            _bgmSlider?.OnValueChangedAsObservable()
                .Subscribe(value =>
                {
                    _bgmValueLabel.text = $"{(int)(value * 100)}%";
                    if (_audioSettingsService != null)
                        _audioSettingsService.SetBgmVolume(new AudioVolume(value));
                })
                .AddTo(this);

            _seSlider?.OnValueChangedAsObservable()
                .Subscribe(value =>
                {
                    _seValueLabel.text = $"{(int)(value * 100)}%";
                    if (_audioSettingsService != null)
                        _audioSettingsService.SetSeVolume(new AudioVolume(value));
                })
                .AddTo(this);

            // SE Test Button
            _seTestButton?.OnClickAsObservable()
                .Subscribe(_ => PlayTestSE())
                .AddTo(this);
        }

        /// <summary>
        /// ライセンステキストを読み込み
        /// </summary>
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
                Debug.LogWarning("TitleViewController: TextAssetが設定されていません");
            }
        }

        /// <summary>
        /// マニュアルダイアログを表示
        /// </summary>
        private void ShowManualDialog()
        {
            _manualDialogPresenter?.ShowManual();
        }

        /// <summary>
        /// ライセンスウィンドウを表示
        /// </summary>
        private void ShowLicenseWindow()
        {
            _modalOverlay?.SetVisible(true);
            _licenseWindowInstance.FadeIn();
        }

        /// <summary>
        /// ライセンスウィンドウを非表示
        /// </summary>
        private void HideLicenseWindow()
        {
            _licenseWindowInstance.FadeOut(onComplete: () => _modalOverlay?.SetVisible(false));
        }

        /// <summary>
        /// 音声設定ウィンドウを表示
        /// </summary>
        private void ShowAudioSettingsWindow()
        {
            UpdateAudioSettingsUI();
            _modalOverlay?.SetVisible(true);
            _audioSettingsInstance?.FadeIn();
        }

        /// <summary>
        /// 音声設定ウィンドウを非表示
        /// </summary>
        private void HideAudioSettingsWindow()
        {
            _audioSettingsInstance?.FadeOut(onComplete: () => _modalOverlay?.SetVisible(false));
        }

        /// <summary>
        /// 全てのモーダルを非表示
        /// </summary>
        private void HideAllModals()
        {
            HideLicenseWindow();
            HideAudioSettingsWindow();
        }

        /// <summary>
        /// 音声設定UIを更新
        /// </summary>
        private void UpdateAudioSettingsUI()
        {
            if (_audioSettingsService == null) return;

            var bgmVolume = _audioSettingsService.BgmVolume.CurrentValue;
            var seVolume = _audioSettingsService.SeVolume.CurrentValue;

            if (_bgmSlider != null)
            {
                _bgmSlider.value = bgmVolume.Value;
                _bgmValueLabel.text = $"{(int)(bgmVolume.Value * 100)}%";
            }

            if (_seSlider != null)
            {
                _seSlider.value = seVolume.Value;
                _seValueLabel.text = $"{(int)(seVolume.Value * 100)}%";
            }
        }

        /// <summary>
        /// 音声設定をデフォルトに戻す
        /// </summary>
        private void ResetAudioSettings()
        {
            if (_audioSettingsService == null) return;

            _audioSettingsService.SetBgmVolume(AudioVolume.DefaultBgm);
            _audioSettingsService.SetSeVolume(AudioVolume.DefaultSe);
            UpdateAudioSettingsUI();
        }

        /// <summary>
        /// ボタンクリック用SEを再生
        /// </summary>
        private void PlayButtonSE()
        {
            // フォールバック: AudioDatabase経由での再生を試行
            _audioPlayer?.PlaySe("button-click");
        }

        /// <summary>
        /// テスト用SEを再生
        /// </summary>
        private void PlayTestSE()
        {
            PlayButtonSE(); // テストボタンでもボタンSEを再生
        }

        /// <summary>
        /// ボタンの有効/無効を設定
        /// </summary>
        public void SetButtonsEnabled(bool enabled)
        {
            _startButton?.SetEnabled(enabled);
            _manualButton?.SetEnabled(enabled);
            _licenseButton?.SetEnabled(enabled);
            _audioSettingsButton?.SetEnabled(enabled);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _onStartButtonClickedSubject?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}