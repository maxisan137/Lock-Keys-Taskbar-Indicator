using System.Text.Json;
using System.IO;

namespace Maxisan.LockKeysTaskbarIndicator;

internal static class ConfigHandler
{
    private const string CONFIG_FILE_PATH = "config.json";

    private const string DEFAULT_ICON_PATH_CAPS_ON = "icons/capslock_on.ico";
    private const string DEFAULT_ICON_PATH_CAPS_OFF = "icons/capslock_off.ico";
    private const string DEFAULT_ICON_PATH_NUM_ON = "icons/numlock_on.ico";
    private const string DEFAULT_ICON_PATH_NUM_OFF = "icons/numlock_off.ico";
    private const string DEFAULT_ICON_PATH_SCROLL_ON = "icons/scrolllock_on.ico";
    private const string DEFAULT_ICON_PATH_SCROLL_OFF = "icons/scrolllock_off.ico";

    /// <summary>
    /// Reads configuration from file or creates one if it doesn't exist
    /// </summary>
    /// <returns>Icons configuration</returns>
    public static TrayConfig GetConfig()
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

        TrayConfig config = GetDefaultConfig();
        SaveConfig(config);
        return config;
    }

    /// <summary>
    /// Writes the given configuration to file
    /// </summary>
    /// <param name="config">Configuration to be saved</param>
    public static void SaveConfig(TrayConfig config)
    {
        string configJsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        
        if (File.Exists(CONFIG_FILE_PATH))
        {
            File.Delete(CONFIG_FILE_PATH);
        }
        
        File.WriteAllText(CONFIG_FILE_PATH, configJsonString);
    }

    private static TrayConfig GetDefaultConfig()
    {
        return new TrayConfig()
        {
            CapsStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_CAPS_ON,
                IconPathOff = DEFAULT_ICON_PATH_CAPS_OFF,
                ShowIcon = true
            },

            NumStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_NUM_ON,
                IconPathOff = DEFAULT_ICON_PATH_NUM_OFF,
                ShowIcon = true
            },

            ScrollStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_SCROLL_ON,
                IconPathOff = DEFAULT_ICON_PATH_SCROLL_OFF,
                ShowIcon = true
            }
        };
    }
}
