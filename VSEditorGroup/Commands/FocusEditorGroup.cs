using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSEditorGroup.Commands
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal static class FocusEditorGroup
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int COMMAND_ID = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c97f2dcc-ff9c-4e05-b114-233b7c719fd4");

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner _package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, uint i)
        {
            // Switch to the main thread - the call to AddCommand in FocusEditorGroup's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            //new FocusEditorGroup(package, commandService, (int)i, COMMAND_ID + (int)i);

            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            //_editorGroupTarget = editorGroupTarget;
            var menuCommandID = new CommandID(CommandSet, COMMAND_ID + (int)i);
            var menuItem = new MenuCommand((s, e) =>
            {
                var editorGroup = (int)i;
                Execute(editorGroup, package);

                //var message = string.Format(CultureInfo.CurrentCulture, "Called editorGroup: {0}",
                //    editorGroup);
                //var title = "FocusEditorGroup";

                //// Show a message box to prove we were here
                //VsShellUtilities.ShowMessageBox(
                //    package,
                //    message,
                //    title,
                //    OLEMSGICON.OLEMSGICON_INFO,
                //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private static async Task Execute(int editorGroup, AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            dte = dte ?? throw new ArgumentNullException(nameof(dte));

            var documents = GroupHandler.GetRelevantDocuments(dte);
            GroupHandler.ActivateGroup(documents, editorGroup);
        }
    }
}
