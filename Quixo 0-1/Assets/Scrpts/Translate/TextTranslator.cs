using TMPro;
using UnityEngine;

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
