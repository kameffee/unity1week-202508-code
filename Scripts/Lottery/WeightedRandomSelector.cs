using System.Collections.Generic;
using NRandom;
using UnityEngine;

namespace Unity1week202508.Lottery
{
    /// <summary>
    /// 重み付きランダム選択を行うクラス
    /// </summary>
    public class WeightedRandomSelector<T>
    {
        private readonly List<WeightedItem<T>> _items = new();
        private float _totalWeight = 0f;
        private bool _isWeightsDirty = true;

        /// <summary>
        /// アイテムを重みと一緒に追加
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <param name="weight">重み（確率）</param>
        public void AddItem(T item, float weight)
        {
            if (weight <= 0f)
            {
                Debug.LogWarning($"Weight must be positive. Item: {item}, Weight: {weight}");
                return;
            }

            _items.Add(new WeightedItem<T>(item, weight));
            _isWeightsDirty = true;
        }

        /// <summary>
        /// アイテムをクリア
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _totalWeight = 0f;
            _isWeightsDirty = true;
        }

        /// <summary>
        /// 重み付きランダム選択でアイテムを取得
        /// </summary>
        /// <returns>選択されたアイテム</returns>
        public T SelectRandom()
        {
            if (_items.Count == 0)
            {
                Debug.LogError("No items available for selection");
                return default(T);
            }

            // 重みの合計を再計算（必要な場合のみ）
            if (_isWeightsDirty)
            {
                RecalculateTotalWeight();
            }

            // 0から総重みの範囲でランダム値を生成
            float randomValue = RandomEx.Shared.NextFloat(0f, _totalWeight);
            
            // ランダム値に対応するアイテムを選択
            float currentWeight = 0f;
            foreach (var item in _items)
            {
                currentWeight += item.Weight;
                if (randomValue <= currentWeight)
                {
                    return item.Item;
                }
            }

            // フォールバック（浮動小数点誤差対策）
            return _items[^1].Item;
        }

        /// <summary>
        /// 複数のアイテムを重み付きランダム選択で取得
        /// </summary>
        /// <param name="count">取得するアイテム数</param>
        /// <returns>選択されたアイテムのリスト</returns>
        public List<T> SelectMultiple(int count)
        {
            var result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(SelectRandom());
            }
            return result;
        }

        /// <summary>
        /// 総重みを取得
        /// </summary>
        public float GetTotalWeight()
        {
            if (_isWeightsDirty)
            {
                RecalculateTotalWeight();
            }
            return _totalWeight;
        }

        /// <summary>
        /// アイテム数を取得
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// 各アイテムの実際の確率を取得（デバッグ用）
        /// </summary>
        public Dictionary<T, float> GetProbabilities()
        {
            if (_isWeightsDirty)
            {
                RecalculateTotalWeight();
            }

            var probabilities = new Dictionary<T, float>();
            foreach (var item in _items)
            {
                probabilities[item.Item] = (item.Weight / _totalWeight) * 100f;
            }
            return probabilities;
        }

        /// <summary>
        /// 総重みを再計算
        /// </summary>
        private void RecalculateTotalWeight()
        {
            _totalWeight = 0f;
            foreach (var item in _items)
            {
                _totalWeight += item.Weight;
            }
            _isWeightsDirty = false;
        }
    }

    /// <summary>
    /// 重み付きアイテムの内部クラス
    /// </summary>
    internal class WeightedItem<T>
    {
        public T Item { get; }
        public float Weight { get; }

        public WeightedItem(T item, float weight)
        {
            Item = item;
            Weight = weight;
        }
    }
}