using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(TMP_Text))]
public class TextTranslator : MonoBehaviour
{
    TMP_Text _text;
    string _key;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _key = _text.text;
        _SetText();
        Data.OnLanguageChanged.AddListener(_SetText);
    }

    private void _SetText()
    {
        _text.text = Data.LOCALIZATION[_key][Data.CURRENT_LANGUAGE];
    }
}
