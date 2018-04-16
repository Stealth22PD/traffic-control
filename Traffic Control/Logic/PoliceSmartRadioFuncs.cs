using System;

namespace Stealth.Plugins.TrafficControl.Logic
{
    internal static class PoliceSmartRadioFuncs
    {
        /// <summary>
        /// Adds an Action and an availability check to the specified button. Only buttons contained in a folder matching your plugin's name can be manipulated.
        /// </summary>
        /// <param name="action">The action to execute if the button is selected.</param>
        /// <param name="buttonName">The texture file name of the button, excluding any directories or file extensions.</param>
        /// <returns>Returns whether the button was successfully added or not. If false, a reason is logged to the console.</returns>
        public static bool AddActionToButton(Action action, string buttonName)
        {
            return PoliceSmartRadio.API.Functions.AddActionToButton(action, buttonName);
        }

        /// <summary>
        /// Adds an Action and an availability check to the specified button. Only buttons contained in a folder matching your plugin's name can be manipulated.
        /// </summary>
        /// <param name="action">The action to execute if the button is selected.</param>
        /// <param name="isAvailable">Function returning a bool indicating whether the button is currently available (if false, button is hidden). This is often called, so try making this light-weight (e.g. simply return the value of a boolean property). Make sure to do proper checking in your Action too, as the user can forcefully display all buttons via a setting in their config file.</param>
        /// <param name="buttonName">The texture file name of the button, excluding any directories or file extensions.</param>
        /// <returns>Returns whether the button was successfully added or not. If false, a reason is logged to the console.</returns>
        public static bool AddActionToButton(Action action, Func<bool> isAvailable, string buttonName)
        {
            return PoliceSmartRadio.API.Functions.AddActionToButton(action, isAvailable, buttonName);
        }

        /// <summary>
        /// Raised whenever the player selects a button on the SmartRadio.
        /// </summary>
        /// <param name="handler"></param>
        public static void AddButtonSelectedHandler(Action handler)
        {
            PoliceSmartRadio.API.Functions.ButtonSelected += handler;
        }
    }
}