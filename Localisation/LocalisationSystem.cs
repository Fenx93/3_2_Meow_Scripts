using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LocalisationSystem
{
    public enum Language
    {
        English, Russian
    }

    public static Language language = Language.Russian;

    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedRU;

    public static bool isInit;
    public static CSVLoader csvLoader;

    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();
        UpdateDictionaries();

        isInit = true;
    }

    private static void UpdateDictionaries()
    {
        localisedEN = csvLoader.GetDictionaryValues("en");
        localisedRU = csvLoader.GetDictionaryValues("ru");
    }

    public static Dictionary<string,string> GetDictionaryForEditor()
    {
        if (!isInit)
            Init();
        return localisedEN;
    }

    public static string GetLocalisedValue(string key)
    {
        if (!isInit)
            Init();

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.Russian:
                localisedRU.TryGetValue(key, out value);
                break;
        }
        return value;
    }

    private static void Initialise(string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }
    }
#if UNITY_EDITOR
    public static void Add(string key, string value)
    {
        Initialise(value);

        csvLoader.LoadCSV();
        csvLoader.Add(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Replace(string key, string value)
    {
        Initialise(value);

        csvLoader.LoadCSV();
        csvLoader.Edit(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Remove(string key, string value)
    {
        Initialise(value);

        csvLoader.LoadCSV();
        csvLoader.Remove(key);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }
#endif
}
