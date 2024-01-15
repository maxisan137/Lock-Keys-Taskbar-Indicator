using System.Collections.Generic;
using Forms = System.Windows.Forms;


namespace Maxisan.LockKeysTaskbarIndicator
{
    internal class StatusIcon
    {
        private const string MENU_TEXT_SPACE = " ";
        private const string MENU_TEXT_SHOW = "Show";
        private const string MENU_TEXT_HIDE = "Hide";
        private const string MENU_TEXT_STATUS = "status";
        private const string MENU_TEXT_EXIT = "Exit";

        private readonly string name;

        private readonly string iconPathOn;
        private readonly string iconPathOff;
        private readonly System.Drawing.Icon iconOn;
        private readonly System.Drawing.Icon iconOff;

        private bool show;

        private readonly Forms.NotifyIcon notifyIcon;

        // A method used to check the key lock status
        public delegate bool LockCheck();
        private readonly LockCheck lockCheck;

        private static readonly List<StatusIcon> statusIcons = new();

        public StatusIcon(string name, StatusIconConfig config, LockCheck lockCheckMethod, App parent)
        {
            this.name = name;
            iconPathOn = config.IconPathOn;
            iconOn = new(iconPathOn);
            iconPathOff = config.IconPathOff;
            iconOff = new(iconPathOff);
            show = config.ShowIcon;
            lockCheck = lockCheckMethod;

            notifyIcon = new() { ContextMenuStrip = new Forms.ContextMenuStrip() };

            // Clear menus of all icons
            foreach (StatusIcon statusIcon in statusIcons)
            {
                statusIcon.notifyIcon.ContextMenuStrip?.Items.Clear();
            }

            // Add icon to static list
            statusIcons.Add(this);

            // Re-populate all icons with menu items
            foreach (StatusIcon statusIconI in statusIcons)
            {
                // Add menu items for showing/hiding all icons
                foreach (StatusIcon statusIconJ in statusIcons)
                {
                    statusIconI.notifyIcon.ContextMenuStrip?.Items.Add(
                        statusIconJ.MenuItemText(),
                        null,
                        (sender, e) =>
                        {
                            statusIconJ.SwitchVisibility();
                            parent.WriteConfig();
                        }
                        );
                }

                // Add menu item for exiting the application
                statusIconI.notifyIcon.ContextMenuStrip?.Items.Add(
                    MENU_TEXT_EXIT,
                    null,
                    (sender, e) =>
                    {
                        parent.Shutdown();
                    }
                    );
            }

            // Visualize the icon
            if ( show ) { notifyIcon.Visible = true; }

            // Makes sure to disable Hide option in case there is only one icon showed
            UpdateMenuItems();
        }

        // Returns configuration of the status icon
        public StatusIconConfig GetConfiguration()
        {
            return new StatusIconConfig()
            {
                IconPathOn = iconPathOn,
                IconPathOff = iconPathOff,
                ShowIcon = show
            };
        }

        // Checks the key lock status and changes icon accordingly
        public void CheckLockStatus()
        {
            notifyIcon.Icon = lockCheck() ? iconOn : iconOff;
        }

        // Show or Hide status icon
        public void SwitchVisibility()
        {
            show = !show;
            foreach(StatusIcon statusIcon in statusIcons)
            {
                statusIcon.notifyIcon.Visible = false;
            }
            foreach(StatusIcon statusIcon in statusIcons)
            {
                statusIcon.notifyIcon.Visible = statusIcon.show;
            }
            UpdateMenuItems();
        }

        // Returns the text of menu item related to this status icon in particular
        private string MenuItemText()
        {
            return (show ? MENU_TEXT_HIDE : MENU_TEXT_SHOW) + MENU_TEXT_SPACE + name + MENU_TEXT_SPACE + MENU_TEXT_STATUS;
        }

        // Update the text of menu items based on whether or not the respective icons are showing
        private static void UpdateMenuItems()
        {
            // Check if thre is only one icon visible, in which case disable ability to hide it
            int disabled = -1;
            for (int i = 0; i < statusIcons.Count; i++)
            {
                if (statusIcons[i].show)
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

            // Change menu items
            foreach (StatusIcon statusIcon in statusIcons)
            {
                var cMenuStrip = statusIcon.notifyIcon.ContextMenuStrip;
                if (cMenuStrip is null) continue;

                for (int i = 0; i < statusIcons.Count; i++)
                {
                    cMenuStrip.Items[i].Text = statusIcons[i].MenuItemText();
                    cMenuStrip.Items[i].Enabled = true;
                }

                if (disabled != -1)
                {
                    cMenuStrip.Items[disabled].Enabled = false;
                }
            }
        }

        // Disposes of all tray icons
        public static void DisposeAllIcons()
        {
            foreach(StatusIcon statusIcon in statusIcons)
            {
                statusIcon.notifyIcon.Dispose();
            }
        }
    }
}
