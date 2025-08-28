using System.Collections.Generic;
using Unity1week202508.Data;

namespace Unity1week202508.Utility
{
    /// <summary>
    /// 確率検証の結果を格納するクラス
    /// </summary>
    public class ProbabilityValidationResult
    {
        /// <summary>
        /// 検証が成功したかどうか
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 総確率
        /// </summary>
        public float TotalProbability { get; set; }

        /// <summary>
        /// 景品の総数
        /// </summary>
        public int PrizeCount { get; set; }

        /// <summary>
        /// エラーメッセージのリスト
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// 警告メッセージのリスト
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// レア度別の確率情報
        /// </summary>
        public List<RarityProbabilityInfo> RarityInfo { get; set; } = new();

        /// <summary>
        /// 検証結果に問題があるかどうか
        /// </summary>
        public bool HasIssues => Errors.Count > 0 || Warnings.Count > 0;
    }

    /// <summary>
    /// レア度別の確率情報
    /// </summary>
    public class RarityProbabilityInfo
    {
        /// <summary>
        /// レア度
        /// </summary>
        public PrizeRarity Rarity { get; set; }

        /// <summary>
        /// このレア度のアイテム数
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// このレア度の総確率
        /// </summary>
        public float TotalProbability { get; set; }

        /// <summary>
        /// このレア度の平均確率
        /// </summary>
        public float AverageProbability { get; set; }
    }
}