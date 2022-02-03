﻿[System.Serializable]
public struct LocalisedString
{
    public string key;

    public LocalisedString(string key)
    {
        this.key = key;
    }

    public string value 
    { get => LocalisationSystem.GetLocalisedValue(key); }

    public static implicit operator LocalisedString(string key) 
        => new LocalisedString(key);
}
