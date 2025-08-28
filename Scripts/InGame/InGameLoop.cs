using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity1week202508.Audio.Services;
using Unity1week202508.Data;
using Unity1week202508.Lottery;
using UnityEngine;
using VContainer.Unity;

namespace Unity1week202508.InGame
{
    public class InGameLoop : IAsyncStartable
    {
        private readonly LotteryResultPhase _lotteryResultPhase;
        private readonly LotterySelectionService _selectionService;
        private readonly LotteryFieldManager _fieldManager;
        private readonly PrizeAcquisitionRepository _prizeAcquisitionRepository;
        private readonly AudioPlayer _audioPlayer;

        public InGameLoop(
            LotteryResultPhase lotteryResultPhase,
            LotterySelectionService selectionService,
            LotteryFieldManager fieldManager,
            PrizeAcquisitionRepository prizeAcquisitionRepository,
            AudioPlayer audioPlayer)
        {
            _lotteryResultPhase = lotteryResultPhase;
            _selectionService = selectionService;
            _fieldManager = fieldManager;
            _prizeAcquisitionRepository = prizeAcquisitionRepository;
            _audioPlayer = audioPlayer;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            const string mainBgmId = "main-bgm";
            if (_audioPlayer.CurrentBgmId != mainBgmId)
            {
                _audioPlayer.PlayBgm(mainBgmId);
            }

            // フィールドにクジを配置
            _fieldManager.Initialize();

            // 獲得済み景品数をログ出力
            var acquiredCount = _prizeAcquisitionRepository.GetAcquiredCount();
            Debug.Log($"獲得済み景品数: {acquiredCount}個");

            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    Debug.Log("クジ選択待ち");

                    // プレイヤーがクジを選択するまで待機
                    var selectedPrize = await _selectionService.WaitForSelectionAsync(cancellation);

                    Debug.Log(
                        $"選択されたクジ: {selectedPrize.PrizeName} (残り: {_selectionService.GetRemainingLotteryCount()}個)");

                    // 結果演出
                    await _lotteryResultPhase.ExecuteAsync(selectedPrize, cancellation);
                }
            }
            catch (OperationCanceledException)
            {
                // アプリケーション終了時のキャンセルは正常終了
                Debug.Log("InGameLoop: キャンセルされました");
            }
        }
    }
}