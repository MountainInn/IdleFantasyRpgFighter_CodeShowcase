using UnityEngine;
using Zenject;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class MobGroup : MonoBehaviour
{
    [Inject] GameSettings gameSettings;

    List<Mob> mobs;

    public Mob GetFirstAliveMob()
    {
        return mobs.FirstOrDefault(m => m.IsAlive);
    }

    public class Pool : MonoMemoryPool<IEnumerable<Mob>, MobGroup>
    {
        public ReactiveQueue<MobGroup> mobGroups = new();
       
        protected override void OnSpawned(MobGroup item)
        {
            base.OnSpawned(item);

            mobGroups.Enqueue(item);
        }

        protected override void OnDespawned(MobGroup item)
        {
            base.OnDespawned(item);

            mobGroups.Dequeue();
        }


        protected override void Reinitialize(IEnumerable<Mob> mobs,
                                             MobGroup item)
        {
            base.Reinitialize(mobs, item);

            item.mobs = mobs.ToList();
        }
    }
}
