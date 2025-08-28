using R3;
using Unity1week202508.Manual;
using Unity1week202508.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using VContainer;

namespace Unity1week202508.InGame.UI
{
    /// <summary>
    /// InGameシーンのUIコントローラー
    /// </summary>
    public class InGameUIController : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField]
        private UIDocument _uiDocument;

        [Header("UI Assets")]
        [SerializeField]
        private VisualTreeAsset _prizeCollectionDialogTemplate;

        [SerializeField]
        private StyleSheet _prizeCollectionStyleSheet;

        private ManualDialogPresenter _manualDialogPresenter;

        [Inject]
        public void Construct(ManualDialogPresenter manualDialogPresenter)
        {
            _manualDialogPresenter = manualDialogPresenter;
        }

        private void Awake()
        {
            Assert.IsNotNull(_uiDocument);
        }

        public Observable<Unit> OnClickCollectionButtonAsObservable()
        {
            return _uiDocument.rootVisualElement.Q<Button>("collection-button")
                .OnClickAsObservable()
                .AsUnitObservable();
        }

        public Observable<Unit> OnClickManualButtonAsObservable()
        {
            return _uiDocument.rootVisualElement.Q<Button>("manual-button")
                .OnClickAsObservable()
                .AsUnitObservable();
        }

        /// <summary>
        /// マニュアルダイアログを表示
        /// </summary>
        public void ShowManualDialog()
        {
            _manualDialogPresenter?.ShowManual();
        }
    }
}