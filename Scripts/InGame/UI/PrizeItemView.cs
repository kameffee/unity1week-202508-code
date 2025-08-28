using Unity1week202508.Data;
using UnityEngine.UIElements;

namespace Unity1week202508.InGame.UI
{
    /// <summary>
    /// 景品アイテム1個を管理するView
    /// </summary>
    public class PrizeItemView
    {
        private readonly VisualElement _root;
        private readonly VisualElement _prizeImage;
        private readonly Label _prizeName;

        public PrizeItemView(VisualElement root)
        {
            _root = root;
            _prizeImage = root.Q<VisualElement>("prize-image");
            _prizeName = root.Q<Label>("prize-name");
        }

        /// <summary>
        /// 景品データを設定
        /// </summary>
        /// <param name="prize">景品データ</param>
        /// <param name="isAcquired">獲得済みかどうか</param>
        public void SetPrizeData(PrizeMasterData prize, bool isAcquired)
        {
            // 獲得状態に応じてCSSクラスを設定
            _root.RemoveFromClassList("acquired");
            _root.RemoveFromClassList("unacquired");
            _root.AddToClassList(isAcquired ? "acquired" : "unacquired");

            // 画像を設定
            if (prize.Image != null)
            {
                _prizeImage.style.backgroundImage = new StyleBackground(prize.Image);
            }
            else
            {
                _prizeImage.style.backgroundImage = StyleKeyword.Null;
            }

            // 名前を設定
            _prizeName.text = isAcquired ? prize.PrizeName : "???";

            // ツールチップ（獲得済みの場合のみ）
            _root.tooltip = isAcquired ? prize.PrizeName : string.Empty;
        }

        /// <summary>
        /// 獲得状態を設定
        /// </summary>
        /// <param name="isAcquired">獲得済みかどうか</param>
        public void SetAcquiredState(bool isAcquired)
        {
            _root.RemoveFromClassList("acquired");
            _root.RemoveFromClassList("unacquired");
            _root.AddToClassList(isAcquired ? "acquired" : "unacquired");

            // 名前表示も更新（別途SetPrizeDataを呼ぶ必要あり）
        }
    }
}