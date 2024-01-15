using Newtonsoft.Json;
using System.IO;


namespace Maxisan.LockKeysTaskbarIndicator;

internal static class ConfigHandler
{
    private const string CONFIG_FILE_PATH = "config.json";

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
        if (File.Exists(CONFIG_FILE_PATH))
        {
            try
            {
                AppConfig? savedConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(CONFIG_FILE_PATH));
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


        if (File.Exists(CONFIG_FILE_PATH))
        {
            File.Delete(CONFIG_FILE_PATH);
        }
        
        File.WriteAllText(CONFIG_FILE_PATH, configJsonString);
    }

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
