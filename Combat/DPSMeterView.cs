using UnityEngine;
using UniRx;
using Zenject;
using TMPro;

public class DPSMeterView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;

    public void SetText(float dps)
    {
        label.text = $"DPS: {dps:F0}";
    }
}
