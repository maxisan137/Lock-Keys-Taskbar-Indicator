using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace Lock_Keys_Taskbar_Indicator
{
    public partial class App : Application
    {
        private const string APP_NAME = "LockKeysTaskBarIndicator";
        private const string APP_SHORTCUT_NAME = "Lock Keys Taskbar Indicator";
        private const string FILE_EXT_EXE = ".exe";
        private const string FILE_EXT_LNK = ".lnk";
        private const string CONFIG_FILE_PATH = "config.json";

        private const string STATUS_ICON_NAME_CAPS = "Caps Lock";
        private const string STATUS_ICON_NAME_NUM = "Num Lock";
        private const string STATUS_ICON_NAME_SCROLL = "Scroll Lock";

        private Configuration config = new();

        private StatusIcon? capsStatusIcon;
        private StatusIcon? numStatusIcon;
        private StatusIcon? scrollStatusIcon;

        private readonly DispatcherTimer timer = new();

        private static Mutex? mutex = null;
        
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

            // Add app shortcut to startup, if not already done so
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolderPath, APP_SHORTCUT_NAME + FILE_EXT_LNK);
            if (!File.Exists(shortcutPath))
            {
                IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
                string exeDir = AppContext.BaseDirectory;
                shortcut.WorkingDirectory = exeDir;
                shortcut.TargetPath = Path.Combine(exeDir, APP_NAME + FILE_EXT_EXE);
                shortcut.Save();
            }

            // Load configuration
            ReadConfig();

            // Setup icons
            capsStatusIcon = new(
                STATUS_ICON_NAME_CAPS,
                config.CapsStatusIcon,
                () => { return Forms.Control.IsKeyLocked(Forms.Keys.CapsLock); },
                this
                );

            numStatusIcon = new(
                STATUS_ICON_NAME_NUM,
                config.NumStatusIcon,
                () => { return Forms.Control.IsKeyLocked(Forms.Keys.NumLock); },
                this
                );

            scrollStatusIcon = new
                (STATUS_ICON_NAME_SCROLL,
                config.ScrollStatusIcon,
                () => { return Forms.Control.IsKeyLocked(Forms.Keys.Scroll); },
                this
                );

            SwitchIcons();

            // Setup timer
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (sender, e) => { SwitchIcons(); };
            timer.Start();

            base.OnStartup(e);
        }

        // Reads configuration from file or creates one if doesn't exist
        private void ReadConfig()
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                try
                {
                    Configuration? savedConfig = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(CONFIG_FILE_PATH));
                    if (savedConfig is not null) config = (Configuration)savedConfig;
                }
                catch { WriteConfig(); }
            }
            else
            {
                WriteConfig();
            }
        }

        // Writes current configuration to file
        public void WriteConfig()
        {
            // Update configuration
            if (capsStatusIcon is not null) config.CapsStatusIcon = capsStatusIcon.GetConfiguration();
            if (numStatusIcon is not null) config.NumStatusIcon = numStatusIcon.GetConfiguration();
            if (scrollStatusIcon is not null) config.ScrollStatusIcon = scrollStatusIcon.GetConfiguration();

            // Write to file
            string configJsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            if (File.Exists(CONFIG_FILE_PATH)) { File.Delete(CONFIG_FILE_PATH); }
            File.WriteAllText(CONFIG_FILE_PATH, configJsonString);
        }

        // Checks lock keys status
        private void SwitchIcons()
        {
            capsStatusIcon?.CheckLockStatus();
            numStatusIcon?.CheckLockStatus();
            scrollStatusIcon?.CheckLockStatus();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StatusIcon.DisposeAllIcons();
            mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
