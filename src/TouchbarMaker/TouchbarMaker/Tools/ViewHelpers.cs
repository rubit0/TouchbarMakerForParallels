using System.Windows.Controls;

namespace TouchbarMaker.Tools
{
    public static class ViewHelpers
    {
        // source: https://stackoverflow.com/a/6506938
        /// <summary>
        /// Clear current selected item of a TreeView
        /// </summary>
        /// <param name="input">Target TreeView input</param>
        public static void ClearSelection(this TreeView input)
        {
            var selected = input.SelectedItem;
            if (selected == null)
                return;

            if (!(input.ContainerFromItem(selected) is TreeViewItem treeViewItem))
                return;

            treeViewItem.IsSelected = false;
        }

        // source: https://stackoverflow.com/a/6506938
        private static TreeViewItem ContainerFromItem(this TreeView treeView, object item)
        {
            var containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
            if (containerThatMightContainItem != null)
                return containerThatMightContainItem;
            else
                return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
        }

        // source: https://stackoverflow.com/a/6506938
        private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)
        {
            foreach (object curChildItem in itemCollection)
            {
                var parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                    return null;
                var containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (containerThatMightContainItem != null)
                    return containerThatMightContainItem;
                var recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
                if (recursionResult != null)
                    return recursionResult;
            }
            return null;
        }
    }
}
