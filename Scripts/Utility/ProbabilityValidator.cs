using System.Linq;
using Unity1week202508.Data;
using UnityEngine;

namespace Unity1week202508.Utility
{
    /// <summary>
    /// 景品確率の検証とバランス調整のためのユーティリティクラス
    /// </summary>
    public static class ProbabilityValidator
    {
        /// <summary>
        /// 景品データソースの確率を検証
        /// </summary>
        /// <param name="dataSource">検証する景品データソース</param>
        public static ProbabilityValidationResult ValidateProbabilities(PrizeMasterDataSource dataSource)
        {
            var result = new ProbabilityValidationResult();
            var prizes = dataSource.Data;

            if (prizes == null || prizes.Length == 0)
            {
                result.IsValid = false;
                result.Warnings.Add("景品データが存在しません");
                return result;
            }

            // 基本的な検証
            result.TotalProbability = prizes.Sum(p => p.Probability);
            result.PrizeCount = prizes.Length;

            // 確率の範囲チェック
            foreach (var prize in prizes)
            {
                if (prize.Probability <= 0f)
                {
                    result.Errors.Add($"景品 '{prize.PrizeName}' の確率が0以下です: {prize.Probability}");
                }
                else if (prize.Probability > 100f)
                {
                    result.Warnings.Add($"景品 '{prize.PrizeName}' の確率が100%を超えています: {prize.Probability}%");
                }
            }

            // レア度別の確率チェック
            ValidateRarityBalance(prizes, result);

            // 極端な確率の警告
            CheckExtremeProportions(prizes, result);

            // 全体的な妥当性判定
            result.IsValid = result.Errors.Count == 0;

            return result;
        }

        /// <summary>
        /// レア度バランスの検証
        /// </summary>
        private static void ValidateRarityBalance(PrizeMasterData[] prizes, ProbabilityValidationResult result)
        {
            var rarityGroups = prizes.GroupBy(p => p.Rarity).ToList();
            
            foreach (var group in rarityGroups)
            {
                float totalProbabilityForRarity = group.Sum(p => p.Probability);
                float averageProbability = totalProbabilityForRarity / group.Count();
                
                result.RarityInfo.Add(new RarityProbabilityInfo
                {
                    Rarity = group.Key,
                    ItemCount = group.Count(),
                    TotalProbability = totalProbabilityForRarity,
                    AverageProbability = averageProbability
                });

                // レア度に応じた適切な確率範囲の推奨
                var recommendedRange = GetRecommendedProbabilityRange(group.Key);
                if (averageProbability < recommendedRange.min || averageProbability > recommendedRange.max)
                {
                    result.Warnings.Add(
                        $"レア度 {group.Key} の平均確率 ({averageProbability:F2}%) が " +
                        $"推奨範囲 ({recommendedRange.min:F2}%-{recommendedRange.max:F2}%) から外れています");
                }
            }
        }

        /// <summary>
        /// 極端な確率比率をチェック
        /// </summary>
        private static void CheckExtremeProportions(PrizeMasterData[] prizes, ProbabilityValidationResult result)
        {
            float maxProbability = prizes.Max(p => p.Probability);
            float minProbability = prizes.Min(p => p.Probability);
            float ratio = maxProbability / minProbability;

            if (ratio > 1000f)
            {
                result.Warnings.Add($"確率の差が極端です (最大/最小比: {ratio:F1})。バランス調整を検討してください。");
            }

            // 支配的な確率をチェック
            float totalProbability = prizes.Sum(p => p.Probability);
            var dominantPrize = prizes.FirstOrDefault(p => p.Probability / totalProbability > 0.8f);
            if (dominantPrize != null)
            {
                result.Warnings.Add($"景品 '{dominantPrize.PrizeName}' が全体の80%以上を占めています。");
            }
        }

        /// <summary>
        /// レア度に応じた推奨確率範囲を取得
        /// </summary>
        private static (float min, float max) GetRecommendedProbabilityRange(PrizeRarity rarity)
        {
            return rarity switch
            {
                PrizeRarity.Common => (30f, 70f),
                PrizeRarity.Uncommon => (10f, 30f),
                PrizeRarity.Rare => (3f, 10f),
                PrizeRarity.Epic => (0.5f, 3f),
                PrizeRarity.Legendary => (0.1f, 1f),
                _ => (1f, 100f)
            };
        }

        /// <summary>
        /// 確率を正規化（合計100%になるように調整）
        /// </summary>
        public static void NormalizeProbabilities(PrizeMasterDataSource dataSource, float targetTotal = 100f)
        {
            var prizes = dataSource.Data;
            if (prizes == null || prizes.Length == 0) return;

            float currentTotal = prizes.Sum(p => p.Probability);
            if (currentTotal <= 0f) return;

            float normalizeRatio = targetTotal / currentTotal;

            Debug.Log($"確率を正規化: 現在の合計 {currentTotal:F2} -> 目標 {targetTotal:F2} (倍率: {normalizeRatio:F3})");

            foreach (var prize in prizes)
            {
                float oldProbability = prize.Probability;
                // Reflectionを使用してprivateフィールドを更新
                var field = typeof(PrizeMasterData).GetField("_probability", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    float newProbability = Mathf.Max(0.1f, oldProbability * normalizeRatio);
                    field.SetValue(prize, newProbability);
                }
            }

            // ScriptableObjectを保存
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(dataSource);
#endif
        }

        /// <summary>
        /// デバッグ用の詳細情報を出力
        /// </summary>
        public static void LogDetailedProbabilityInfo(PrizeMasterDataSource dataSource)
        {
            var result = ValidateProbabilities(dataSource);
            
            Debug.Log("=== 景品確率詳細情報 ===");
            Debug.Log($"総景品数: {result.PrizeCount}");
            Debug.Log($"総確率: {result.TotalProbability:F2}");
            
            foreach (var rarityInfo in result.RarityInfo)
            {
                Debug.Log($"[{rarityInfo.Rarity}] " +
                         $"アイテム数: {rarityInfo.ItemCount}, " +
                         $"総確率: {rarityInfo.TotalProbability:F2}%, " +
                         $"平均確率: {rarityInfo.AverageProbability:F2}%");
            }

            foreach (var warning in result.Warnings)
            {
                Debug.LogWarning(warning);
            }

            foreach (var error in result.Errors)
            {
                Debug.LogError(error);
            }
        }
    }
}