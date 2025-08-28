using UnityEditor;
using UnityEngine;

namespace Unity1week202508.Data.Editor
{
    /// <summary>
    /// PrizeMasterDataのカスタムエディター
    /// </summary>
    [CustomEditor(typeof(PrizeMasterData))]
    public class PrizeMasterDataEditor : UnityEditor.Editor
    {
        private PrizeMasterData _targetData;

        private void OnEnable()
        {
            _targetData = (PrizeMasterData)target;
        }

        public override void OnInspectorGUI()
        {
            // デフォルトのInspectorを描画
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("確率情報", EditorStyles.boldLabel);

            // 現在の確率を表示
            EditorGUILayout.LabelField($"設定確率: {_targetData.Probability:F2}");

            // レア度に基づく推奨確率範囲を表示
            var recommendedRange = GetRecommendedProbabilityRange(_targetData.Rarity);
            EditorGUILayout.LabelField($"推奨確率範囲 ({_targetData.Rarity}): {recommendedRange.min:F1}% - {recommendedRange.max:F1}%");

            // 確率が推奨範囲外の場合は警告
            if (_targetData.Probability < recommendedRange.min || _targetData.Probability > recommendedRange.max)
            {
                EditorGUILayout.HelpBox($"確率が推奨範囲外です。レア度 {_targetData.Rarity} の推奨範囲: {recommendedRange.min:F1}% - {recommendedRange.max:F1}%", MessageType.Warning);
            }

            EditorGUILayout.Space();

            // レア度設定用のボタン
            EditorGUILayout.LabelField("クイック設定", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Common設定"))
            {
                SetProbabilityForRarity(PrizeRarity.Common);
            }
            if (GUILayout.Button("Uncommon設定"))
            {
                SetProbabilityForRarity(PrizeRarity.Uncommon);
            }
            if (GUILayout.Button("Rare設定"))
            {
                SetProbabilityForRarity(PrizeRarity.Rare);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Epic設定"))
            {
                SetProbabilityForRarity(PrizeRarity.Epic);
            }
            if (GUILayout.Button("Legendary設定"))
            {
                SetProbabilityForRarity(PrizeRarity.Legendary);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// レア度に応じた確率を設定
        /// </summary>
        private void SetProbabilityForRarity(PrizeRarity rarity)
        {
            SerializedProperty rarityProperty = serializedObject.FindProperty("_rarity");
            SerializedProperty probabilityProperty = serializedObject.FindProperty("_probability");

            rarityProperty.enumValueIndex = (int)rarity;
            
            var recommendedRange = GetRecommendedProbabilityRange(rarity);
            float recommendedProbability = (recommendedRange.min + recommendedRange.max) / 2f;
            probabilityProperty.floatValue = recommendedProbability;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
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
    }
}