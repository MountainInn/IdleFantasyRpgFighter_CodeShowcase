using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("UI Parents")]
    [SerializeField] RectTransform talentsParent;
    [Header("UI Prefabs")]
    [SerializeField] TalentView talentViewPrefab;
    [Space]
    [SerializeField] FloatingText prefabFloatingText;
    [SerializeField] CritFloatingText prefabCritFloatingText;
    [Space]
    [Header("Prefabs")]
    [SerializeField] Land prefabLand;
    [SerializeField] Road prefabRoad;
    [Header("Instances")]
    [SerializeField] Canvas canvas;
    [SerializeField] MobView mobView;
    [SerializeField] CharacterView characterView;
    [SerializeField] DPSMeterView dpsMeterView;
    [SerializeField] Vault vault;
    [SerializeField] VaultView vaultView;
    [SerializeField] RoadCaster roadCaster;
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    [Header("Game Settings")]
    [SerializeField] GameSettings gameSettings;
    [SerializeField] Cheats cheats;


    public override void InstallBindings()
    {
        Container
            .Bind(
                typeof(SaveSystem),
                typeof(Treadmill),
                typeof(LootManager),
                typeof(WorldExtender),
                typeof(DPSMeter)
            )
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .OnInstantiated((context, obj)=>
            {
                if (obj is Component comp)
                {
                    comp.gameObject.name = $"{comp.GetType().Name}";
                }
            });


        Container
            .Bind<FloatingTextSpawner>()
            .FromInstance(mobDamagedFloatingText)
            .AsSingle()
            .WhenInjectedInto<Mob>();

        Container .Bind<CharacterView>() .FromInstance(characterView);
        Container .Bind<GameSettings>() .FromInstance(gameSettings);
        Container .Bind<DPSMeterView>() .FromInstance(dpsMeterView);
        Container .Bind<RoadCaster>() .FromInstance(roadCaster);
        Container .Bind<MobView>() .FromInstance(mobView);
        Container .Bind<Cheats>() .FromInstance(cheats);
        Container .Bind<Vault>() .FromInstance(vault);


        Container.BindView(talentViewPrefab, talentsParent);


        Container
            .BindMemoryPool<Land, Land.Pool>()
            .FromComponentInNewPrefab(prefabLand);

        Container
            .BindMemoryPool<Road, Road.Pool>()
            .FromComponentInNewPrefab(prefabRoad);


        Container
            .BindMemoryPool<FloatingText, FloatingText.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(prefabFloatingText)
            .UnderTransform(canvas.transform);

        Container
            .BindMemoryPool<CritFloatingText, CritFloatingText.Pool>()
            .WithInitialSize(3)
            .FromComponentInNewPrefab(prefabCritFloatingText)
            .UnderTransform(canvas.transform);

        
    }
}
