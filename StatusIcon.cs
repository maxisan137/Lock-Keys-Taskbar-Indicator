using System;
using Forms = System.Windows.Forms;


namespace Maxisan.LockKeysTaskbarIndicator;

internal class StatusIcon
{
    private readonly string _name;
    
    /// <summary>
    /// Icon configuration reference
    /// </summary>
    private readonly StatusIconConfig _config;

    private readonly System.Drawing.Icon _iconOn;
    private readonly System.Drawing.Icon _iconOff;

    private readonly Forms.NotifyIcon notifyIcon;

    /// <summary>
    /// Method used to check the key lock status
    /// </summary>
    private readonly Func<bool> _lockCheck;

    /// <summary>
    /// Status of requried icon visibility.
    /// False means that the icon should not be displayed in the tray.
    /// </summary>
    private bool _show;

    public StatusIcon(string name, StatusIconConfig config, Func<bool> lockCheckMethod)
    {
        _name = name;

        _config = config;

        _iconOn = new(_config.IconPathOn);
        _iconOff = new(_config.IconPathOff);
        _show = _config.ShowIcon;

        _lockCheck = lockCheckMethod;

        notifyIcon = new() { ContextMenuStrip = new Forms.ContextMenuStrip() };
    }

    /// <summary>
    /// Get the name of this status icon
    /// </summary>
    /// <returns>Status icon name</returns>
    public string GetName()
    {
        return _name;
    }

    /// <summary>
    /// Returns the status of this icon being shown in the tray
    /// </summary>
    /// <returns>Status of this icon being shown in the tray</returns>
    public bool IsShown()
    {
        return _show;
    }

    /// <summary>
    /// Switch the status of this icon being shown in the tray.
    /// Note: this does not make the icon invisible, but simply changes the internal parameter of whether or not it should be invisible
    /// </summary>
    public void SwitchVisibilityStatus()
    {
        _show = !_show;
        ResetVisibility();
    }

    /// <summary>
    /// Switches off the visibility of icon, regardless of it's set status
    /// </summary>
    public void SwitchOffVisibility()
    {
        notifyIcon.Visible = false;
    }

    /// <summary>
    /// Reset the visibility of tray icon. Is icon is set to be visible, the icon becomes visible in the tray
    /// </summary>
    public void ResetVisibility()
    {
        notifyIcon.Visible = _show;
    }

    /// <summary>
    /// Dispose of the tray icon
    /// </summary>
    public void Dispose()
    {
        notifyIcon.Dispose();
    }

    /// <summary>
    /// Add a menu item to the icon's context menu
    /// </summary>
    /// <param name="text">Menu item text</param>
    /// <param name="onClick">Menu item on-click event handler</param>
    public void AddContextMenuItem(string text, EventHandler onClick)
    {
        notifyIcon.ContextMenuStrip?.Items.Add(text, null, onClick);
    }

    /// <summary>
    /// Clear all items from the icon's context menu
    /// </summary>
    public void ClearContextMenuItems()
    {
        notifyIcon.ContextMenuStrip?.Items.Clear();
    }

    /// <summary>
    /// Set the text of an item in the icon's context menu
    /// </summary>
    /// <param name="index">Menu item index</param>
    /// <param name="text">New menu item text</param>
    public void SetContextMenuItemText(int index, string text)
    {
        var menuItem = GetContextMenuItem(index);
        if (menuItem != null)
        {
            menuItem.Text = text;
        }
    }

    /// <summary>
    /// Set the availability of an item in the iocn's context menu
    /// </summary>
    /// <param name="index">Menu item index</param>
    /// <param name="enabled">Whether or not the menu item should be enabled</param>
    public void SetContextMenuItemAvailability(int index, bool enabled)
    {
        var menuItem = GetContextMenuItem(index);
        if (menuItem != null)
        {
            menuItem.Enabled = enabled;
        }
    }

    /// <summary>
    /// Update the icon's own configuration object
    /// </summary>
    public void UpdateConfiguration()
    {
        _config.ShowIcon = _show;
    }

    /// <summary>
    /// Checks the key lock status and changes icon accordingly
    /// </summary>
    public void CheckLockStatus()
    {
        notifyIcon.Icon = _lockCheck() ? _iconOn : _iconOff;
    }

    /// <summary>
    /// Returns a menu item of the tray icon's context menu
    /// </summary>
    /// <param name="index">Menu item index</param>
    /// <returns>Context menu's item</returns>
    private Forms.ToolStripItem? GetContextMenuItem(int index)
    {
        var cMenuStrip = notifyIcon.ContextMenuStrip;
        if (cMenuStrip is null)
        {
            return null;
        }
        return cMenuStrip.Items[index];
    }
}
