using System.Collections.Generic;


namespace Maxisan.LockKeysTaskbarIndicator;

internal class StatusIconTray
{
    private const string MENU_TEXT_SPACE = " ";
    private const string MENU_TEXT_SHOW = "Show";
    private const string MENU_TEXT_HIDE = "Hide";
    private const string MENU_TEXT_STATUS = "status";
    private const string MENU_TEXT_EXIT = "Exit";

    private readonly App _parent;
    private readonly TrayConfig _config;
    private readonly List<StatusIcon> _icons = new();
    
    public StatusIconTray(TrayConfig config, App parent)
    {
        _config = config;
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

            // Add menu item for exiting the application
            statusIconI.AddContextMenuItem(
                MENU_TEXT_EXIT,
                (sender, e) =>
                {
                    _parent.Shutdown();
                }
                );
        }

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
}
