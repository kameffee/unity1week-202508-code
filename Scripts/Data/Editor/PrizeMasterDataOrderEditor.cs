using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity1week202508.Data.Editor
{
    /// <summary>
    /// 景品の表示順序を管理するEditorWindow
    /// </summary>
    public class PrizeMasterDataOrderEditor : EditorWindow
    {
        private PrizeMasterDataSource _dataSource;
        private ReorderableList _reorderableList;
        private Vector2 _scrollPosition;
        private readonly List<PrizeMasterData> _prizeList = new();
        private bool _hasUnsavedChanges = false;

        [MenuItem("Window/Unity1week/Prize Order Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PrizeMasterDataOrderEditor>("Prize Order Editor");
            window.minSize = new Vector2(700, 400);
            window.Show();
        }

        private void OnEnable()
        {
            // 前回選択していたDataSourceを復元
            string path = EditorPrefs.GetString("PrizeMasterDataOrderEditor_LastDataSource", "");
            if (!string.IsNullOrEmpty(path))
            {
                _dataSource = AssetDatabase.LoadAssetAtPath<PrizeMasterDataSource>(path);
                if (_dataSource != null)
                {
                    RefreshList();
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // ヘッダー
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("景品表示順序エディタ", EditorStyles.boldLabel);
                
                if (_hasUnsavedChanges)
                {
                    GUILayout.FlexibleSpace();
                    var style = new GUIStyle(EditorStyles.miniLabel);
                    style.normal.textColor = Color.yellow;
                    EditorGUILayout.LabelField("※未保存の変更があります", style);
                }
            }
            
            EditorGUILayout.Space(5);
            
            // PrizeMasterDataSourceの選択
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Data Source:", GUILayout.Width(80));
                var newDataSource = (PrizeMasterDataSource)EditorGUILayout.ObjectField(
                    _dataSource, 
                    typeof(PrizeMasterDataSource), 
                    false
                );
                
                if (newDataSource != _dataSource)
                {
                    if (_hasUnsavedChanges && _dataSource != null)
                    {
                        if (!EditorUtility.DisplayDialog("未保存の変更", 
                            "未保存の変更があります。変更を破棄してもよろしいですか？", 
                            "破棄", "キャンセル"))
                        {
                            return;
                        }
                    }
                    
                    _dataSource = newDataSource;
                    if (_dataSource != null)
                    {
                        EditorPrefs.SetString("PrizeMasterDataOrderEditor_LastDataSource", 
                            AssetDatabase.GetAssetPath(_dataSource));
                    }
                    RefreshList();
                }
            }
            
            EditorGUILayout.Space(10);
            
            if (_dataSource == null)
            {
                EditorGUILayout.HelpBox("PrizeMasterDataSourceを選択してください", MessageType.Info);
                return;
            }
            
            // 統計情報表示
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"景品数: {_prizeList.Count}", EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space(5);
            
            // リスト表示
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            if (_reorderableList != null)
            {
                _reorderableList.DoLayoutList();
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(10);
            
            // ボタン
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("DisplayOrderを10刻みで再割り当て", GUILayout.Height(30)))
                {
                    ReassignDisplayOrders();
                }
                
                GUI.enabled = _hasUnsavedChanges;
                if (GUILayout.Button("変更を保存", GUILayout.Height(30)))
                {
                    SaveChanges();
                }
                GUI.enabled = true;
                
                if (GUILayout.Button("元に戻す", GUILayout.Height(30)))
                {
                    RefreshList();
                }
            }
            
            EditorGUILayout.Space(5);
            
            // ヘルプテキスト
            EditorGUILayout.HelpBox(
                "• ドラッグ&ドロップで景品の順序を変更できます\n" +
                "• DisplayOrderの値を直接編集することも可能です\n" +
                "• 「DisplayOrderを10刻みで再割り当て」で番号を整理できます\n" +
                "• 変更後は必ず「変更を保存」をクリックしてください",
                MessageType.Info);
        }

        private void RefreshList()
        {
            if (_dataSource == null)
            {
                _reorderableList = null;
                _prizeList.Clear();
                _hasUnsavedChanges = false;
                return;
            }

            // 配列をリストにコピー（nullチェック付き）
            _prizeList.Clear();
            if (_dataSource.Data != null)
            {
                foreach (var prize in _dataSource.Data)
                {
                    if (prize != null)
                    {
                        _prizeList.Add(prize);
                    }
                }
            }

            CreateReorderableList();
            _hasUnsavedChanges = false;
        }

        private void CreateReorderableList()
        {
            _reorderableList = new ReorderableList(
                _prizeList,
                typeof(PrizeMasterData),
                true, // draggable
                true, // displayHeader
                false, // displayAddButton
                false  // displayRemoveButton
            );

            // ヘッダー描画
            _reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                var idWidth = 40f;
                var orderWidth = 60f;
                var nameWidth = 200f;
                var rarityWidth = 80f;
                var probabilityWidth = 60f;
                var imageWidth = 50f;
                
                var idRect = new Rect(rect.x + 15, rect.y, idWidth, rect.height);
                var orderRect = new Rect(idRect.xMax + 5, rect.y, orderWidth, rect.height);
                var nameRect = new Rect(orderRect.xMax + 5, rect.y, nameWidth, rect.height);
                var rarityRect = new Rect(nameRect.xMax + 5, rect.y, rarityWidth, rect.height);
                var probabilityRect = new Rect(rarityRect.xMax + 5, rect.y, probabilityWidth, rect.height);
                var imageRect = new Rect(probabilityRect.xMax + 5, rect.y, imageWidth, rect.height);
                
                EditorGUI.LabelField(idRect, "ID");
                EditorGUI.LabelField(orderRect, "Order");
                EditorGUI.LabelField(nameRect, "名前");
                EditorGUI.LabelField(rarityRect, "レア度");
                EditorGUI.LabelField(probabilityRect, "確率(%)");
                EditorGUI.LabelField(imageRect, "画像");
            };

            // 要素描画
            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= _prizeList.Count) return;
                
                var prizeData = _prizeList[index];
                if (prizeData == null) return;

                var idWidth = 40f;
                var orderWidth = 60f;
                var nameWidth = 200f;
                var rarityWidth = 80f;
                var probabilityWidth = 60f;
                var imageWidth = 50f;
                
                var idRect = new Rect(rect.x, rect.y + 2, idWidth, EditorGUIUtility.singleLineHeight);
                var orderRect = new Rect(idRect.xMax + 5, rect.y + 2, orderWidth, EditorGUIUtility.singleLineHeight);
                var nameRect = new Rect(orderRect.xMax + 5, rect.y + 2, nameWidth, EditorGUIUtility.singleLineHeight);
                var rarityRect = new Rect(nameRect.xMax + 5, rect.y + 2, rarityWidth, EditorGUIUtility.singleLineHeight);
                var probabilityRect = new Rect(rarityRect.xMax + 5, rect.y + 2, probabilityWidth, EditorGUIUtility.singleLineHeight);
                var imageRect = new Rect(probabilityRect.xMax + 5, rect.y + 2, imageWidth, EditorGUIUtility.singleLineHeight);
                
                // ID（読み取り専用）
                EditorGUI.LabelField(idRect, prizeData.Id.ToString());
                
                // DisplayOrder（編集可能）
                EditorGUI.BeginChangeCheck();
                var serializedPrizeData = new SerializedObject(prizeData);
                var displayOrderProperty = serializedPrizeData.FindProperty("_displayOrder");
                if (displayOrderProperty != null)
                {
                    EditorGUI.PropertyField(orderRect, displayOrderProperty, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedPrizeData.ApplyModifiedProperties();
                        EditorUtility.SetDirty(prizeData);
                        _hasUnsavedChanges = true;
                    }
                }
                else
                {
                    EditorGUI.LabelField(orderRect, "N/A");
                }
                
                // 名前（読み取り専用）
                EditorGUI.LabelField(nameRect, prizeData.PrizeName);
                
                // レア度（読み取り専用）
                var rarityColor = GetRarityColor(prizeData.Rarity);
                var oldColor = GUI.color;
                GUI.color = rarityColor;
                EditorGUI.LabelField(rarityRect, prizeData.Rarity.ToString());
                GUI.color = oldColor;
                
                // 確率（読み取り専用）
                EditorGUI.LabelField(probabilityRect, $"{prizeData.Probability:F1}%");
                
                // 画像プレビュー
                if (prizeData.Image != null)
                {
                    GUI.DrawTexture(imageRect, prizeData.Image.texture, ScaleMode.ScaleToFit);
                }
            };

            // ドラッグ&ドロップ時の処理
            _reorderableList.onReorderCallback = (ReorderableList list) =>
            {
                _hasUnsavedChanges = true;
                EditorUtility.DisplayProgressBar("更新中", "DisplayOrderを更新しています...", 0.5f);
                UpdateDisplayOrdersAfterReorder();
                EditorUtility.ClearProgressBar();
            };
        }

        private Color GetRarityColor(PrizeRarity rarity)
        {
            switch (rarity)
            {
                case PrizeRarity.Common:
                    return Color.gray;
                case PrizeRarity.Uncommon:
                    return Color.green;
                case PrizeRarity.Rare:
                    return new Color(0.2f, 0.5f, 1f); // Blue
                case PrizeRarity.Epic:
                    return new Color(0.7f, 0.3f, 0.9f); // Purple
                case PrizeRarity.Legendary:
                    return new Color(1f, 0.8f, 0f); // Gold
                default:
                    return Color.white;
            }
        }

        private void UpdateDisplayOrdersAfterReorder()
        {
            // 現在の並び順に基づいてDisplayOrderを更新
            for (int i = 0; i < _prizeList.Count; i++)
            {
                var prizeData = _prizeList[i];
                if (prizeData == null) continue;
                
                var serializedPrizeData = new SerializedObject(prizeData);
                var displayOrderProperty = serializedPrizeData.FindProperty("_displayOrder");
                if (displayOrderProperty != null)
                {
                    displayOrderProperty.intValue = i * 10; // 10刻みで設定
                    serializedPrizeData.ApplyModifiedProperties();
                    EditorUtility.SetDirty(prizeData);
                }
            }
        }

        private void ReassignDisplayOrders()
        {
            if (_prizeList == null || _prizeList.Count == 0) return;

            EditorUtility.DisplayProgressBar("処理中", "DisplayOrderを再割り当てしています...", 0f);

            // 現在のリスト順でDisplayOrderを10刻みで再割り当て
            for (int i = 0; i < _prizeList.Count; i++)
            {
                var prizeData = _prizeList[i];
                if (prizeData == null) continue;
                
                var serializedPrizeData = new SerializedObject(prizeData);
                var displayOrderProperty = serializedPrizeData.FindProperty("_displayOrder");
                if (displayOrderProperty != null)
                {
                    displayOrderProperty.intValue = i * 10;
                    serializedPrizeData.ApplyModifiedProperties();
                    EditorUtility.SetDirty(prizeData);
                }
                
                EditorUtility.DisplayProgressBar("処理中", 
                    $"DisplayOrderを再割り当てしています... ({i + 1}/{_prizeList.Count})", 
                    (float)(i + 1) / _prizeList.Count);
            }

            EditorUtility.ClearProgressBar();
            _hasUnsavedChanges = true;
            
            Debug.Log($"DisplayOrderを再割り当てしました（{_prizeList.Count}個の景品）");
            EditorUtility.DisplayDialog("完了", 
                $"{_prizeList.Count}個の景品のDisplayOrderを10刻みで再割り当てしました。", 
                "OK");
        }

        private new void SaveChanges()
        {
            if (_dataSource == null) return;

            EditorUtility.DisplayProgressBar("保存中", "変更を保存しています...", 0.3f);

            // リストの内容をDataSourceの配列に反映
            _dataSource.Data = _prizeList.ToArray();
            
            // DataSourceを保存
            EditorUtility.SetDirty(_dataSource);
            
            // OnValidateを呼び出してソート
            _dataSource.OnValidate();
            
            EditorUtility.DisplayProgressBar("保存中", "アセットを保存しています...", 0.7f);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.ClearProgressBar();
            
            _hasUnsavedChanges = false;
            
            Debug.Log("変更を保存しました");
            EditorUtility.DisplayDialog("保存完了", "変更を保存しました。", "OK");
        }

        private void OnDestroy()
        {
            if (_hasUnsavedChanges && _dataSource != null)
            {
                var result = EditorUtility.DisplayDialog(
                    "未保存の変更",
                    "未保存の変更があります。保存しますか？",
                    "保存",
                    "破棄"
                );
                
                if (result)
                {
                    SaveChanges();
                }
            }
        }
    }
}