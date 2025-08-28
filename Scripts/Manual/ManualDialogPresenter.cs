using System;
using R3;
using UnityEngine;

namespace Unity1week202508.Manual
{
    /// <summary>
    /// マニュアルダイアログのPresenterクラス
    /// </summary>
    public class ManualDialogPresenter : IDisposable
    {
        private readonly ManualDialogView _view;
        private readonly TextAsset _manualTextAsset;
        private readonly CompositeDisposable _disposables = new();

        public ManualDialogPresenter(ManualDialogView view, TextAsset manualTextAsset)
        {
            _view = view;
            _manualTextAsset = manualTextAsset;

            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            // マニュアルテキストを設定
            LoadManualText();

            // 閉じるボタンのイベントを購読
            _view.OnCloseClickedAsObservable()
                .Subscribe(_ => OnManualClosed())
                .AddTo(_disposables);
        }

        /// <summary>
        /// マニュアルテキストを読み込んで設定
        /// </summary>
        private void LoadManualText()
        {
            _view.SetManualText(_manualTextAsset.text);
        }

        /// <summary>
        /// マニュアルダイアログを表示
        /// </summary>
        public void ShowManual()
        {
            _view.Show();
            Debug.Log("マニュアルダイアログを表示しました");
        }

        /// <summary>
        /// マニュアルダイアログを非表示
        /// </summary>
        public void HideManual()
        {
            _view.Hide();
            Debug.Log("マニュアルダイアログを非表示にしました");
        }

        /// <summary>
        /// マニュアルダイアログの表示状態を取得
        /// </summary>
        /// <returns>表示中の場合true</returns>
        public bool IsManualVisible()
        {
            return _view.IsVisible();
        }

        /// <summary>
        /// マニュアルが閉じられた時の処理
        /// </summary>
        private static void OnManualClosed()
        {
            Debug.Log("マニュアルダイアログが閉じられました");
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}