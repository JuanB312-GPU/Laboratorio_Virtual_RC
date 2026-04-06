using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }
    public AppConfig Config { get; private set; }

    void Awake()
    {
        // Singleton
        Debug.Log("ConfigManager Awake");
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadConfig();
    }

    void LoadConfig()
    {
        string path = Path.Combine(Application.persistentDataPath, "config.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Config = JsonUtility.FromJson<AppConfig>(json);
            Debug.Log($"Config cargada: {Config.ip}:{Config.puerto1}");
        }
        else
        {
            Debug.LogWarning("config.json no encontrado, usando valores por defecto.");
            Config = new AppConfig
            {
                ip = "127.0.0.1",
                puerto1 = 8080,
                puerto2 = 8081,
                puerto3 = 8082
            };
        }
    }
}
