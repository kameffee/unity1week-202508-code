using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity1week202508.Data
{
    /// <summary>
    /// 景品の獲得データ
    /// </summary>
    [Serializable]
    public class PrizeAcquisitionData
    {
        public int prizeId;
        public int count;

        public PrizeAcquisitionData() { }

        public PrizeAcquisitionData(int prizeId, int count)
        {
            this.prizeId = prizeId;
            this.count = count;
        }
    }

    /// <summary>
    /// 景品獲得データのリスト（JSON化用）
    /// </summary>
    [Serializable]
    public class PrizeAcquisitionDataList
    {
        public List<PrizeAcquisitionData> prizes = new();
    }
    /// <summary>
    /// 景品獲得履歴を管理するリポジトリ
    /// </summary>
    public class PrizeAcquisitionRepository
    {
        private const string SaveKey = "AcquiredPrizes";
        private readonly List<PrizeAcquisitionData> _acquisitionData;

        public PrizeAcquisitionRepository()
        {
            // PlayerPrefsから獲得データを読み込み
            _acquisitionData = LoadAcquisitionData();
        }

        /// <summary>
        /// 景品を獲得済みとして保存
        /// </summary>
        /// <param name="prizeId">景品ID</param>
        /// <returns>初回獲得の場合はtrue、既に獲得済みの場合はfalse</returns>
        public bool AddAcquiredPrize(int prizeId)
        {
            var existingData = _acquisitionData.FirstOrDefault(p => p.prizeId == prizeId);
            
            if (existingData == null)
            {
                // 初回獲得
                _acquisitionData.Add(new PrizeAcquisitionData(prizeId, 1));
                SaveAcquisitionData();
                return true;
            }

            // 既に獲得済み - カウントを増やす
            existingData.count++;
            SaveAcquisitionData();
            return false;
        }

        /// <summary>
        /// 指定した景品が獲得済みかどうかを確認
        /// </summary>
        /// <param name="prizeId">景品ID</param>
        /// <returns>獲得済みの場合true</returns>
        public bool IsAcquired(int prizeId)
        {
            return _acquisitionData.Any(p => p.prizeId == prizeId);
        }

        /// <summary>
        /// 指定した景品の獲得数を取得
        /// </summary>
        /// <param name="prizeId">景品ID</param>
        /// <returns>獲得数（未獲得の場合は0）</returns>
        public int GetPrizeCount(int prizeId)
        {
            return _acquisitionData.FirstOrDefault(p => p.prizeId == prizeId)?.count ?? 0;
        }

        /// <summary>
        /// 獲得済み景品IDのリストを取得
        /// </summary>
        /// <returns>獲得済み景品IDのリスト</returns>
        public IReadOnlyList<int> GetAcquiredPrizeIds()
        {
            return _acquisitionData.Select(p => p.prizeId).ToList().AsReadOnly();
        }

        /// <summary>
        /// 全ての獲得データを取得
        /// </summary>
        /// <returns>獲得データのリスト</returns>
        public IReadOnlyList<PrizeAcquisitionData> GetAllAcquisitionData()
        {
            return _acquisitionData.AsReadOnly();
        }

        /// <summary>
        /// 獲得済み景品数を取得（ユニーク数）
        /// </summary>
        /// <returns>獲得済み景品数</returns>
        public int GetAcquiredCount()
        {
            return _acquisitionData.Count;
        }

        /// <summary>
        /// 獲得した景品の総個数を取得
        /// </summary>
        /// <returns>全景品の獲得個数の合計</returns>
        public int GetTotalItemCount()
        {
            return _acquisitionData.Sum(p => p.count);
        }

        /// <summary>
        /// 獲得済み景品をクリア（デバッグ用）
        /// </summary>
        public void ClearAcquiredPrizes()
        {
            _acquisitionData.Clear();
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// PlayerPrefsから獲得データを読み込み
        /// </summary>
        private List<PrizeAcquisitionData> LoadAcquisitionData()
        {
            // 新形式のデータを読み込み
            var newFormatData = PlayerPrefs.GetString(SaveKey, string.Empty);
            if (!string.IsNullOrEmpty(newFormatData))
            {
                try
                {
                    var dataList = JsonUtility.FromJson<PrizeAcquisitionDataList>(newFormatData);
                    return dataList?.prizes ?? new List<PrizeAcquisitionData>();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"データの読み込みに失敗: {e.Message}");
                }
            }

            return new List<PrizeAcquisitionData>();
        }
        

        /// <summary>
        /// PlayerPrefsに獲得データを保存
        /// </summary>
        private void SaveAcquisitionData()
        {
            var dataList = new PrizeAcquisitionDataList { prizes = _acquisitionData };
            var jsonData = JsonUtility.ToJson(dataList);
            PlayerPrefs.SetString(SaveKey, jsonData);
            PlayerPrefs.Save();
        }
    }
}