using Microsoft.Win32;


namespace Maxisan.LockKeysTaskbarIndicator;

internal class ThemeHandler
{
    private const string REGISTRY_KEY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string REGISTRY_VALUE_LIGHT_THEME = "AppsUseLightTheme";

    private readonly AppConfig _config;

    public static readonly Theme DEFAULT_THEME = Theme.Dark;

    public ThemeHandler(AppConfig config)
    {
        _config = config;
    }

    public Theme GetCurrentTheme()
    {
        return _config.DarkTheme ? Theme.Dark : Theme.Light;
    }

    public void SetTheme(Theme newTheme)
    {
        _config.DarkTheme = newTheme == Theme.Dark;
        ConfigHandler.SaveConfig(_config);
    }

    /// <summary>
    /// Get the theme currently set in Windows registry
    /// </summary>
    /// <returns>Return the theme currently set in Windows registry</returns>
    public static Theme GetWindowsTheme()
    {
        try
        {
            var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_PATH);

            object registryValueObject = key?.GetValue(REGISTRY_VALUE_LIGHT_THEME);
            if (registryValueObject is null)
            {
                return Theme.Light; // Default to light theme if the value is not found
            }

            int registryValue = (int)registryValueObject;
            return registryValue == 1 ? Theme.Light : Theme.Dark; // 0 means dark theme, 1 means light theme
        }
        catch
        {
            return DEFAULT_THEME;
        }
    }
}
