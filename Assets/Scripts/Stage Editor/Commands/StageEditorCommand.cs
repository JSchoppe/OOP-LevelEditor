using System;
using CommandsCommon;

namespace StageEditor.Commands
{
    /// <summary>
    /// Base class for all stage editor commands.
    /// </summary>
    public abstract class StageEditorCommand : IReversibleCommand
    {
        #region State Fields
        private bool inExecutedState;
        #endregion
        #region Abstract Constructor
        public StageEditorCommand()
        {
            inExecutedState = false;
        }
        #endregion
        #region Properties
        /// <summary>
        /// The text for this command that faces the user.
        /// </summary>
        public virtual string ScreenName => "Command";
        #endregion
        #region Reversible Command Implementation
        /// <summary>
        /// Executes this command.
        /// </summary>
        public virtual void Execute()
        {
            // Throw error if these methods are used out of order.
            if (inExecutedState)
                throw new InvalidOperationException("Tried to execute a command that was already in executed state.");
            else
                inExecutedState = true;
        }
        /// <summary>
        /// Reverts the changes made by this command.
        /// </summary>
        public virtual void Undo()
        {
            // Throw error if these methods are used out of order.
            if (!inExecutedState)
                throw new InvalidOperationException("Cannot undo command before executing command.");
            else
                inExecutedState = false;
        }
        #endregion
        #region Optional Cleanup Method
        /// <summary>
        /// Tells this command that it will not be called
        /// again and can release any resources.
        /// </summary>
        public virtual void Delete()
        {

        }
        #endregion
    }
}
