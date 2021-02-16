using UnityEngine;
using StageEditor.Commands;
using CSharpUtilities.Collections;

namespace StageEditor
{
    #region Delegates
    /// <summary>
    /// A listener that responds to information about changed command state.
    /// </summary>
    /// <param name="canUndo">Whether a command can be undone in the new state.</param>
    /// <param name="canRedo">Whether a command can be redone in the new state.</param>
    public delegate void CommandsUpdatedListener(bool canUndo, bool canRedo);
    #endregion
    // TODO this class should have further abstracted base layers.
    /// <summary>
    /// Handles the commands for the stage editor.
    /// </summary>
    public sealed class StageEditorCommandManager : MonoBehaviour
    {
        #region Piping Events
        /// <summary>
        /// Called every time the command tree changes.
        /// Provides information about available traversal.
        /// </summary>
        public event CommandsUpdatedListener CommandsUpdated;
        #endregion
        #region Command Structure State Fields
        private Tree<StageEditorCommand> tree;
        #endregion
        #region Initialization
        private void Awake()
        {
            // Bootstrap the tree structure with a basic command.
            InitializeStageCommand bootStrapper = new InitializeStageCommand();
            tree = new Tree<StageEditorCommand>(bootStrapper);
            bootStrapper.Execute();
        }
        #endregion
        #region Command Methods
        /// <summary>
        /// Executes a command and adds it to the command tree.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void Do(StageEditorCommand command)
        {
            // Allows for overwritten commands to clean up
            // after themselves.
            foreach (StageEditorCommand subCommand in tree.GetAllChildrenBelowLocation())
                subCommand.Delete();
            // This explicitly removes branched history on
            // the tree. TODO would be interesting to
            // explore branched history.
            while (tree.Children.Count > 0)
                tree.RemoveChildBranch(tree.Children[0]);
            // Execute the command.
            command.Execute();
            // Add the command to the tree and move down to it.
            tree.AddChildren(command);
            tree.StepIn(0);
            // Broadcast information about the command state.
            CommandsUpdated?.Invoke(true, tree.Children.Count > 0);
        }
        /// <summary>
        /// Undoes the most recent command.
        /// </summary>
        public void Undo()
        {
            // Undo the command and step up a level
            // in the command tree.
            tree.Current.Undo();
            tree.StepOut();
            // Broadcast information about the command state.
            CommandsUpdated?.Invoke(!tree.IsAtRoot, true);
        }
        /// <summary>
        /// Redoes a command that was undone.
        /// </summary>
        public void Redo()
        {
            // Step down to the next child and re-execute
            // its command implementation.
            tree.StepIn(0);
            tree.Current.Execute();
            // Broadcast information about the command state.
            CommandsUpdated?.Invoke(true, tree.Children.Count > 0);
        }
        #endregion
    }
}
