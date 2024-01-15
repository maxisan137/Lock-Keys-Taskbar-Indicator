using System.Text.Json;
using System.IO;

namespace Maxisan.LockKeysTaskbarIndicator;

internal class ConfigHandler
{
    private const string CONFIG_FILE_PATH = "config.json";

    /// <summary>
    /// Reads configuration from file or creates one if it doesn't exist
    /// </summary>
    /// <returns>Icons configuration</returns>
    public TrayConfig GetConfig()
    {
        if (File.Exists(CONFIG_FILE_PATH))
        {
            try
            {
                TrayConfig? savedConfig = JsonSerializer.Deserialize<TrayConfig>(File.ReadAllText(CONFIG_FILE_PATH));
                if (savedConfig is not null)
                {
                    return savedConfig;
                }
            }
            catch { }
        }

        TrayConfig config = new();
        SaveConfig(config);
        return config;
    }

    /// <summary>
    /// Writes the given configuration to file
    /// </summary>
    /// <param name="config">Configuration to be saved</param>
    public void SaveConfig(TrayConfig config)
    {
        string configJsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        
        if (File.Exists(CONFIG_FILE_PATH))
        {
            File.Delete(CONFIG_FILE_PATH);
        }
        
        File.WriteAllText(CONFIG_FILE_PATH, configJsonString);
    }
}
