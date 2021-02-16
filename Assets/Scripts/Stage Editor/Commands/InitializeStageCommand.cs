namespace StageEditor.Commands
{
    /// <summary>
    /// A default command that runs when the
    /// stage editor has been opened.
    /// </summary>
    public sealed class InitializeStageCommand : StageEditorCommand
    {
        #region Screen Name
        /// <summary>
        /// The screen name for the initialize stage command.
        /// </summary>
        public override string ScreenName => "New Map Created";
        #endregion
        #region Command Implementation
        public override void Execute()
        {
        
        }
        public override void Undo()
        {
        
        }
        #endregion
    }
}
