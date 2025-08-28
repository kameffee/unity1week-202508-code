using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity1week202508.Data;

namespace Unity1week202508.Lottery
{
    /// <summary>
    /// クジ選択のビジネスロジック
    /// </summary>
    public class LotterySelectionService
    {
        private readonly LotteryFieldManager _fieldManager;
        private readonly Subject<PrizeMasterData> _onSelectedSubject = new();

        public Observable<PrizeMasterData> OnSelectedAsObservable() => _onSelectedSubject;

        public LotterySelectionService(LotteryFieldManager fieldManager)
        {
            _fieldManager = fieldManager;

            // フィールドマネージャーからのクジ選択イベントを購読
            _fieldManager.OnLotterySelectedAsObservable()
                .Subscribe(eventData =>
                {
                    // 上位層には賞品データのみを渡す
                    _onSelectedSubject.OnNext(eventData.PrizeData);
                });
        }

        /// <summary>
        /// クジ選択フェーズを実行
        /// </summary>
        public async UniTask<PrizeMasterData> WaitForSelectionAsync(CancellationToken cancellationToken = default)
        {
            // フィールドをリセット（全クジを選択可能にする）
            _fieldManager.ResetField();

            // プレイヤーがクジを選択するまで待機
            var selectedPrize = await _onSelectedSubject
                .FirstAsync(cancellationToken: cancellationToken);

            // 選択されたクジはFieldManagerで既に削除されている

            return selectedPrize;
        }

        /// <summary>
        /// 残りのクジ数を取得
        /// </summary>
        public int GetRemainingLotteryCount()
        {
            return _fieldManager.GetRemainingCount();
        }

        public void Dispose()
        {
            _onSelectedSubject?.Dispose();
        }
    }
}