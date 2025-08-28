using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using Unity1week202508.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202508.Lottery
{
    public class LotteryResultPerformerView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Button _closeButton;
        
        [SerializeField]
        private Image _prizeImage;
        
        [SerializeField]
        private TextMeshProUGUI _prizeNameText;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public Observable<Unit> OnClickCloseAsObservable() => _closeButton.OnClickAsObservable();
        
        public void SetPrizeData(PrizeMasterData prizeData)
        {
            if (prizeData != null)
            {
                if (_prizeImage != null)
                    _prizeImage.sprite = prizeData.Image;
                
                if (_prizeNameText != null)
                    _prizeNameText.text = prizeData.PrizeName;
            }
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellationToken);
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            return UniTask.CompletedTask;
        }
    }
}