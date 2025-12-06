using LightNap.Core.Profile.Dto.Response;

namespace LightNap.Core.Profile.Helpers
{
    /// <summary>
    /// Provides methods to migrate browser settings to newer versions.
    /// </summary>
    public static class BrowserSettingsMigration
    {
        /// <summary>
        /// Migrates the given browser settings to the latest version.
        /// </summary>
        /// <param name="settings">The browser settings to migrate.</param>
        /// <returns>The migrated browser settings.</returns>
        public static BrowserSettingsDto Migrate(BrowserSettingsDto settings)
        {
            // This pattern should cover most cases for migrating settings forward with basic adds and deletes.
            // However, it isn't great for more complex migrations, so consider a more robust solution if needed.

            //if (settings.Version == 1)
            //{
            //    settings = BrowserSettingsMigration.MigrateFromV1ToV2(settings);
            //}

            //if (settings.Version == 2)
            //{
            //    settings = BrowserSettingsMigration.MigrateFromV2ToV3(settings);
            //}

            return settings;
        }

        // /// <summary>
        // /// Migrates browser settings from version 1 to version 2.
        // /// </summary>
        // /// <param name="settings">The browser settings to migrate.</param>
        // /// <returns>The migrated browser settings.</returns>
        // private static BrowserSettingsDto MigrateFromV1ToV2(BrowserSettingsDto settings)
        // {
        //     settings.Version = 2;
        //     // Example: Add new fields or transform existing ones
        //     // settings.ClientSpecificSettings["NewField"] = "defaultValue";
        //     return settings;
        // }

        // /// <summary>
        // /// Migrates browser settings from version 2 to version 3.
        // /// </summary>
        // /// <param name="settings">The browser settings to migrate.</param>
        // /// <returns>The migrated browser settings.</returns>
        // private static BrowserSettingsDto MigrateFromV2ToV3(BrowserSettingsDto settings)
        // {
        //     settings.Version = 3;
        //     // Example: Add new fields or transform existing ones
        //     // settings.MajorSetting = "defaultValue";
        //     return settings;
        // }
    }
}
