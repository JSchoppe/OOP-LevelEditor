using UnityEngine.UI;

namespace UnityUtilities.UI
{
    /// <summary>
    /// Provides extensions for the Selectable class.
    /// </summary>
    public static class SelectableExtensions
    {
        #region Deselect (Work-around)
        /// <summary>
        /// Removes selection focus from an object.
        /// </summary>
        /// <param name="selectable">The object remove focus from.</param>
        public static void Deselect(this Selectable selectable)
        {
            // This is workaround for the fact that Unity does not
            // expose a method to deselect these objects. Making
            // an object non-interactable removes the selection focus.
            selectable.interactable = false;
            selectable.interactable = true;
        }
        #endregion
    }
}
