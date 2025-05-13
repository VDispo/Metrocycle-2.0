using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Collections.Generic;

public class LocaleDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private bool initialized = false;

    void Start()
    {
        // Load saved locale from PlayerPrefs
        string savedLocaleCode = PlayerPrefs.GetString("selectedLocale", null);
        if (!string.IsNullOrEmpty(savedLocaleCode))
        {
            var savedLocale = LocalizationSettings.AvailableLocales.GetLocale(savedLocaleCode);
            if (savedLocale != null)
            {
                LocalizationSettings.SelectedLocale = savedLocale;
            }
        }

        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        languageDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            // Use the native name of the language (e.g. "Français" instead of "French")
            options.Add(new TMP_Dropdown.OptionData(locale.Identifier.CultureInfo.NativeName));
        }

        languageDropdown.AddOptions(options);

        // Set current selected language
        int currentIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        languageDropdown.value = currentIndex;
        languageDropdown.RefreshShownValue();

        languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        initialized = true;
    }

    void OnDropdownValueChanged(int index)
    {
        if (!initialized) return;

        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        // Save selected language to PlayerPrefs
        PlayerPrefs.SetString("selectedLocale", selectedLocale.Identifier.Code);
        PlayerPrefs.Save();
    }
}