using System;
using UnityEngine;

namespace StageEditor
{
    /// <summary>
    /// Holds the current state for the stage editor.
    /// </summary>
    public sealed class StageEditorState
    {
        #region Private State Fields
        private readonly bool[,] tilesFilled;
        private readonly RangeInt xRange, zRange;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new state container for a stage editor.
        /// </summary>
        /// <param name="xRange">The range of x values that create the stage.</param>
        /// <param name="zRange">The range of z values that create the stage.</param>
        public StageEditorState(RangeInt xRange, RangeInt zRange)
        {
            // Initialize the state for the field of fillable tiles.
            this.xRange = xRange;
            this.zRange = zRange;
            tilesFilled = new bool[
                xRange.length + 1,
                zRange.length + 1];
        }
        #endregion
        #region Functions Wrapping Tile Fill State
        private bool IsTileFilled(int x, int z)
            => tilesFilled[x - xRange.start, z - zRange.start];
        private void SetTile(int x, int z, bool value)
            => tilesFilled[x - xRange.start, z - zRange.start] = value;
        #endregion
        #region Argument Validation Function
        private void ValidateRegionArgs(int x, int z, int lengthX, int lengthZ)
        {
            if (x < xRange.start || x > xRange.end)
                throw new ArgumentOutOfRangeException(
                    "x", "Parameter `x` was outside of the stage size.");
            if (z < zRange.start || z > zRange.end)
                throw new ArgumentOutOfRangeException(
                    "z", "Parameter `z` was outside of the stage size.");
            if (x + lengthX > xRange.end)
                throw new ArgumentOutOfRangeException(
                    "x", "Parameter `lengthX` is outside of the stage size.");
            if (z + lengthZ > zRange.end)
                throw new ArgumentOutOfRangeException(
                    "z", "Parameter `lengthZ` is outside of the stage size.");
        }
        #endregion
        #region Region Modification Methods
        /// <summary>
        /// Checks whether a given range of tiles are free.
        /// </summary>
        /// <param name="x">The lower x value of the region.</param>
        /// <param name="z">The lower z value of the region.</param>
        /// <param name="lengthX">The x length of the region.</param>
        /// <param name="lengthZ">The z length of the region.</param>
        /// <returns>True if the region has completely free.</returns>
        public bool IsRegionFree(int x, int z, int lengthX, int lengthZ)
        {
            ValidateRegionArgs(x, z, lengthX, lengthZ);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthZ; j++)
                    if (IsTileFilled(x + i, z + j))
                        return false;
            return true;
        }
        /// <summary>
        /// Frees out a region of the stage.
        /// </summary>
        /// <param name="x">The lower x value of the region.</param>
        /// <param name="z">The lower z value of the region.</param>
        /// <param name="lengthX">The x length of the region.</param>
        /// <param name="lengthZ">The z length of the region.</param>
        public void FreeRegion(int x, int z, int lengthX, int lengthZ)
        {
            ValidateRegionArgs(x, z, lengthX, lengthZ);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthZ; j++)
                    SetTile(x + i, z + j, false);
        }
        /// <summary>
        /// Blocks out a region of the stage.
        /// </summary>
        /// <param name="x">The lower x value of the region.</param>
        /// <param name="z">The lower z value of the region.</param>
        /// <param name="lengthX">The x length of the region.</param>
        /// <param name="lengthZ">The z length of the region.</param>
        public void FillRegion(int x, int z, int lengthX, int lengthZ)
        {
            ValidateRegionArgs(x, z, lengthX, lengthZ);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthZ; j++)
                    SetTile(x + i, z + j, true);
        }
        #endregion
    }
}
