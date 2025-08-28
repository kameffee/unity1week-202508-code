using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity1week202508.Title
{
    /// <summary>
    /// シーン遷移サービス
    /// </summary>
    public class SceneLoaderService
    {
        /// <summary>
        /// 指定したシーンに遷移
        /// </summary>
        public async UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            try
            {
                Debug.Log($"シーン遷移開始: {sceneName}");
                
                // 非同期でシーンをロード
                await SceneManager.LoadSceneAsync(sceneName)
                    .ToUniTask(cancellationToken: cancellationToken);
                
                Debug.Log($"シーン遷移完了: {sceneName}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"シーン遷移がキャンセルされました: {sceneName}");
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"シーン遷移エラー: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// InGameシーンに遷移
        /// </summary>
        public async UniTask LoadInGameSceneAsync(CancellationToken cancellationToken = default)
        {
            await LoadSceneAsync("InGame", cancellationToken);
        }
    }
}