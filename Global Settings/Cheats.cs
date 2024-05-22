using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "Cheats", menuName = "Cheats")]
public class Cheats : ScriptableObject
{
    public BoolReactiveProperty oneShotCharacter, oneShotMob;
    public BoolReactiveProperty mobOneSecondAttackTimer;
    public BoolReactiveProperty godMode;
    public BoolReactiveProperty trainingDummy;
    public BoolReactiveProperty infinitMoney;
    public BoolReactiveProperty everlastingGulag;
    public BoolReactiveProperty noCooldown;
    [Space]
    public IntReactiveProperty startingRecruitAllyLevel;
}
