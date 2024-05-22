using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class AbilityView : MonoBehaviour
{
    [SerializeField] public AbilityButton abilityButton;
    [SerializeField] public Wheel wheel;
    [SerializeField] Image cooldownImage;


    public void Connect(Ability ability)
    {
        abilityButton.onPointerDown.AsObservable()
            .Subscribe(_ => ability.UseAsync().Forget())
            .AddTo(this);

        ability.cooldown.ObserveRatio()
            .Subscribe(ratio =>
                       cooldownImage.fillAmount = ratio)
            .AddTo(this);

        this.UpdateAsObservable()
            .Subscribe(_ => ability.Tick())
            .AddTo(this);
    }

    public async UniTask OnPointerDown()
    {
        wheel.RandomizeAngleOffset();
        await wheel.fade.FadeIn();
        wheel.ToggleRotation(true);
    }

    public void OnPointerUp()
    {
        wheel.fade.FadeOut().Forget();
        wheel.ToggleRotation(false);
    }
}
