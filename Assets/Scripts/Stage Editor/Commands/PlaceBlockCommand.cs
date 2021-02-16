using UnityEngine;
using CSharpUtilities.Geometry;

namespace StageEditor.Commands
{
    /// <summary>
    /// Places a block on the stage editor grid, blocking out that region.
    /// </summary>
    public sealed class PlaceBlockCommand : StageEditorCommand
    {
        #region Command Reference Fields
        private readonly GameObject spawnedInstance;
        private readonly StageEditorState stageState;
        #endregion
        #region Command Parameter Fields
        private readonly EditorBlockData blockData;
        private readonly Vector3 atLocation;
        private readonly OrthoAngle rotation;
        private readonly RectInt rect;
        #endregion
        #region Constructor + Command Initialization
        /// <summary>
        /// Creates a command to place a block at the given location and rotation.
        /// </summary>
        /// <param name="blockData">Describes the block being placed.</param>
        /// <param name="atLocation">The location to place the block at.</param>
        /// <param name="rotation">The rotation of this block.</param>
        /// <param name="stageState">The persistent stage state object.</param>
        public PlaceBlockCommand(EditorBlockData blockData, Vector3 atLocation, OrthoAngle rotation, StageEditorState stageState)
        {
            this.blockData = blockData;
            this.atLocation = atLocation;
            this.rotation = rotation;
            this.stageState = stageState;
            // Calculate the rect that this block will effect.
            rect = blockData.GetGridRect(
                new Vector2(atLocation.x, atLocation.z),
                rotation == OrthoAngle.Ninety || rotation == OrthoAngle.TwoSeventy);
            // Create a new instance for this command.
            spawnedInstance = new GameObject
            {
                layer = LayerMask.NameToLayer("Block")
            };
            // Allow future commands to access this block data.
            spawnedInstance.AddComponent<EditorBlockData_SceneInstance>().Data = blockData;
        }
        #endregion
        #region Screen Name
        /// <summary>
        /// The screen name for the place block command.
        /// </summary>
        public override string ScreenName => "Block Placed";
        #endregion
        #region Command Implementation
        /// <summary>
        /// Executes the command for block placement.
        /// </summary>
        public override void Execute()
        {
            base.Execute();
            // Block out the tiles that this block occupies.
            stageState.FillRegion(rect.x, rect.y, rect.width, rect.height);
            // Add the visual components to the GameObject.
            spawnedInstance.transform.position = atLocation;
            spawnedInstance.transform.rotation = Quaternion.AngleAxis(rotation.ToDegrees(), Vector3.up);
            spawnedInstance.AddComponent<MeshRenderer>().material =
                new Material(Shader.Find("Diffuse"));
            spawnedInstance.AddComponent<MeshFilter>().sharedMesh = blockData.mesh;
            spawnedInstance.AddComponent<MeshCollider>().sharedMesh = blockData.mesh;
        }
        /// <summary>
        /// Reverts the command for block placement.
        /// </summary>
        public override void Undo()
        {
            base.Undo();
            // Free up the region that was filled.
            stageState.FreeRegion(rect.x, rect.y, rect.width, rect.height);
            // Remove the components from the GameObject.
            Object.Destroy(spawnedInstance.GetComponent<MeshRenderer>());
            Object.Destroy(spawnedInstance.GetComponent<MeshFilter>());
            Object.Destroy(spawnedInstance.GetComponent<MeshCollider>());
        }
        #endregion
        #region Clean Up
        public override void Delete()
        {
            base.Delete();
            // Remove the root object associated with
            // the block placement command.
            Object.Destroy(spawnedInstance);
        }
        #endregion
    }
}
