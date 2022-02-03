using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocaliserUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    public LocalisedString localisedString;

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.text = localisedString.value;
    }
}
