using System.Collections.Generic;
using Unity1week202508.Data;
using Unity1week202508.Lottery;
using UnityEngine;

namespace Unity1week202508.InGame
{
    /// <summary>
    /// クジ用のランダム選択処理（確率重み付きバージョン）
    /// </summary>
    public class LotteryRandomSelector
    {
        private readonly PrizeMasterDataRepository _repository;
        private readonly WeightedRandomSelector<PrizeMasterData> _weightedSelector = new();
        private bool _isInitialized;

        public LotteryRandomSelector(PrizeMasterDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 指定数のランダムな賞品を取得（確率重み付き、重複あり）
        /// </summary>
        public IEnumerable<PrizeMasterData> GetRandomPrizes(int count)
        {
            var allPrizes = _repository.GetAll();
            if (allPrizes.Count == 0)
            {
                Debug.LogWarning("No prizes available in repository");
                yield break;
            }

            // 重み付き選択器を初期化（必要な場合のみ）
            if (!_isInitialized)
            {
                InitializeWeightedSelector(allPrizes);
            }

            // 指定数のランダム選択を実行
            for (int i = 0; i < count; i++)
            {
                yield return _weightedSelector.SelectRandom();
            }
        }

        /// <summary>
        /// 重み付き選択器を初期化
        /// </summary>
        private void InitializeWeightedSelector(IReadOnlyList<PrizeMasterData> prizes)
        {
            _weightedSelector.Clear();

            foreach (var prize in prizes)
            {
                _weightedSelector.AddItem(prize, prize.Probability);
            }

            _isInitialized = true;

            // デバッグ用にログ出力
            Debug.Log($"重み付き選択器を初期化: {prizes.Count}個の景品, 総重み: {_weightedSelector.GetTotalWeight()}");
        }
    }
}