using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "WeakPoints", menuName = "SO/Talents/WeakPoints")]
public class WeakPoints : Talent
{
    [SerializeField] float lifespan = 2;
    [SerializeField] [HideInInspector] List<Field> chanceToAppearFields;
    [SerializeField] [HideInInspector] List<Field> damageMultFields;

    [Inject] GameSettings gameSettings;

    IDisposable timerSubscription;

    public void SubscribeSpawnOnTimer(Canvas canvas, WeakPointView.Pool viewPool)
    {
        gameSettings
            .SubscribeToTimer(timerSubscription,
                              canvas,
                              () => Roll(canvas, viewPool));
    }

    public float chanceToAppear {get; protected set;}
    public float damageMult {get; protected set;}

    public void Roll(Canvas canvas, WeakPointView.Pool pool)
    {
        if (UnityEngine.Random.value < chanceToAppear)
        {
            SpawnWeakPoint(canvas, pool);
        }
    }

    public void Shoot(Mob mob, Character character)
    {
        float damage = character.Stats.strength * damageMult;

        character.InflictDamage(mob, damage);
    }

    void SpawnWeakPoint(Canvas canvas, WeakPointView.Pool weakPointViewPool)
    {
        WeakPointView view = weakPointViewPool.Spawn();

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;

        Vector3 canvasScale = Vector3.one * 0.4f;

        Vector3 halfSize = rect.size / 2;
        halfSize.Scale(canvasScale);

        Vector3 position = UnityEngine.Random.insideUnitCircle.xy_(5);
        position.Scale(halfSize * 0.8f);

        view.transform.position = position + halfSize;


        var cancel = canvas.GetCancellationTokenOnDestroy();

        UniTask
            .WaitForSeconds(lifespan)
            .AttachExternalCancellation(cancel)
            .SuppressCancellationThrow()
            .ContinueWith(isCanceled =>
            {
                if (isCanceled)
                    return;

                if (view?.gameObject?.activeSelf ?? false)
                    weakPointViewPool.DisableButtonAndDespawn(view);
            })
            .Forget();

    }

    protected override void OnLevelUp()
    {
        chanceToAppear = chanceToAppearFields[Level];
        damageMult = damageMultFields[Level];
    }

    public override IObservable<string> ObserveDescription()
    {
        return
            Observable.Return("*BLANK*");
    }
}
