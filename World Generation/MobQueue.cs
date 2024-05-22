using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/MobQueue",fileName ="MobQueue")]
public class MobQueue : ScriptableObject
{
    public List<QueueSegment> queueSegments;
    public float powerMultiplier = 1f;
    public DropList dropList;

    public IEnumerable<IEnumerable<MobStatsSO>> GenerateQueue()
    {
        return
            queueSegments
            .Select(mobsInSegment =>
                    mobsInSegment
                    .GetShuffled()
                    .Select(mobStat =>
                    {
                        mobStat = Instantiate(mobStat);
                        mobStat.Multiply(powerMultiplier * mobsInSegment.powerMultiplier);

                        if (mobStat.dropList)
                            mobStat.dropList = Instantiate(mobStat.dropList);
                        else
                            mobStat.dropList = new DropList();

                        if (mobsInSegment.dropList?.entries.Any() ?? false)
                            mobStat.dropList.entries.AddRange(mobsInSegment.dropList.entries);

                        if (dropList?.entries.Any() ?? false)
                            mobStat.dropList.entries.AddRange(dropList.entries);

                        return mobStat;
                    })
            );
    }

    public void GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths,
                                            out int totalLength)
    {
        subLengths = queueSegments.Select(seg => seg.segmentLength);
        totalLength = GetTotalLength();
    }

    public int GetTotalLength()
    {
        return
            queueSegments
            .Sum(seg => seg.segmentLength);
    }

    public IEnumerable<MobStatsSO> Next()
    {
        foreach (var segment in queueSegments)
        {
            for (int m = 0; m < segment.segmentLength; m++)
            {
                MobStatsSO so = segment.mobStats.GetRandom();

                MobStatsSO nextMobStats = Instantiate(so);

                nextMobStats.Multiply(segment.powerMultiplier * powerMultiplier);

                yield return nextMobStats;
            }
        }
    }

    [System.SerializableAttribute]
    public class QueueSegment
    {
        public List<MobStatsSO> mobStats;
        public int segmentLength;
        public float powerMultiplier = 1f;
        public DropList dropList;

        public IEnumerable<MobStatsSO> GetShuffled()
        {
            return
                mobStats.GetRandoms(segmentLength);
        }
    }
}
