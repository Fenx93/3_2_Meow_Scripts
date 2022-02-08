using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocaliserUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    public LocalisedString localisedString;

    private void Start()
    {
        SetLanguageValue();
    }

    public void SetLanguageValue()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.text = localisedString.Value;
    }
}
