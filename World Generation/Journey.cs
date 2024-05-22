using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using Zenject;

public class Journey : MonoBehaviour
{
    [SerializeField] JourneySO journeySO;

    [SerializeField] public UnityEvent onSegmentCompleted;
    [SerializeField] public UnityEvent onQueueCompleted;
    [SerializeField] public UnityEvent onJourneyCompleted;

    SuperVolume arenaProgress;

    SaveState saveState;
    IEnumerable<IEnumerable<MobStatsSO>> queue;

    public struct SaveState
    {
        public int mobStatIndex;
        public int segmentIndex;
        public int queueIndex;
        public string journeyName;

        public SaveState(int mobStatIndex, int segmentIndex, int queueIndex, string journeyName)
        {
            this.mobStatIndex = mobStatIndex;
            this.segmentIndex = segmentIndex;
            this.queueIndex = queueIndex;
            this.journeyName = journeyName;
        }
    }

    CompositeDisposable subscriptions;
    Coroutine queueCoroutine;

    // [Inject] SegmentedProgressBar arenaProgressBar;
    [Inject] SaveSystem saveSystem;
    [Inject]
    void RegisterWithSaveSystem(SaveSystem saveSystem)
    {
        saveSystem
            .MaybeRegister<SaveState>(this,
                                      "journeyState",
                                      () => saveState,
                                      (val) => saveState = val);
    }

    void ResetQueuePosition()
    {
        saveState.mobStatIndex = 0;
        saveState.segmentIndex = 0;
        saveState.queueIndex = 0;
    }

    void SubscribeArenaProgress(MobQueue mobQueue)
    {
        subscriptions?.Dispose();
        subscriptions = new();

        mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths, out int totalLength);

        arenaProgress = new SuperVolume(subLengths);

        arenaProgress
            .ObserveSubvolumeFull()
            .Subscribe(tuple => OnSegmentFinished())
            .AddTo(subscriptions);

        // arenaProgressBar
        //     .Subscribe(queue, arenaProgress, subscriptions);
    }

    void OnSegmentFinished()
    {
        onSegmentCompleted?.Invoke();
    }

    public MobStatsSO GetNextMob()
    {
        foreach (var item in GetMobs())
            return item;

        return null;
    }

    IEnumerable<MobStatsSO> GetMobs()
    {
        foreach (var (q, journeyField) in journeySO.queues.Enumerate().Skip(saveState.queueIndex))
        {
            saveState.queueIndex = q;

            MobQueue mobQueue = journeyField.mobQueue;
            mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths,
                                                 out int totalLength);

            queue = mobQueue.GenerateQueue();

            // SubscribeArenaProgress(mobQueue);

            foreach (var (s, segment) in queue.Enumerate().Skip(saveState.segmentIndex))
            {
                saveState.segmentIndex = s;
                foreach (var (m, mobStat) in segment.Enumerate().Skip(saveState.mobStatIndex))
                {
                    saveState.mobStatIndex = m;

                    yield return mobStat;
                }
            }

            journeyField.onQueueCompleted?.Invoke();
            onQueueCompleted?.Invoke();
        }
    }

    public void LoadSaveData(SaveState saveState)
    {
        StopAllCoroutines();

        this.saveState = saveState;

        journeySO = Resources.Load<JourneySO>($"SO/Journeys/{saveState.journeyName}");
    }

    public SaveState GetSaveData()
    {
        return this.saveState;
    }
}
