using System;
using UnityEngine;

namespace StageEditor
{
    /// <summary>
    /// Editor block data that is exposed to the unity inspector.
    /// </summary>
    public sealed class EditorBlockData_SceneInstance : MonoBehaviour
    {
        #region Inspector Fields
        [Tooltip("The dimensions of the block.")]
        [SerializeField] private Vector3Int dimensions = Vector3Int.one;
        [Tooltip("The transform pivot of the block.")]
        [SerializeField] private Vector3 pivot = Vector3.one;
        [Tooltip("The mesh associated with the block.")]
        [SerializeField] private Mesh associatedMesh = null;
        private void OnValidate()
        {
            // Keep values greater than zero.
            if (dimensions.x < 1)
                dimensions.x = 1;
            if (dimensions.y < 1)
                dimensions.y = 1;
            if (dimensions.z < 1)
                dimensions.z = 1;
        }
        #endregion
        #region Non-Mono Accessor
        /// <summary>
        /// Generates a copy of the block data from the inspector.
        /// </summary>
        public EditorBlockData Data
        {
            get => new EditorBlockData(dimensions, pivot, associatedMesh);
            set
            {
                dimensions = value.size;
                pivot = value.pivot;
                associatedMesh = value.mesh;
            }
        }
        #endregion
    }
    /// <summary>
    /// The immutable data associated with blocks placed in the editor.
    /// </summary>
    public sealed class EditorBlockData
    {
        #region Fields
        /// <summary>
        /// The dimensions of the block.
        /// </summary>
        public readonly Vector3Int size;
        /// <summary>
        /// The location of the transform pivot of the block.
        /// </summary>
        public readonly Vector3 pivot;
        /// <summary>
        /// The mesh associated with the block.
        /// </summary>
        public readonly Mesh mesh;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new instance of editor block data.
        /// </summary>
        /// <param name="size">The dimensions of the block.</param>
        /// <param name="pivot">The pivot point for the block transform.</param>
        /// <param name="mesh">The mesh associated with the block.</param>
        public EditorBlockData(Vector3Int size, Vector3 pivot, Mesh mesh)
        {
            // Enforce valid sizing.
            if (size.x < 1 || size.y < 1 || size.z < 1)
                throw new ArgumentOutOfRangeException("size", "Size values must be greater than zero.");
            this.size = size;
            this.pivot = pivot;
            this.mesh = mesh;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Calculates the rect of the block given the
        /// current state of the block.
        /// </summary>
        /// <param name="location">The current location of the block pivot.</param>
        /// <param name="isPivoted">Whether the block is turned perpendicular.</param>
        /// <returns>The current grid rect of the block.</returns>
        public RectInt GetGridRect(Vector2 location, bool isPivoted)
        {
            if (!isPivoted)
            {
                location -= new Vector2(pivot.x, pivot.z);
                return new RectInt(
                    Mathf.RoundToInt(location.x),
                    Mathf.FloorToInt(location.y),
                    size.x, size.z);
            }
            else
            {
                location -= new Vector2(pivot.z, pivot.x);
                return new RectInt(
                    Mathf.RoundToInt(location.x),
                    Mathf.FloorToInt(location.y),
                    size.z, size.x);
            }
        }
        #endregion
    }
}
