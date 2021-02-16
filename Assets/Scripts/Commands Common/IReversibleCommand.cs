namespace CommandsCommon
{
    /// <summary>
    /// Implements a command that can be executed and reversed.
    /// </summary>
    public interface IReversibleCommand
    {
        #region Method Implementations
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
        /// <summary>
        /// Reverses the effect of executing the command.
        /// </summary>
        void Undo();
        #endregion
    }
}
