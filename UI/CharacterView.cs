using Zenject;
using UnityEngine;

public class CharacterView : MobView
{
    public void Subscribe(Character character)
    {
        healthBar.Subscribe(character.gameObject, character.health);
        energyBar.Subscribe(character.gameObject, character.mana);
    }
}
