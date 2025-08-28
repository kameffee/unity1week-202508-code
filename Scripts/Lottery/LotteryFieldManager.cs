using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NRandom;
using R3;
using Unity1week202508.Data;
using Unity1week202508.InGame;
using UnityEngine;
using VContainer;

namespace Unity1week202508.Lottery
{
    /// <summary>
    /// フィールド上のクジオブジェクトを管理
    /// </summary>
    public class LotteryFieldManager : MonoBehaviour
    {
        [SerializeField]
        private LotteryObject _lotteryObjectPrefab;

        [SerializeField]
        private Transform _objectContainer;

        [SerializeField]
        private int _objectCount = 10;

        [SerializeField]
        private float _spawnAreaWidth = 8f;

        [SerializeField]
        private float _spawnAreaHeight = 4f;

        [SerializeField]
        private float _minDistanceBetweenObjects = 0.8f;

        [SerializeField]
        private int _maxPlacementAttempts = 50;

        [Header("自動追加設定")]
        [SerializeField]
        private int _maxObjectCount = 50;

        [SerializeField]
        private float _autoAddInterval = 5f;

        [SerializeField]
        private bool _enableAutoAdd = true;

        private long _nextUniqueId = 1;
        private readonly Dictionary<long, LotteryObject> _lotteryObjectsDict = new();
        private readonly Subject<LotteryClickedEventData> _onLotterySelectedSubject = new();
        private LotteryRandomSelector _randomSelector;
        private CancellationTokenSource _autoAddCancellationTokenSource;

        public Observable<LotteryClickedEventData> OnLotterySelectedAsObservable() => _onLotterySelectedSubject;

        [Inject]
        public void Construct(LotteryRandomSelector randomSelector)
        {
            _randomSelector = randomSelector;
        }

        public void Initialize()
        {
            SpawnLotteryObjects();

            // 自動追加を開始
            if (_enableAutoAdd)
            {
                StartAutoAddLotteryObjects();
            }
        }

        private void SpawnLotteryObjects()
        {
            ClearObjects();
            _nextUniqueId = 1; // IDをリセット

            var selectedPrizes = _randomSelector.GetRandomPrizes(_objectCount);
            var prizeList = new List<PrizeMasterData>(selectedPrizes);

            if (prizeList.Count == 0)
            {
                Debug.LogWarning("クジデータが存在しません");
                return;
            }

            // ランダムな位置にクジオブジェクトを生成
            foreach (var prizeData in prizeList)
            {
                CreateLotteryObjectAtRandomPosition(prizeData);
            }
        }

        private async UniTask OnLotteryClicked(LotteryClickedEventData eventData, CancellationToken cancellation)
        {
            await eventData.LotteryObject.PlayOutAnimationAsync(cancellation);

            // 選択されたクジオブジェクトを削除
            RemoveSelectedObject(eventData.UniqueId);

            _onLotterySelectedSubject.OnNext(eventData);
        }

        public void SetAllObjectsInteractable(bool interactable)
        {
            foreach (var obj in _lotteryObjectsDict.Values)
            {
                if (obj != null)
                    obj.SetInteractable(interactable);
            }
        }

        public void ResetField()
        {
            SetAllObjectsInteractable(true);
        }

        /// <summary>
        /// 選択されたクジオブジェクトを削除
        /// </summary>
        private void RemoveSelectedObject(long uniqueId)
        {
            // ユニークIDでオブジェクトを特定して削除
            if (_lotteryObjectsDict.Remove(uniqueId, out var lotteryObject))
            {
                if (lotteryObject != null)
                {
                    Destroy(lotteryObject.gameObject);
                }
            }
        }

        /// <summary>
        /// 残りのクジ数を取得
        /// </summary>
        public int GetRemainingCount()
        {
            return _lotteryObjectsDict.Count;
        }

        /// <summary>
        /// 自動追加タスクを開始
        /// </summary>
        private void StartAutoAddLotteryObjects()
        {
            _autoAddCancellationTokenSource?.Cancel();
            _autoAddCancellationTokenSource = new CancellationTokenSource();
            AutoAddLotteryObjectsAsync(_autoAddCancellationTokenSource.Token).Forget();
        }

        /// <summary>
        /// 自動追加タスクを停止
        /// </summary>
        public void StopAutoAddLotteryObjects()
        {
            _autoAddCancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 5秒ごとにクジを自動追加
        /// </summary>
        private async UniTaskVoid AutoAddLotteryObjectsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 指定間隔待機
                await UniTask.Delay((int)(_autoAddInterval * 1000), cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    break;

                // 最大数に達していない場合のみ追加
                if (_lotteryObjectsDict.Count < _maxObjectCount)
                {
                    AddNewLotteryObject();
                    Debug.Log($"自動でクジを追加しました (現在: {_lotteryObjectsDict.Count}/{_maxObjectCount}個)");
                }
            }
        }

        /// <summary>
        /// 新しいクジオブジェクトを1つ追加
        /// </summary>
        private void AddNewLotteryObject()
        {
            if (_randomSelector == null)
            {
                Debug.LogWarning("LotteryRandomSelectorが初期化されていません");
                return;
            }

            // ランダムに1つの賞品を選択
            var selectedPrizes = _randomSelector.GetRandomPrizes(1);
            var prizeList = new List<PrizeMasterData>(selectedPrizes);

            if (prizeList.Count == 0)
            {
                Debug.LogWarning("追加するクジデータが存在しません");
                return;
            }

            var prizeData = prizeList[0];
            if (CreateLotteryObjectAtRandomPosition(prizeData))
            {
                Debug.Log($"新しいクジを追加: PrizeID={prizeData.Id}");
            }
        }

        /// <summary>
        /// ランダムな位置にクジオブジェクトを生成
        /// </summary>
        /// <param name="prizeData">賞品データ</param>
        /// <returns>生成に成功した場合はtrue</returns>
        private bool CreateLotteryObjectAtRandomPosition(PrizeMasterData prizeData)
        {
            var random = RandomEx.Shared;

            // 既存のオブジェクトの位置を収集
            var existingPositions = GetExistingObjectPositions();

            // 重ならない位置を探す
            var position = FindNonOverlappingPosition(existingPositions);
            if (!position.HasValue)
            {
                Debug.LogWarning("クジオブジェクトを配置できませんでした（最大試行回数に到達）");
                return false;
            }

            // クジオブジェクトを生成
            CreateLotteryObject(prizeData, position.Value, random.NextFloat(0, 360));
            return true;
        }

        /// <summary>
        /// 既存のオブジェクトの位置リストを取得
        /// </summary>
        private List<Vector3> GetExistingObjectPositions()
        {
            var positions = new List<Vector3>();
            foreach (var obj in _lotteryObjectsDict.Values)
            {
                if (obj != null)
                {
                    positions.Add(obj.transform.position);
                }
            }

            return positions;
        }

        /// <summary>
        /// 既存のオブジェクトと重ならない位置を探す
        /// </summary>
        private Vector3? FindNonOverlappingPosition(List<Vector3> existingPositions)
        {
            var random = RandomEx.Shared;

            for (int attempt = 0; attempt < _maxPlacementAttempts; attempt++)
            {
                // ランダムな位置を計算
                var randomX = random.NextFloat(-_spawnAreaWidth / 2f, _spawnAreaWidth / 2f);
                var randomY = random.NextFloat(-_spawnAreaHeight / 2f, _spawnAreaHeight / 2f);
                var randomZ = RandomEx.Shared.NextFloat(0f, 360f);
                var position = new Vector3(randomX, randomY, randomZ);

                // 既存のオブジェクトとの距離をチェック
                if (!IsPositionTooClose(position, existingPositions))
                {
                    return position;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定位置が既存のオブジェクトに近すぎるかチェック
        /// </summary>
        private bool IsPositionTooClose(Vector3 position, List<Vector3> existingPositions)
        {
            foreach (var existingPos in existingPositions)
            {
                if (Vector3.Distance(position, existingPos) < _minDistanceBetweenObjects)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// クジオブジェクトを生成して配置
        /// </summary>
        private void CreateLotteryObject(PrizeMasterData prizeData, Vector3 position, float rotationZ)
        {
            // ユニークIDを生成
            var uniqueId = _nextUniqueId++;

            // クジオブジェクトを生成
            var rotation = Quaternion.Euler(0, 0, rotationZ);
            var lotteryObject = Instantiate(_lotteryObjectPrefab, position, rotation, _objectContainer);
            lotteryObject.Initialize(prizeData, uniqueId);
            // 登場アニメーションを再生
            lotteryObject.PlayInAnimation();


            // クリックイベントを購読
            lotteryObject.OnClickAsObservable()
                .SubscribeAwait(async (eventData, ct) => await OnLotteryClicked(eventData, ct))
                .AddTo(this);

            _lotteryObjectsDict[uniqueId] = lotteryObject;
        }

        private void ClearObjects()
        {
            foreach (var obj in _lotteryObjectsDict.Values)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }

            _lotteryObjectsDict.Clear();
        }

        private void OnDestroy()
        {
            // 自動追加タスクを停止
            _autoAddCancellationTokenSource?.Cancel();
            _autoAddCancellationTokenSource?.Dispose();

            ClearObjects();
            _onLotterySelectedSubject?.Dispose();
        }
    }
}