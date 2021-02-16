using System;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities.UI;

// TODO this script still needs to be refactored.

namespace StageEditor.UI
{
    /// <summary>
    /// Defines the UI logic for the stage editor.
    /// </summary>
    public sealed class EditorInteractionLogic : MonoBehaviour
    {
        #region Piping Events
        /// <summary>
        /// Called when the user has selected a new block from the insert menu.
        /// </summary>
        public event Action<EditorBlockData> NewBlockSelected;
        /// <summary>
        /// Called when the user switches edit modes.
        /// </summary>
        public event Action<EditMode> EditModeChanged;
        #endregion
        #region Inspector Fields
        [Header("Driving References")]
        [Tooltip("The driver for the undo/redo interactibility.")]
        [SerializeField] private StageEditorCommandManager commandManager = null;
        [Header("UI References")]
        [Tooltip("The button that opens the insertion menu.")]
        [SerializeField] private Button insertToolButton = null;
        [Tooltip("The button that starts deletion mode.")]
        [SerializeField] private Button deleteToolButton = null;
        [Tooltip("The button that starts selection mode.")]
        [SerializeField] private Button selectToolButton = null;
        [Tooltip("The button that starts translation mode.")]
        [SerializeField] private Button translateToolButton = null;
        [Tooltip("The button that starts rotation mode.")]
        [SerializeField] private Button rotateToolButton = null;
        [Tooltip("The button that starts clone mode.")]
        [SerializeField] private Button cloneToolButton = null;
        [Tooltip("The button that invokes undo.")]
        [SerializeField] private Button undoButton = null;
        [Tooltip("The button that invokes redo.")]
        [SerializeField] private Button redoButton = null;
        [Tooltip("The root GameObject for the block insertion panel.")]
        [SerializeField] private GameObject insertPanel = null;
        [Tooltip("The root GameObject which holds all block buttons.")]
        [SerializeField] private GameObject insertButtonsRoot = null;
        #endregion
        #region Interaction State Fields
        private EditMode mode;
        #endregion
        #region Initialization
        private void Awake()
        {
            mode = EditMode.None;
            // Listen for changes in command structure.
            commandManager.CommandsUpdated += OnCommandsUpdated;
            // Set initial insertion menu state.
            insertPanel.SetActive(false);
            // Bind the UI elements.
            undoButton.onClick.AddListener(OnUndoClick);
            redoButton.onClick.AddListener(OnRedoClick);
            insertToolButton.onClick.AddListener(OnInsertToolClick);
            deleteToolButton.onClick.AddListener(OnDeleteToolClick);
            selectToolButton.onClick.AddListener(OnSelectToolClick);
            translateToolButton.onClick.AddListener(OnTranslateToolClick);
            rotateToolButton.onClick.AddListener(OnRotateToolClick);
            cloneToolButton.onClick.AddListener(OnCloneToolClick);
            // Bind every button for block selection.
            foreach (Transform child in insertButtonsRoot.transform)
            {
                Button button = child.GetComponent<Button>();
                button.onClick.AddListener(() => { OnBlockButtonPressed(button); });
            }

            buttons = new Button[]
            {
                insertToolButton,
                deleteToolButton,
                selectToolButton,
                translateToolButton,
                rotateToolButton,
                cloneToolButton
            };
        }
        #endregion


        private Button[] buttons;

        private EditMode Mode
        {
            set
            {
                mode = value;
                EditModeChanged?.Invoke(mode);
            }
        }

        public enum EditMode : byte
        {
            None, Insert, Delete, Select, Translate, Rotate, Clone
        }


        private void OnBlockButtonPressed(Button button)
        {
            // Retrieve the block data and send it off (to the cursor).
            NewBlockSelected?.Invoke(
                button.GetComponent<EditorBlockData_SceneInstance>().Data);
        }

        private void OnCommandsUpdated(bool canUndo, bool canRedo)
        {
            // Set the interactability of undo and redo based on
            // the broadcasted state of the command manager.
            undoButton.interactable = canUndo;
            redoButton.interactable = canRedo;
        }

        private void OnUndoClick()
        {
            commandManager.Undo();
        }
        private void OnRedoClick()
        {
            commandManager.Redo();
        }


        private void OnInsertToolClick()
        {
            if (mode == EditMode.Insert)
            {
                // Update insert panel visibility.
                insertPanel.SetActive(false);
                insertToolButton.Deselect();
                // Broadcast state change.
                Mode = EditMode.None;
                // Set button state to control the flow
                // of actions by the user.
                SetOneButton(false, insertToolButton);
            }
            else
            {
                insertPanel.SetActive(true);
                Mode = EditMode.Insert;
                SetOneButton(true, insertToolButton);
            }
        }
        private void OnDeleteToolClick()
        {
            if (mode == EditMode.Delete)
            {
                deleteToolButton.Deselect();
                // Broadcast state change.
                Mode = EditMode.None;
                // Set button state to control the flow
                // of actions by the user.
                SetOneButton(false, deleteToolButton);
            }
            else
            {
                Mode = EditMode.Delete;
                SetOneButton(true, deleteToolButton);
            }
        }

        private void SetOneButton(bool state, Button targetButton)
        {
            foreach (Button button in buttons)
                button.interactable = (button == targetButton) ? state : !state;
        }

        private void OnSelectToolClick()
        {

        }
        private void OnTranslateToolClick()
        {

        }
        private void OnRotateToolClick()
        {

        }
        private void OnCloneToolClick()
        {

        }
    }
}
