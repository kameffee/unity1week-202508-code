using Unity1week202508.Data;

namespace Unity1week202508.Lottery
{
    /// <summary>
    /// クジがクリックされた時のイベントデータ
    /// </summary>
    public readonly struct LotteryClickedEventData
    {
        /// <summary>
        /// クジオブジェクトのユニークID（フィールド管理用）
        /// </summary>
        public long UniqueId { get; }
        
        /// <summary>
        /// クジの賞品データ
        /// </summary>
        public PrizeMasterData PrizeData { get; }

        public LotteryObject LotteryObject { get; }

        public LotteryClickedEventData(
            long uniqueId,
            LotteryObject lotteryObject,
            PrizeMasterData prizeData)
        {
            UniqueId = uniqueId;
            LotteryObject = lotteryObject;
            PrizeData = prizeData;
        }
    }
}