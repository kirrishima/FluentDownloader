using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;

namespace FluentDownloader.Helpers
{
    public static class LocalizedStrings
    {
        /// <summary>
        /// Manages access to application resources and localized strings
        /// </summary>
        private static readonly ResourceManager _resourceManager = new();

        private static ResourceMap? _mainResourceMap;
        private static ResourceMap? _leftSideBarResourceMap;
        private static ResourceMap? _messagesResourceMap;
        private static ResourceMap? _settingsResourceMap;
        private static ResourceMap? _speedUnitsResourceMap;

        /// <summary>
        /// Gets the main resource map for application strings (Resources subtree)
        /// </summary>
        /// <remarks>
        /// Initialized lazily on first access using the "Resources" subtree
        /// </remarks>
        private static ResourceMap MainResourceMap =>
            _mainResourceMap ??= _resourceManager.MainResourceMap.GetSubtree("Resources");

        /// <summary>
        /// Gets the resource map for left sidebar UI elements (LeftSideBar subtree)
        /// </summary>
        /// <remarks>
        /// Initialized lazily on first access using the "LeftSideBar" subtree
        /// </remarks>
        private static ResourceMap LeftSideBarResourceMap =>
            _leftSideBarResourceMap ??= _resourceManager.MainResourceMap.GetSubtree("LeftSideBar");

        /// <summary>
        /// Gets the resource map for application messages (Messages subtree)
        /// </summary>
        /// <remarks>
        /// Initialized lazily on first access using the "Messages" subtree
        /// </remarks>
        private static ResourceMap MessagesResourceMap =>
            _messagesResourceMap ??= _resourceManager.MainResourceMap.GetSubtree("Messages");

        private static ResourceMap SettingsResourceMap =>
            _settingsResourceMap ??= _resourceManager.MainResourceMap.GetSubtree("Settings");

        private static ResourceMap SpeedUnitsResourceMap =>
            _speedUnitsResourceMap ??= _resourceManager.MainResourceMap.GetSubtree("SpeedUnit");

        /// <summary>
        /// Retrieves a localized string from the specified resource map
        /// </summary>
        /// <param name="resourceKey">The key identifying the resource to retrieve</param>
        /// <param name="resourceMap">The resource map to search for the key</param>
        /// <returns>
        /// The localized string if found, otherwise an empty string.
        /// Returns empty string if any error occurs during retrieval.
        /// </returns>
        private static string GetResourceString(string resourceKey, ResourceMap resourceMap)
        {
            try
            {
                return resourceMap?.GetValue(resourceKey).ValueAsString ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves a localized string from the main application resources
        /// </summary>
        /// <param name="resourceKey">The key identifying the resource to retrieve</param>
        /// <returns>
        /// Localized string from Resources map if found, otherwise empty string
        /// </returns>
        public static string GetResourceString(string resourceKey) =>
            GetResourceString(resourceKey, MainResourceMap);

        /// <summary>
        /// Retrieves a localized string for theme-related resources
        /// </summary>
        /// <param name="resourceKey">The key identifying the theme resource</param>
        /// <returns>
        /// Localized string from LeftSideBar map if found, otherwise empty string
        /// </returns>
        public static string GetLeftSidebarString(string resourceKey) =>
            GetResourceString(resourceKey, LeftSideBarResourceMap);

        /// <summary>
        /// Retrieves a localized message string
        /// </summary>
        /// <param name="resourceKey">The key identifying the message resource</param>
        /// <returns>
        /// Localized string from Messages map if found, otherwise empty string
        /// </returns>
        public static string GetMessagesString(string resourceKey) =>
            GetResourceString(resourceKey, MessagesResourceMap);

        public static string GetSettingsString(string resourceKey) =>
            GetResourceString(resourceKey, SettingsResourceMap);

        public static string GetSpeedUnitsString(string resourceKey) =>
            GetResourceString(resourceKey, SpeedUnitsResourceMap);
    }
}
