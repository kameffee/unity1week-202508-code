using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202508.Data;

namespace Unity1week202508.Lottery
{
    public class LotteryResultPhase
    {
        private readonly PrizeAcquisitionDialogView _prizeAcquisitionDialogView;
        private readonly PrizeAcquisitionRepository _prizeAcquisitionRepository;

        public LotteryResultPhase(
            PrizeAcquisitionDialogView prizeAcquisitionDialogView,
            PrizeAcquisitionRepository prizeAcquisitionRepository)
        {
            _prizeAcquisitionDialogView = prizeAcquisitionDialogView;
            _prizeAcquisitionRepository = prizeAcquisitionRepository;
        }

        public async UniTask ExecuteAsync(PrizeMasterData prizeData, CancellationToken cancellationToken = default)
        {
            // 景品を獲得済みとして保存し、初回獲得かチェック
            bool isFirstAcquisition = _prizeAcquisitionRepository.AddAcquiredPrize(prizeData.Id);

            // 初回獲得の場合のみ演出を表示
            if (isFirstAcquisition)
            {
                // 賞品データを設定
                _prizeAcquisitionDialogView.SetPrizeData(prizeData);

                // 結果表示
                await _prizeAcquisitionDialogView.ShowAsync(cancellationToken);

                // OKボタンクリックまで待機
                await _prizeAcquisitionDialogView.OnClickOkAsObservable()
                    .FirstAsync(cancellationToken: cancellationToken);

                // 結果非表示
                await _prizeAcquisitionDialogView.HideAsync(cancellationToken);
            }
            else
            {
                // 既に獲得済みの場合は演出をスキップ
                UnityEngine.Debug.Log($"既に獲得済みの景品: {prizeData.PrizeName} (ID: {prizeData.Id})");
            }
        }
    }
}