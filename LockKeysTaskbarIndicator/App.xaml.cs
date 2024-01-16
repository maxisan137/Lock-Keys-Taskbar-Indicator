using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Forms = System.Windows.Forms;


namespace Maxisan.LockKeysTaskbarIndicator;

public partial class App : Application
{
    private const string APP_NAME = "LockKeysTaskBarIndicator";

    private const string STATUS_ICON_NAME_CAPS = "Caps Lock";
    private const string STATUS_ICON_NAME_NUM = "Num Lock";
    private const string STATUS_ICON_NAME_SCROLL = "Scroll Lock";

    private const int REFERSH_TIME_MS = 100;

    private static Mutex? mutex = null;

    private StatusIconTray _iconTray;


    protected override void OnStartup(StartupEventArgs e)
    {
        // Ensure that only one instance of the app is running
        bool createdNew;
        mutex = new(true, APP_NAME, out createdNew);
        if (!createdNew)
        {
            MessageBox.Show("An instance of the application is already running!");
            Current.Shutdown();
        }

        // Get configuration
        AppConfig config = ConfigHandler.GetConfig();

        // Create icon tray
        _iconTray = new(config, this);

        // Add icons
        _iconTray.AddIcon(new StatusIcon(
            STATUS_ICON_NAME_CAPS,
            config.CapsStatusIcon,
            () => { return Forms.Control.IsKeyLocked(Forms.Keys.CapsLock); }
            ));

        _iconTray.AddIcon( new StatusIcon(
            STATUS_ICON_NAME_NUM,
            config.NumStatusIcon,
            () => { return Forms.Control.IsKeyLocked(Forms.Keys.NumLock); }
            ));

        _iconTray.AddIcon(new StatusIcon(
            STATUS_ICON_NAME_SCROLL,
            config.ScrollStatusIcon,
            () => { return Forms.Control.IsKeyLocked(Forms.Keys.Scroll); }
            ));

        // Update icons
        _iconTray.UpdateIcons();

        // Setup timer
        DispatcherTimer timer = new();
        timer.Interval = TimeSpan.FromMilliseconds(REFERSH_TIME_MS);
        timer.Tick += (sender, e) => { _iconTray.UpdateIcons(); };
        timer.Start();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _iconTray.DisposeIcons();
        mutex?.Dispose();
        base.OnExit(e);
    }
}
