namespace CSharpUtilities.Geometry
{
    /// <summary>
    /// Defines an angle pointing in one of four axis aligned directions.
    /// </summary>
    public enum OrthoAngle : byte
    {
        Zero, Ninety, OneEighty, TwoSeventy
    }
    /// <summary>
    /// Provides extension methods for converting to and from OrthoAngle.
    /// </summary>
    public static class OrthoAngleExtensions
    {
        #region Conversions
        /// <summary>
        /// Converts a floating point degrees value to the nearest ortho angle.
        /// </summary>
        /// <param name="value">The value in degrees.</param>
        /// <returns>The nearest ortho angle.</returns>
        public static OrthoAngle ToOrthoAngle(this float value)
        {
            // Wrap angle into range.
            value %= 360f;
            if (value < 0f)
                value += 360f;
            // Find the closest angle to return.
            if (value > 315f)
                return OrthoAngle.Zero;
            else if (value > 225f)
                return OrthoAngle.TwoSeventy;
            else if (value > 135f)
                return OrthoAngle.OneEighty;
            else if (value > 45f)
                return OrthoAngle.Ninety;
            else
                return OrthoAngle.Zero;
        }
        /// <summary>
        /// Converts an ortho angle to its corresponding value in degrees.
        /// </summary>
        /// <param name="orthoAngle">The angle to convert.</param>
        /// <returns>The angle in degrees.</returns>
        public static float ToDegrees(this OrthoAngle orthoAngle)
        {
            switch (orthoAngle)
            {
                case OrthoAngle.Zero: return 0f;
                case OrthoAngle.Ninety: return 90f;
                case OrthoAngle.OneEighty: return 180f;
                case OrthoAngle.TwoSeventy: return 270f;
                default: return default;
            }
        }
        #endregion
        #region Value Cycling
        /// <summary>
        /// Advances the ortho angle 90 degrees to the next axis aligned angle.
        /// </summary>
        public static void Advance(this ref OrthoAngle orthoAngle)
        {
            switch (orthoAngle)
            {
                case OrthoAngle.Zero: orthoAngle = OrthoAngle.Ninety; break;
                case OrthoAngle.Ninety: orthoAngle = OrthoAngle.OneEighty; break;
                case OrthoAngle.OneEighty: orthoAngle = OrthoAngle.TwoSeventy; break;
                case OrthoAngle.TwoSeventy: orthoAngle = OrthoAngle.Zero; break;
            }
        }
        #endregion
    }
}
