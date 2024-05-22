using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System.Linq;
using System.Collections;

public class Encounter : MonoBehaviour
{
    [SerializeField] public UnityEvent onBattleStarted;
    [SerializeField] public UnityEvent onBattleWon;
    [SerializeField] public UnityEvent onBattleLost;

    [Inject] Character character;
    [Inject] SplineMove splineMove;
    [Inject] MobView mobView;

    [Inject]
    public void Construct(BattleTrigger battleTrigger)
    {
        battleTrigger.onMobFound.AddListener(mob =>
        {
            StartBattle(mob);
        });
    }

    public ReactiveProperty<Mob> battleTarget = new(null);
    public bool IsOngoing => battleTarget.Value != null;

    void Start()
    {
        mobView.gameObject.SetActive(false);
    }


    void StartBattle(Mob mob)
    {
        SubMobToView(mob, mobView);

        mob.onDie
            .AsObservable()
            .Take(1)
            .Subscribe(_ =>
            {
                mobView.gameObject.SetActive(false);

                onBattleWon?.Invoke();
            })
            .AddTo(this);

        character.onDie
            .AsObservable()
            .Take(1)
            .Subscribe(_ => onBattleLost?.Invoke())
            .AddTo(this);

        character.target = mob;
        mob.target = character;

        battleTarget.Value = mob;

        splineMove.isMoving = false;

        onBattleStarted?.Invoke();
      
        StartCoroutine(BattleCoroutine());

        IEnumerator BattleCoroutine()
        {
            var combatants = new Combatant[] { character, mob };

            while (character.IsAlive && mob.IsAlive)
            {
                if (mob.AttackTimerTick(Time.deltaTime))
                {
                    mob.PushAttack();
                }

                yield return null;
            }

            character.target = null;
            mob.target = null;

            battleTarget.Value = null;

            splineMove.isMoving = true;
        }
    }

    void SubMobToView(Mob mob, MobView mobView)
    {
        Fade fade = mobView.GetComponent<Fade>();

        fade.FadeIn();
        mob.onDie.AddListener(() => fade.FadeOut());

        mobView.Subscribe(mob);
        mobView.gameObject.SetActive(true);
    }
}
