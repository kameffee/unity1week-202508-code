using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity1week202508.Data
{
    [CreateAssetMenu(fileName = "PrizeMasterDataSource", menuName = "MasterData/Prize/DataSource", order = 1)]
    public class PrizeMasterDataSource : ScriptableObject
    {
        public PrizeMasterData[] Data 
        {
            get => _data;
            set => _data = value;
        }

        [SerializeField]
        private PrizeMasterData[] _data;

        public PrizeMasterData Get(int id)
        {
            var result = _data.FirstOrDefault(x => x.Id == id);
            Assert.IsNotNull(result, $"PrizeMasterData with ID {id} not found.");
            return result;
        }

        public void OnValidate()
        {
            // DisplayOrder順にソート、同じ値の場合はID順
            if (_data != null && _data.Length > 0)
            {
                Array.Sort(_data, (x, y) =>
                {
                    if (x == null || y == null) return 0;
                    int orderCompare = x.DisplayOrder.CompareTo(y.DisplayOrder);
                    return orderCompare != 0 ? orderCompare : x.Id.CompareTo(y.Id);
                });
            }
        }
    }
}