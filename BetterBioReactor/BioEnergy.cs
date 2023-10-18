namespace BetterBioReactor;

using UnityEngine;
using UnityEngine.UI;

public class BioEnergy: MonoBehaviour
{
    public static Font Arial { get; private set; }
    public bool FullyConsumed => RemainingEnergy <= 0f;
    public string EnergyString => $"{Mathf.RoundToInt(RemainingEnergy)}/{MaxEnergy}";
    public int Size => Pickupable.inventoryItem.width * Pickupable.inventoryItem.height;
    public float MaxEnergy => BaseBioReactor.GetCharge(Pickupable.GetTechType());

    public Text DisplayText { get; internal set; }

    public Pickupable Pickupable { get; internal set; }

    public float RemainingEnergy;

    public void UpdateInventoryText()
    {
        if (this.DisplayText is null)
            return;

        this.DisplayText.text = this.EnergyString;
    }

    public void AddDisplayText(uGUI_ItemIcon icon)
    {
        Arial ??= (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        var textGO = new GameObject("EnergyLabel");
        textGO.transform.SetParent(icon.transform);
        textGO.transform.localEulerAngles = Vector3.zero;
        textGO.transform.localRotation = Quaternion.identity;

        Text text = textGO.AddComponent<Text>();
        text.font = Arial;
        text.material = Arial.material;
        text.text = this.EnergyString;
        text.fontSize = 14 + Size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.yellow;

        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = Color.black;

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;

        this.DisplayText = text;
    }
}