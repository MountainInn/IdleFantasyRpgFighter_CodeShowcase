
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] protected Sprite borderSprite;
    [SerializeField] protected Sprite maskSprite;
    [SerializeField] protected Sprite fillSprite;
    [SerializeField] protected float underFillDelay;
    [Space]
    [SerializeField] [Range(0,1f)] protected float fillAmount;
    [SerializeField] protected float pixelsPerUnit;
    [SerializeField] protected Color borderColor;
    [SerializeField] protected Color fillColor;
    [Space]
    [SerializeField] protected Slider slider;
    [SerializeField] protected Image borderImage;
    [SerializeField] protected Image maskImage;
    [SerializeField] protected Image fillImage;
    [Space]
    [SerializeField] protected TextMeshProUGUI label;
    [Header("Afterimage")]
    [SerializeField] protected ProgressBar afterimage;
    [SerializeField] protected Sprite afterimageSprite;
    [SerializeField] protected Color afterimageColor;

    protected Queue<Tween> queue = new();

    protected void OnValidate()
    {
        if (slider)
        {
            slider.value = fillAmount;
        }

        if (afterimage)
        {
            afterimage.fillSprite = afterimageSprite;
            afterimage.pixelsPerUnit = pixelsPerUnit;
            afterimage.fillColor = afterimageColor;
            afterimage.borderColor = borderColor;
            afterimage.fillAmount = fillAmount + .1f;
            afterimage.maskSprite = maskSprite;
            afterimage.borderSprite = borderSprite;

            afterimage.OnValidate();
        }

        if (fillImage)
        {
            fillImage.sprite = fillSprite;
            fillImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Sliced;
        }

        if (maskImage)
        {
            maskImage.sprite = maskSprite;
            maskImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            maskImage.type = Image.Type.Sliced;
        }

        if (borderImage)
        {
            borderImage.sprite = borderSprite;
            borderImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            borderImage.color = borderColor;
            borderImage.type = Image.Type.Sliced;
        }
    }

    public IDisposable Subscribe(GameObject volumeOwner, Volume volume)
    {
        return
            volume
            .ObserveAll()
            .TakeWhile(_ => volumeOwner != null && volumeOwner.activeSelf)
            .Subscribe(tup =>
            {
                if (float.IsNaN(tup.ratio))
                    return;
               
                if (label)
                    label.text = volume.ToString();

                slider.value = tup.ratio;

                if (afterimage != null)
                {
                    var tween = afterimage.slider.DOValue(tup.ratio, underFillDelay);

                    QueueTween(tween);
                }
            });
    }

    protected void QueueTween(Tween tween)
    {
        queue.Enqueue(tween);

        tween.OnKill(() =>
        {
            queue.Dequeue();

            queue.Peek()?.Play();
        });

        if (queue.Peek() == tween)
        {
            tween.Play();
        }
    }
}
