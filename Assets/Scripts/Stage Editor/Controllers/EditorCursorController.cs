using UnityEngine;
using InputBroadcasters;
using StageEditor.UI;
using StageEditor.Commands;
using UnityUtilities.InspectorFields;
using CSharpUtilities.Geometry;

// TODO this script still needs to be refactored.
// TODO consider adding state pattern, this script is unruly.

namespace StageEditor.Controllers
{
    public sealed class EditorCursorController : MonoBehaviour
    {

        public RangeInt RangeX { get; private set; }
        public RangeInt RangeZ { get; private set; }

        public Vector2Int CurrentTile { get; private set; }

        private OrthoAngle blockRotation;
        private bool blockIsPlaceable;

        [SerializeField] private ColliderCursorBroadcaster colliderClickBroadcaster = null;
        [SerializeField] private Transform cameraPivot = null;
        [SerializeField] private RangeInt_InspectorField tileRangeX = null;
        [SerializeField] private RangeInt_InspectorField tileRangeZ = null;
        [SerializeField] private float camMoveSpeed = 1f;
        [SerializeField] private float camRotateSpeed = 1f;

        [SerializeField] private Material placeableBlockMaterial = null;
        [SerializeField] private Material unplaceableBlockMaterial = null;

        private void OnValidate()
        {
            tileRangeX.OnValidate();
            tileRangeZ.OnValidate();
        }

        [SerializeField] private EditorInteractionLogic interactionLogic = null;
        [SerializeField] private MeshFilter newBlockFilter = null;
        [SerializeField] private MeshRenderer newBlockRenderer = null;
        [SerializeField] private StageEditorCommandManager commandManager = null;

        private Vector3Int selectionSize;
        private EditorBlockData selectedBlockData;
        private EditorInteractionLogic.EditMode mode;

        private StageEditorState stageState;

        private void Awake()
        {
            interactionLogic.NewBlockSelected += OnNewPieceSelected;
            interactionLogic.EditModeChanged += OnEditModeChanged;
            colliderClickBroadcaster.ColliderHovered += OnColliderHovered;
            colliderClickBroadcaster.ColliderClicked += OnColliderClicked;

            RangeX = tileRangeX.Value;
            RangeZ = tileRangeZ.Value;

            newBlockRenderer.material = placeableBlockMaterial;

            blockRotation = OrthoAngle.Zero;
            blockIsPlaceable = true;
            selectionSize = Vector3Int.one;
            stageState = new StageEditorState(RangeX, RangeZ);
        }

        private void OnEditModeChanged(EditorInteractionLogic.EditMode mode)
        {
            this.mode = mode;
            switch (mode)
            {
                case EditorInteractionLogic.EditMode.None:
                    newBlockRenderer.enabled = false;
                    selectedBlockData = null;
                    newBlockFilter.mesh = null;
                    colliderClickBroadcaster.LayerMask &= ~(1 << LayerMask.NameToLayer("Block"));
                    if (currentDeleteHovered != null)
                        currentDeleteHovered.material = new Material(Shader.Find("Diffuse"));
                    break;
                case EditorInteractionLogic.EditMode.Insert:
                    newBlockRenderer.enabled = true;
                    break;
                case EditorInteractionLogic.EditMode.Delete:
                    colliderClickBroadcaster.LayerMask |= (1 << LayerMask.NameToLayer("Block"));
                    break;
            }
        }

        private void OnColliderClicked(RaycastHit hitInfo)
        {
            switch (mode)
            {
                case EditorInteractionLogic.EditMode.Insert:
                    OnColliderClickedInsertMode(hitInfo); break;
                case EditorInteractionLogic.EditMode.Delete:
                    OnColliderClickedDeleteMode(hitInfo); break;
            }
        }
        private void OnColliderClickedInsertMode(RaycastHit hitInfo)
        {
            if (selectedBlockData != null && blockIsPlaceable)
            {
                commandManager.Do(new PlaceBlockCommand(selectedBlockData,
                    newBlockFilter.transform.position, newBlockFilter.transform.eulerAngles.y.ToOrthoAngle(), stageState));
            }
        }
        private void OnColliderClickedDeleteMode(RaycastHit hitInfo)
        {
            if (currentDeleteHovered != null
                && currentDeleteHovered.name != "Blocking Quad")
            {
                commandManager.Do(new DeleteBlockCommand(
                    currentDeleteHovered.gameObject, stageState
                    ));
            }
        }

        private void OnColliderHovered(RaycastHit hitInfo)
        {
            switch (mode)
            {
                case EditorInteractionLogic.EditMode.Insert:
                    OnColliderHoveredInsert(hitInfo); break;
                case EditorInteractionLogic.EditMode.Delete:
                    OnColliderHoveredDelete(hitInfo); break;
            }
        }
        private void OnColliderHoveredInsert(RaycastHit hitInfo)
        {
            Vector3 cursorPosition = new Vector3();

            if (selectionSize.x % 2 == 0)
                cursorPosition.x = Mathf.CeilToInt(hitInfo.point.x);
            else
                cursorPosition.x = Mathf.CeilToInt(hitInfo.point.x + 0.5f) - 0.5f;

            if (selectionSize.z % 2 == 0)
                cursorPosition.z = Mathf.CeilToInt(hitInfo.point.z);
            else
                cursorPosition.z = Mathf.CeilToInt(hitInfo.point.z + 0.5f) - 0.5f;

            while (cursorPosition.x - (selectionSize.x * 0.5f) < RangeX.start)
                cursorPosition.x++;
            while (cursorPosition.x + (selectionSize.x * 0.5f) > RangeX.end)
                cursorPosition.x--;

            while (cursorPosition.z - (selectionSize.z * 0.5f) < RangeZ.start)
                cursorPosition.z++;
            while (cursorPosition.z + (selectionSize.z * 0.5f) > RangeZ.end)
                cursorPosition.z--;

            newBlockFilter.transform.position = cursorPosition;

            if (selectedBlockData != null)
            {
                if (blockRotation == OrthoAngle.Zero || blockRotation == OrthoAngle.OneEighty)
                {
                    blockIsPlaceable =
                        stageState.IsRegionFree(
                            Mathf.FloorToInt(newBlockFilter.transform.position.x - selectedBlockData.size.x / 2f),
                            Mathf.FloorToInt(newBlockFilter.transform.position.z - selectedBlockData.size.z / 2f),
                            selectedBlockData.size.x, selectedBlockData.size.z);
                }
                else
                {
                    blockIsPlaceable =
                        stageState.IsRegionFree(
                            Mathf.FloorToInt(newBlockFilter.transform.position.x - selectedBlockData.size.z / 2f),
                            Mathf.FloorToInt(newBlockFilter.transform.position.z - selectedBlockData.size.x / 2f),
                            selectedBlockData.size.z, selectedBlockData.size.x);
                }
                if (blockIsPlaceable)
                    newBlockFilter.GetComponent<MeshRenderer>().material = placeableBlockMaterial;
                else
                    newBlockFilter.GetComponent<MeshRenderer>().material = unplaceableBlockMaterial;
            }
        }

        private MeshRenderer currentDeleteHovered = null;
        private void OnColliderHoveredDelete(RaycastHit hitInfo)
        {
            MeshRenderer hitBlock;
            if (LayerMask.NameToLayer("Default") == (LayerMask.NameToLayer("Default") | (1 << hitInfo.transform.gameObject.layer)))
                hitBlock = null;
            else
                hitBlock = hitInfo.transform.GetComponent<MeshRenderer>();

            if (currentDeleteHovered != hitBlock)
            {
                if (currentDeleteHovered != null)
                    currentDeleteHovered.material = new Material(Shader.Find("Diffuse"));
                currentDeleteHovered = hitBlock;
                if (currentDeleteHovered != null)
                    currentDeleteHovered.material = unplaceableBlockMaterial;
            }    
        }

        private void OnNewPieceSelected(EditorBlockData data)
        {
            selectedBlockData = data;
            newBlockFilter.sharedMesh = data.mesh;
            selectionSize = data.size;
        }

        private void OnDestroy()
        {
            // Help the garbage collector.
            interactionLogic.NewBlockSelected -= OnNewPieceSelected;
            interactionLogic.EditModeChanged -= OnEditModeChanged;
            colliderClickBroadcaster.ColliderHovered -= OnColliderHovered;
            colliderClickBroadcaster.ColliderClicked -= OnColliderClicked;
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                blockRotation.Advance();
                newBlockFilter.transform.rotation =
                    Quaternion.AngleAxis(blockRotation.ToDegrees(), Vector3.up);
            }

            float rotationInput = (Input.GetKey(KeyCode.Q) ? -1f : 0f) +
                (Input.GetKey(KeyCode.E) ? 1f : 0f);

            float forwardsBackInput = (Input.GetKey(KeyCode.S) ? -1f : 0f) +
                (Input.GetKey(KeyCode.W) ? 1f : 0f);

            float strafeInput = (Input.GetKey(KeyCode.A) ? -1f : 0f) +
                (Input.GetKey(KeyCode.D) ? 1f : 0f);

            cameraPivot.rotation *= Quaternion.AngleAxis(rotationInput * camRotateSpeed * Time.deltaTime, Vector3.up);

            transform.position += cameraPivot.forward * forwardsBackInput * camMoveSpeed * Time.deltaTime
                + cameraPivot.right * strafeInput * camMoveSpeed * Time.deltaTime;

            transform.position = new Vector3
            {
                x = Mathf.Clamp(transform.position.x, RangeX.start, RangeX.end),
                y = transform.position.y,
                z = Mathf.Clamp(transform.position.z, RangeZ.start, RangeZ.end)
            };
        }
    }
}
