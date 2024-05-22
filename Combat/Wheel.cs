using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

[RequireComponent(typeof(Fade))]
public class Wheel : MonoBehaviour
{
    [SerializeField] Sprite fillSprite;
    [SerializeField] List<Field> fields = new();
    [Space]
    [SerializeField] float zoneAngleOffset;
    [SerializeField] float cursorAngle;
    [SerializeField] RectTransform zonesParent;
    [Space]
    [SerializeField] RectTransform cursor;
    [SerializeField] public Fade fade;
    [Space]
    [SerializeField] GameSettings gameSettings;

    bool isRotating;


    [Serializable]
    class Field
    {
        [SerializeField] public Zone zone;
        [SerializeField] public Color color;
        [SerializeField] public float width;
       
        [SerializeField] [HideInInspector] public Image left, right;
        [SerializeField] public float startingAngle, endingAngle;

        public void Deconstruct(out Image left, out Image right)
        {
            left = this.left;
            right = this.right;
        }
    }

    void OnValidate()
    {
        fields
            .Enumerate()
            .Map((tup) =>
            {
                var (i, f) = tup;

                if (f.left == null)
                {
                    (f.left, f.right) = InitZones();
                }

                f.left.transform.SetSiblingIndex(i*10);
                f.right.transform.SetSiblingIndex(i*10+1);

                f.left.color =
                    f.right.color = f.color;

            });

        UpdateZones();
    }

    void UpdateZones()
    {
        zoneAngleOffset = NormalizeAngle(zoneAngleOffset);
        cursorAngle = NormalizeAngle(cursorAngle);

        cursor.localEulerAngles = cursor.localEulerAngles.WithZ(cursorAngle);

        zonesParent.transform.localEulerAngles = new Vector3(0, 0, zoneAngleOffset);

        float startingAngle = 0;

        foreach (var field in fields)
        {
            field.left.transform.localEulerAngles  = new Vector3(0, 0, startingAngle % 180);
            field.right.transform.localEulerAngles = new Vector3(0, 0, -startingAngle % 180);

            field.width = Mathf.Clamp(field.width, 0, 180 - startingAngle);

            field.startingAngle = startingAngle % 180;
            field.endingAngle = field.startingAngle + field.width % 180;

            field.left.fillAmount =
                field.right.fillAmount = field.width / 360;

            startingAngle += field.width;
        }

        var last = fields.Last();

        last.width = 180 - last.startingAngle;
        last.endingAngle = 180;
    }

    public void SetZones(params (Zone zone, float width)[] zones)
    {
        ///////////////////
        ///
        /// TODO: Might need to create/destroy zone images at runtime
        /// TODO: Also add pairs (Wheel.Zone , Color) to GameSettings
        ///
        ///////////////////
        foreach (var (field, (zone, width)) in fields.Zip(zones))
        {
            field.zone = zone;
            field.width = width * 180f;
        }

        UpdateZones();
    }

    (Image, Image) InitZones()
    {

        Image left = CreateZone("Left");
        Image right = CreateZone("Right");

        left.fillClockwise = false;

        return (left, right);

        Image CreateZone(string name)
        {
            var zone = new GameObject(name).AddComponent<Image>();
            zone.color = Color.white;
            zone.type = Image.Type.Filled;
            zone.sprite = fillSprite;
            zone.transform.parent = zonesParent;
            zone.transform.localPosition = Vector3.zero;
            return zone;
        }
    }

    public void RandomizeAngleOffset()
    {
        zoneAngleOffset = UnityEngine.Random.value * 360f;

        UpdateZones();
    }

    public void ToggleRotation(bool toggle)
    {
        isRotating = toggle;
    }

    public Zone GetZoneUnderCursor()
    {
        float cursorAngle = Mathf.Abs(cursor.transform.localEulerAngles.z);
        float offsetCursorAngle = Mathf.Abs(NormalizeAngle(cursorAngle - zoneAngleOffset));

        return
            fields
            .First(f =>
            {
                return
                    f.startingAngle <= offsetCursorAngle &&
                    offsetCursorAngle <= f.endingAngle;
            })
            .zone;
    }

    void Update()
    {
        if (isRotating)
        {
            RotateCursor();
            UpdateZones();
        }
    }

    void RotateCursor()
    {
        cursorAngle += gameSettings.cursorRotationSpeed * Time.deltaTime;
    }

    float NormalizeAngle(float angle)
    {
        if (Mathf.Abs(angle) > 180)
            angle -= Mathf.Sign(angle) * 360;

        return angle;
    }


    public enum Zone
        {
            Miss, Damage, Crit
        }
}
