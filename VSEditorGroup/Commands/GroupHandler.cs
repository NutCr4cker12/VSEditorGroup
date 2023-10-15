using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSEditorGroup.Commands
{
    internal static class GroupHandler
    {
        public static IList<Document> GetRelevantDocuments(DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dic = new Dictionary<string, Document>(dte.Documents.Count);

            for (var i = 0; i < dte.Documents.Count; i++)
            {
                var doc = dte.Documents.Item(i + 1);
                if (doc.ActiveWindow.IsFloating ||
                    doc.Type != "Text" ||
                    dic.ContainsKey(doc.FullName) ||
                    doc.ActiveWindow.Left == 0)
                    continue;

                dic.Add(doc.FullName, doc);
            }

            return dic.Values.ToList();
        }

        public static void ActivateGroup(IList<Document> editorGroups, int editorGroupIdx)
        {
            if (editorGroupIdx < 0 || editorGroupIdx >= editorGroups.Count)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            var idx = 0;
            var topOrdered = editorGroups
                .Select(n => n.ActiveWindow.Top)
                .Distinct()
                .OrderBy(n => n);

            foreach (var top in topOrdered)
            {
                var leftOrdered = editorGroups
                    .Where(n => n.ActiveWindow.Top == top)
                    .OrderBy(n => n.ActiveWindow.Left);

                foreach (var doc in leftOrdered)
                {
                    if (idx == editorGroupIdx)
                    {
                        ActivateDocument(doc);
                        return;
                    }
                    idx++;
                }
            }
            throw new Exception("Should not happen!");
        }

        private static void ActivateDocument(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            document.Activate();
        }
    }
}
