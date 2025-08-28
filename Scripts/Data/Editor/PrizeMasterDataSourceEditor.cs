using Unity1week202508.Utility;
using UnityEditor;
using UnityEngine;

namespace Unity1week202508.Data.Editor
{
    /// <summary>
    /// PrizeMasterDataSourceのカスタムエディター
    /// </summary>
    [CustomEditor(typeof(PrizeMasterDataSource))]
    public class PrizeMasterDataSourceEditor : UnityEditor.Editor
    {
        private PrizeMasterDataSource _targetDataSource;

        private void OnEnable()
        {
            _targetDataSource = (PrizeMasterDataSource)target;
        }

        public override void OnInspectorGUI()
        {
            // デフォルトのInspectorを描画
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("確率管理ツール", EditorStyles.boldLabel);

            // 確率の検証
            if (GUILayout.Button("確率を検証", GUILayout.Height(30)))
            {
                ValidateProbabilities();
            }

            // 確率の正規化
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("100%に正規化"))
            {
                ProbabilityValidator.NormalizeProbabilities(_targetDataSource, 100f);
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("1000に正規化"))
            {
                ProbabilityValidator.NormalizeProbabilities(_targetDataSource, 1000f);
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();

            // 詳細情報の表示
            if (GUILayout.Button("詳細情報をログ出力"))
            {
                ProbabilityValidator.LogDetailedProbabilityInfo(_targetDataSource);
            }

            EditorGUILayout.Space();

            // 現在の統計情報を表示
            DisplayStatistics();
        }

        /// <summary>
        /// 確率の検証を実行
        /// </summary>
        private void ValidateProbabilities()
        {
            var result = ProbabilityValidator.ValidateProbabilities(_targetDataSource);

            string message = $"検証完了\n" +
                             $"総景品数: {result.PrizeCount}\n" +
                             $"総確率: {result.TotalProbability:F2}\n" +
                             $"エラー: {result.Errors.Count}個\n" +
                             $"警告: {result.Warnings.Count}個";

            if (result.IsValid && !result.HasIssues)
            {
                EditorUtility.DisplayDialog("確率検証", message + "\n\n問題はありません！", "OK");
            }
            else
            {
                message += "\n\n詳細はコンソールを確認してください。";
                EditorUtility.DisplayDialog("確率検証", message, "OK");
                ProbabilityValidator.LogDetailedProbabilityInfo(_targetDataSource);
            }
        }

        /// <summary>
        /// 統計情報を表示
        /// </summary>
        private void DisplayStatistics()
        {
            if (_targetDataSource.Data == null || _targetDataSource.Data.Length == 0)
                return;

            var result = ProbabilityValidator.ValidateProbabilities(_targetDataSource);

            EditorGUILayout.LabelField("統計情報", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"総景品数: {result.PrizeCount}");
            EditorGUILayout.LabelField($"総確率: {result.TotalProbability:F2}");

            if (result.HasIssues)
            {
                GUI.color = Color.yellow;
                EditorGUILayout.LabelField($"⚠ 問題: エラー {result.Errors.Count}個, 警告 {result.Warnings.Count}個");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.green;
                EditorGUILayout.LabelField("✓ 問題なし");
                GUI.color = Color.white;
            }

            // レア度別の情報
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("レア度別統計", EditorStyles.boldLabel);
            foreach (var rarityInfo in result.RarityInfo)
            {
                EditorGUILayout.LabelField($"{rarityInfo.Rarity}: {rarityInfo.ItemCount}個 ({rarityInfo.TotalProbability:F1}%)");
            }
        }
    }
}