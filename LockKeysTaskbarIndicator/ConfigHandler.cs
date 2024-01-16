using Newtonsoft.Json;
using System;
using System.IO;


namespace Maxisan.LockKeysTaskbarIndicator;

internal static class ConfigHandler
{
    private const string CONFIG_FILE_LOCAL_LOCATION = "Maxisan/LockKeysTaskbarIndicator";
    private const string CONFIG_FILE_NAME = "config.json";

    private const string DEFAULT_ICON_PATH_CAPS_ON_LIGHT = "icons/capslock_on_black.ico";
    private const string DEFAULT_ICON_PATH_CAPS_ON_DARK = "icons/capslock_on_white.ico";

    private const string DEFAULT_ICON_PATH_CAPS_OFF_LIGHT = "icons/capslock_off_black.ico";
    private const string DEFAULT_ICON_PATH_CAPS_OFF_DARK = "icons/capslock_off_white.ico";

    private const string DEFAULT_ICON_PATH_NUM_ON_LIGHT = "icons/numlock_on_black.ico";
    private const string DEFAULT_ICON_PATH_NUM_ON_DARK = "icons/numlock_on_white.ico";

    private const string DEFAULT_ICON_PATH_NUM_OFF_LIGHT = "icons/numlock_off_black.ico";
    private const string DEFAULT_ICON_PATH_NUM_OFF_DARK = "icons/numlock_off_white.ico";

    private const string DEFAULT_ICON_PATH_SCROLL_ON_LIGHT = "icons/scrolllock_on_black.ico";
    private const string DEFAULT_ICON_PATH_SCROLL_ON_DARK = "icons/scrolllock_on_white.ico";

    private const string DEFAULT_ICON_PATH_SCROLL_OFF_LIGHT = "icons/scrolllock_off_black.ico";
    private const string DEFAULT_ICON_PATH_SCROLL_OFF_DARK = "icons/scrolllock_off_white.ico";


    /// <summary>
    /// Reads configuration from file or creates one if it doesn't exist
    /// </summary>
    /// <returns>Icons configuration</returns>
    public static AppConfig GetConfig()
    {
        string configFilePath = GetConfigFilePath();
        if (File.Exists(configFilePath))
        {
            try
            {
                AppConfig? savedConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(configFilePath));
                if (savedConfig is not null)
                {
                    return savedConfig;
                }
            }
            catch { }
        }

        AppConfig config = GetDefaultConfig();
        SaveConfig(config);
        return config;
    }

    /// <summary>
    /// Writes the given configuration to file
    /// </summary>
    /// <param name="config">Configuration to be saved</param>
    public static void SaveConfig(AppConfig config)
    {
        string configJsonString = JsonConvert.SerializeObject(config, Formatting.Indented);

        string configFilePath = GetConfigFilePath();
        string? directoryPath = Path.GetDirectoryName(configFilePath);
        if (directoryPath != null)
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(configFilePath))
        {
            File.Delete(configFilePath);
        }
        
        File.WriteAllText(configFilePath, configJsonString);
    }

    /// <summary>
    /// Get the path to the config file
    /// </summary>
    /// <returns>Path to the config file</returns>
    private static string GetConfigFilePath()
    {
        string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localFolder, CONFIG_FILE_LOCAL_LOCATION, CONFIG_FILE_NAME);
    }

    /// <summary>
    /// Generate a default app config object
    /// </summary>
    /// <returns>Default AppConfig</returns>
    private static AppConfig GetDefaultConfig()
    {
        return new AppConfig()
        {
            CapsStatusIcon = new StatusIconConfig()
            {
                IconPathOn = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_CAPS_ON_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_CAPS_ON_LIGHT
                },
                IconPathOff = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_CAPS_OFF_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_CAPS_OFF_LIGHT
                },
                ShowIcon = true
            },

            NumStatusIcon = new StatusIconConfig()
            {
                IconPathOn = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_NUM_ON_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_NUM_ON_LIGHT
                },
                IconPathOff = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_NUM_OFF_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_NUM_OFF_LIGHT
                },
                ShowIcon = true
            },

            ScrollStatusIcon = new StatusIconConfig()
            {
                IconPathOn = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_SCROLL_ON_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_SCROLL_ON_LIGHT
                },
                IconPathOff = new()
                {
                    IconPathDarkMode = DEFAULT_ICON_PATH_SCROLL_OFF_DARK,
                    IconPathLightMode = DEFAULT_ICON_PATH_SCROLL_OFF_LIGHT
                },
                ShowIcon = true
            },

            DarkTheme = ThemeHandler.GetWindowsTheme() == Theme.Dark
        };
    }
}
