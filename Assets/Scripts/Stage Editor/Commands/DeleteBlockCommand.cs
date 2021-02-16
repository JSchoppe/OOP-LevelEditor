using UnityEngine;
using CSharpUtilities.Geometry;

// TODO this script still needs to be refactored.

namespace StageEditor.Commands
{
    public sealed class DeleteBlockCommand : StageEditorCommand
    {
        private readonly GameObject blockObject;
        private readonly Mesh blockMesh;
        private readonly RectInt rect;
        private readonly StageEditorState stageState;

        public DeleteBlockCommand(GameObject block, StageEditorState stageState)
        {
            EditorBlockData data = block.GetComponent<EditorBlockData_SceneInstance>().Data;

            OrthoAngle angle = block.transform.eulerAngles.y.ToOrthoAngle();

            rect = data.GetGridRect(
                new Vector2(block.transform.position.x, block.transform.position.z),
                angle == OrthoAngle.Ninety || angle == OrthoAngle.TwoSeventy);


            blockObject = block;
            blockMesh = block.GetComponent<MeshFilter>().mesh;
            this.stageState = stageState;
        }


        public override void Execute()
        {
            base.Execute();

            stageState.FreeRegion(rect.x, rect.y, rect.width, rect.height);

            Object.Destroy(blockObject.GetComponent<MeshRenderer>());
            Object.Destroy(blockObject.GetComponent<MeshCollider>());
            Object.Destroy(blockObject.GetComponent<MeshFilter>());
        }

        public override void Undo()
        {
            base.Undo();

            stageState.FillRegion(rect.x, rect.y, rect.width, rect.height);

            blockObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            blockObject.AddComponent<MeshCollider>().sharedMesh = blockMesh;
            blockObject.AddComponent<MeshFilter>().sharedMesh = blockMesh;
        }

        public override void Delete()
        {
            base.Delete();
        }
    }
}
