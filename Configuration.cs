namespace Maxisan.LockKeysTaskbarIndicator
{
    internal struct Configuration
    {
        // Default icons files path
        private const string DEFAULT_ICON_PATH_CAPS_ON = "icons/capslock_on.ico";
        private const string DEFAULT_ICON_PATH_CAPS_OFF = "icons/capslock_off.ico";
        private const string DEFAULT_ICON_PATH_NUM_ON = "icons/numlock_on.ico";
        private const string DEFAULT_ICON_PATH_NUM_OFF = "icons/numlock_off.ico";
        private const string DEFAULT_ICON_PATH_SCROLL_ON = "icons/scrolllock_on.ico";
        private const string DEFAULT_ICON_PATH_SCROLL_OFF = "icons/scrolllock_off.ico";

        public StatusIconConfig CapsStatusIcon { get; set; }
        public StatusIconConfig NumStatusIcon { get; set; }
        public StatusIconConfig ScrollStatusIcon { get; set; }

        public Configuration()
        {
            CapsStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_CAPS_ON,
                IconPathOff = DEFAULT_ICON_PATH_CAPS_OFF,
                ShowIcon = true
            };

            NumStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_NUM_ON,
                IconPathOff = DEFAULT_ICON_PATH_NUM_OFF,
                ShowIcon = true
            };

            ScrollStatusIcon = new StatusIconConfig()
            {
                IconPathOn = DEFAULT_ICON_PATH_SCROLL_ON,
                IconPathOff = DEFAULT_ICON_PATH_SCROLL_OFF,
                ShowIcon = true
            };
        }
    }
}
