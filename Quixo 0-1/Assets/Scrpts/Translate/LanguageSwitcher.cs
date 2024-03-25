using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageSwitcher : MonoBehaviour
{
    private void Awake()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>(); 

        dropdown.options = new List<TMP_Dropdown.OptionData>();

        foreach(string language in Data.LANGUAGES)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(language));
        }

        dropdown.onValueChanged.AddListener((int i) =>
        {
            string language = Data.LANGUAGES[i];
            Data.CURRENT_LANGUAGE = language;
            Data.OnLanguageChanged.Invoke();
        });
    }
}
