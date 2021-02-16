using System;
using UnityEngine;

namespace UnityUtilities.InspectorFields
{
    /// <summary>
    /// Exposed RangeInt to an inspector field.
    /// </summary>
    [Serializable]
    public sealed class RangeInt_InspectorField
    {
        #region Inspector Fields
        [Tooltip("The left minimum end of the range.")]
        [SerializeField] private int min = 0;
        [Tooltip("The right maximum end of the range.")]
        [SerializeField] private int max = 1;
        #endregion
        #region On Validate
        /// <summary>
        /// Call this in OnValidate to ensure valid values.
        /// </summary>
        public void OnValidate()
        {
            if (max < min)
                max = min;
        }
        #endregion
        #region Value Accessor
        /// <summary>
        /// Retrieves the RangeInt value from inspector parameters.
        /// </summary>
        public RangeInt Value => new RangeInt(min, max - min);
        #endregion
    }
}
