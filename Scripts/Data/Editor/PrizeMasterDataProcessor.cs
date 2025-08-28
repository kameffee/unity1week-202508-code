using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity1week202508.Data.Editor
{
    public class PrizeMasterDataProcessor : AssetPostprocessor
    {
        private const string MasterDataPath = "Assets/Application/ScriptableObjects/MasterData/Prize";

        private const string MasterDataSourcePath =
            "Assets/Application/ScriptableObjects/MasterData/Prize/PrizeMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Ending関連のアセットが変更されたかチェック
            var isChanged = HasEndingMasterDataChanged(importedAssets)
                            || HasEndingMasterDataChanged(deletedAssets)
                            || HasEndingMasterDataChanged(movedAssets)
                            || HasEndingMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、EndingMasterDataSourceを更新
            if (isChanged)
            {
                UpdateMasterDataSource();
            }
        }

        private static bool HasEndingMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(MasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("PrizeMasterDataSource"));
        }

        /// <summary>
        /// EndingMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdateMasterDataSource()
        {
            // EndingMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<PrizeMasterDataSource>(MasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(MasterDataPath);
                dataSource = ScriptableObject.CreateInstance<PrizeMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, MasterDataSourcePath);
                Debug.Log($"Created PrizeMasterDataSource at {MasterDataSourcePath}");
            }

            // EndingMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:PrizeMasterData", new[] { MasterDataPath });
            var prizeMasterDataList = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !path.Contains("PrizeMasterDataSource")) // DataSourceは除外
                .Select(AssetDatabase.LoadAssetAtPath<PrizeMasterData>)
                .Where(data => data != null)
                .ToArray();

            // EndingMasterDataSourceに設定
            var serializedObject = new SerializedObject(dataSource);
            var prizeMasterDataProperty = serializedObject.FindProperty("_data");

            prizeMasterDataProperty.arraySize = prizeMasterDataList.Length;
            for (int i = 0; i < prizeMasterDataList.Length; i++)
            {
                prizeMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = prizeMasterDataList[i];
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();

            Debug.Log($"Updated PrizeMasterDataSource with {prizeMasterDataList.Length} items");
        }

        /// <summary>
        /// 指定されたパスにディレクトリが存在しない場合、作成する
        /// </summary>
        private static void CreateDirectoryIfNeeded(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath)) return;

            // ディレクトリが存在しない場合は作成
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        // メニューアイテムから手動で更新する機能
        [MenuItem("Tools/MasterData/Prize/マスタデータソース更新")]
        private static void ManualUpdateMasterDataSource()
        {
            UpdateMasterDataSource();
        }

        [MenuItem("Tools/MasterData/Prize/IDの採番")]
        private static void ReassignIds()
        {
            // PrizeMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<PrizeMasterDataSource>(MasterDataSourcePath);
            if (dataSource == null)
            {
                Debug.LogError($"PrizeMasterDataSource not found at {MasterDataSourcePath}");
                return;
            }

            if (dataSource == null || dataSource.Data == null || dataSource.Data.Length == 0) return;

            // 既存のIDを記録
            var ids = new HashSet<int>();
            var needsUpdate = false;

            // すべてのデータをチェック
            foreach (var data in dataSource.Data.OrderBy(data => data.name))
            {
                if (data == null) continue;

                // SerializedObjectを使ってプロパティにアクセス
                var serializedObject = new SerializedObject(data);
                var idProperty = serializedObject.FindProperty("_id");
                int currentId = idProperty.intValue;

                // IDが重複している場合は再割り当て
                if (!ids.Add(currentId))
                {
                    int newId = 1;
                    // 使用されていないIDを探す
                    while (ids.Contains(newId))
                    {
                        newId++;
                    }

                    // 新しいIDを設定
                    idProperty.intValue = newId;
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(data);

                    Debug.Log($"Changed ID of {data.name} from {currentId} to {newId}");

                    ids.Add(newId);
                    needsUpdate = true;
                }

                // IDに基づいてアセット名を更新（重複していない場合も含めて全て処理）
                var formattedId = $"{idProperty.intValue:D3}"; // 3桁で0埋め
                var expectedName = $"Prize_{formattedId}";

                // 現在の名前と期待する名前が異なる場合に変更
                if (!data.name.Equals(expectedName))
                {
                    var assetPath = AssetDatabase.GetAssetPath(data);
                    AssetDatabase.RenameAsset(assetPath, expectedName);
                    Debug.Log($"Renamed asset from {data.name} to {expectedName}");
                    needsUpdate = true;
                }
            }

            if (needsUpdate)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("ID reassignment completed.");
            }
            else
            {
                Debug.Log("No duplicate IDs found.");
            }
        }
    }
}