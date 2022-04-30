using UnityEngine;

public class SettingsStorage : MonoBehaviour
{
    public static SettingsStorage Instance;
    public SaveSettings Settings { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
