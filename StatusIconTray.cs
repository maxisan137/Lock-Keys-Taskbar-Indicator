using System.Collections.Generic;


namespace Maxisan.LockKeysTaskbarIndicator;

internal class StatusIconTray
{
    private const string MENU_TEXT_SPACE = " ";
    private const string MENU_TEXT_SHOW = "Show";
    private const string MENU_TEXT_HIDE = "Hide";
    private const string MENU_TEXT_STATUS = "status";

    private const string MENU_TEXT_SWITCH_TO = "Switch to";
    private const string MENU_TEXT_THEME_DARK = "Dark Mode";
    private const string MENU_TEXT_THEME_LIGHT = "Light Mode";

    private const string MENU_TEXT_EXIT = "Exit";

    private readonly App _parent;
    private readonly AppConfig _config;
    private readonly ThemeHandler _themeHandler;
    private readonly List<StatusIcon> _icons = new();
    
    public StatusIconTray(AppConfig config, App parent)
    {
        _config = config;
        _themeHandler = new(_config);
        _parent = parent;
    }

    /// <summary>
    /// Add a tray icon
    /// </summary>
    /// <param name="icon">StatusIcon to be added to the tray</param>
    public void AddIcon(StatusIcon icon)
    {
        // Clear all icon's context menu
        foreach (StatusIcon statusIcon in _icons)
        {
            statusIcon.ClearContextMenuItems();
        }

        // Add icon to the list
        _icons.Add(icon);

        // Re-populate all icons with menu items
        foreach (StatusIcon statusIconI in _icons)
        {
            // Add menu items for showing/hiding all icons
            foreach (StatusIcon statusIconJ in _icons)
            {
                statusIconI.AddContextMenuItem(
                    MenuItemText(statusIconJ),
                    (sender, e) =>
                    {
                        statusIconJ.SwitchVisibilityStatus();
                        ReloadIcons();

                        statusIconJ.UpdateConfiguration();
                        ConfigHandler.SaveConfig(_config);
                    }
                    );
            }

            // Add menu separator
            statusIconI.AddContextMenuSeparator();

            // Add menu item for switching the theme
            statusIconI.AddContextMenuItem(
                MenuItemThemeSwitchText(_themeHandler),
                (sender, a) =>
                {
                    _themeHandler.SetTheme(_themeHandler.GetCurrentTheme() == Theme.Light ? Theme.Dark : Theme.Light);
                    UpdateTheme();
                    ReloadIcons();
                }
                );

            // Add menu item for exiting the application
            statusIconI.AddContextMenuItem(
                MENU_TEXT_EXIT,
                (sender, e) =>
                {
                    _parent.Shutdown();
                }
                );
        }

        // Update icons themes
        icon.UpdateTheme(_themeHandler.GetCurrentTheme());

        // Visualize the icon
        icon.ResetVisibility();

        // Makes sure to disable Hide option in case there is only one icon showed
        UpdateMenuItems();
    }
    
    /// <summary>
    /// Update all tray icons based on their respective lock status
    /// </summary>
    public void UpdateIcons()
    {
        foreach (StatusIcon icon in _icons)
        {
            icon.CheckLockStatus();
        }
    }

    /// <summary>
    /// Dispose of all tray icons
    /// </summary>
    public void DisposeIcons()
    {
        foreach(StatusIcon icon in _icons)
        {
            icon.Dispose();
        }
    }

    /// <summary>
    /// Reloads all icons based on their visibility setting
    /// </summary>
    private void ReloadIcons()
    {
        foreach (StatusIcon icon in _icons)
        {
            icon.SwitchOffVisibility();
        }
        foreach (StatusIcon icon in _icons)
        {
            icon.ResetVisibility();
        }
        UpdateMenuItems();
    }

    /// <summary>
    /// Updates the icon theme to be used according to current theme
    /// </summary>
    private void UpdateTheme()
    {
        foreach (StatusIcon icon in _icons)
        {
            icon.UpdateTheme(_themeHandler.GetCurrentTheme());
        }
    }

    /// <summary>
    /// Update all icons menu items texts to reflect which icon's visibility is on or off
    /// </summary>
    private void UpdateMenuItems()
    {
        // Check if there is only one icon visible, in which case disable ability to hide it
        int disabled = -1;
        for (int i = 0; i < _icons.Count; i++)
        {
            if (_icons[i].IsShown())
            {
                if (disabled == -1)
                {
                    disabled = i;
                }
                else
                {
                    disabled = -1;
                    break;
                }
            }
        }

        // Change menu items for each icon
        int themeSwitchMenuItemIndex = _icons.Count + 1;
        foreach (StatusIcon statusIcon in _icons)
        {
            for (int i = 0; i < _icons.Count; i++)
            {
                statusIcon.SetContextMenuItemText(i, MenuItemText(_icons[i]));
                statusIcon.SetContextMenuItemAvailability(i, true);
            }

            if (disabled != -1)
            {
                statusIcon.SetContextMenuItemAvailability(disabled, false);
            }

            // Update theme switch menu item
            statusIcon.SetContextMenuItemText(themeSwitchMenuItemIndex, MenuItemThemeSwitchText(_themeHandler));
        }
    }

    /// <summary>
    /// Returns the text of menu item related to this status icon in particular
    /// </summary>
    /// <param name="statusIcon">Status icon</param>
    /// <returns>Menu text related to that icon</returns>
    private static string MenuItemText(StatusIcon statusIcon)
    {
        return (statusIcon.IsShown() ? MENU_TEXT_HIDE : MENU_TEXT_SHOW) + MENU_TEXT_SPACE + statusIcon.GetName() + MENU_TEXT_SPACE + MENU_TEXT_STATUS;
    }

    /// <summary>
    /// Returns the text of the menu item responsible for switching themes
    /// </summary>
    /// <param name="themeHandler">Theme handler</param>
    /// <returns>Text of the menu item responsible for switching themes</returns>
    private static string MenuItemThemeSwitchText(ThemeHandler themeHandler)
    {
        string themeName = themeHandler.GetCurrentTheme() == Theme.Light ? MENU_TEXT_THEME_DARK : MENU_TEXT_THEME_LIGHT;
        return MENU_TEXT_SWITCH_TO + MENU_TEXT_SPACE + themeName;
    }
}
