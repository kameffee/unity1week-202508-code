using UnityEditor;
using UnityEngine;

namespace Unity1week202508.Data.Editor
{
    /// <summary>
    /// PlayerPrefsを管理するエディターツール
    /// </summary>
    public static class PlayerPrefsEditorTool
    {
        [MenuItem("Tools/PlayerPrefs/すべてクリア")]
        private static void ClearAllPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog(
                "PlayerPrefsクリア確認",
                "すべてのPlayerPrefsデータを削除します。\nこの操作は元に戻せません。\n\n実行しますか？",
                "実行",
                "キャンセル"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("PlayerPrefs: すべてのデータをクリアしました");
            }
        }

        [MenuItem("Tools/PlayerPrefs/獲得済み景品をクリア")]
        private static void ClearAcquiredPrizes()
        {
            if (EditorUtility.DisplayDialog(
                "獲得済み景品クリア確認",
                "獲得済み景品のデータを削除します。\nこの操作は元に戻せません。\n\n実行しますか？",
                "実行",
                "キャンセル"))
            {
                PlayerPrefs.DeleteKey("AcquiredPrizes");
                PlayerPrefs.Save();
                Debug.Log("PlayerPrefs: 獲得済み景品データをクリアしました");
            }
        }

        [MenuItem("Tools/PlayerPrefs/音声設定をクリア")]
        private static void ClearAudioSettings()
        {
            if (EditorUtility.DisplayDialog(
                "音声設定クリア確認",
                "音声設定（BGM/SE音量）をデフォルトに戻します。\nこの操作は元に戻せません。\n\n実行しますか？",
                "実行",
                "キャンセル"))
            {
                PlayerPrefs.DeleteKey("BgmVolume");
                PlayerPrefs.DeleteKey("SeVolume");
                PlayerPrefs.Save();
                Debug.Log("PlayerPrefs: 音声設定をクリアしました（デフォルトに戻りました）");
            }
        }

        [MenuItem("Tools/PlayerPrefs/現在の保存データを表示")]
        private static void ShowCurrentPlayerPrefs()
        {
            Debug.Log("=== PlayerPrefs 現在の保存データ ===");
            
            // 獲得済み景品
            var acquiredPrizes = PlayerPrefs.GetString("AcquiredPrizes", "なし");
            Debug.Log($"獲得済み景品ID: {acquiredPrizes}");
            
            // BGM音量
            var bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
            Debug.Log($"BGM音量: {bgmVolume:F2}");
            
            // SE音量
            var seVolume = PlayerPrefs.GetFloat("SeVolume", 0.5f);
            Debug.Log($"SE音量: {seVolume:F2}");
            
            Debug.Log("=====================================");
        }
    }
}