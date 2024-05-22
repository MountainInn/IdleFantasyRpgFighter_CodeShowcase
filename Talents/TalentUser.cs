using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TalentUser : MonoBehaviour
{
    public List<Talent> talents;
    public List<Ability> abilities;

    [Inject] void Construct(List<Talent> talents, List<Ability> abilities)
    {
        this.talents = talents;
        this.abilities = abilities;
    }
    public bool alreadyInjected;
}
