using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Zenject;

public class CharacterController : MonoBehaviour
{
    [Inject] Button attackButton;

    public Button AttackButton => attackButton;
}
