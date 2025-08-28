using UnityEngine;

namespace Unity1week202508.Data
{
    [CreateAssetMenu(fileName = "Prize_", menuName = "MasterData/Prize/MasterData", order = 0)]
    public class PrizeMasterData : ScriptableObject
    {
        public int Id => _id;
        public string PrizeName => _prizeName;
        public Sprite Image => _image;
        public float Probability => _probability;
        public int DisplayOrder => _displayOrder;

        [Header("基本情報")]
        [SerializeField]
        private int _id;

        [SerializeField]
        private string _prizeName;

        [SerializeField]
        private Sprite _image;

        [Header("確率設定")]
        [SerializeField, Range(0.1f, 100f)]
        private float _probability = 10f; // デフォルト確率

        [Header("レア度設定")]
        [SerializeField]
        private PrizeRarity _rarity = PrizeRarity.Common;

        [Header("表示設定")]
        [SerializeField]
        private int _displayOrder = 0;

        public PrizeRarity Rarity => _rarity;
    }
}