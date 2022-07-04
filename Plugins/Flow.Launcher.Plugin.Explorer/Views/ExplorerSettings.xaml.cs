using Flow.Launcher.Plugin.Explorer.Search.QuickAccessLinks;
using Flow.Launcher.Plugin.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;

namespace Flow.Launcher.Plugin.Explorer.Views
{
    /// <summary>
    /// Interaction logic for ExplorerSettings.xaml
    /// </summary>
    public partial class ExplorerSettings
    {
        private readonly SettingsViewModel viewModel;

        private List<ActionKeywordModel> actionKeywordsListView;


        public ExplorerSettings(SettingsViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();

            this.viewModel = viewModel;

            DataContext = viewModel;

            ActionKeywordModel.Init(viewModel.Settings);

            lbxAccessLinks.Items.SortDescriptions.Add(new SortDescription("Path", ListSortDirection.Ascending));

            lbxExcludedPaths.Items.SortDescriptions.Add(new SortDescription("Path", ListSortDirection.Ascending));
        }


        
        private void AccessLinkDragDrop(string containerName, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || !files.Any())
            {
                return;
            }
            foreach (var s in files)
            {
                if (Directory.Exists(s))
                {
                    var newFolderLink = new AccessLink
                    {
                        Path = s
                    };
                    viewModel.AppendLink(containerName, newFolderLink);
                }
            }
        }

        private void lbxAccessLinks_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void btnOpenIndexingOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsViewModel.OpenWindowsIndexingOptions();
        }
        private void EverythingSortOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbFastSortWarning is not null)
            {
                tbFastSortWarning.Visibility = viewModel.FastSortWarningVisibility;
                tbFastSortWarning.Text = viewModel.SortOptionWarningMessage;
            }
        }
        private void SettingExpander_OnExpanded(object sender, RoutedEventArgs e)
        {
            if (sender is not Expander expander)
                return;

            var parentContainer = VisualTreeHelper.GetParent(expander);

            if (parentContainer is not StackPanel stackPanel)
                return;

            foreach (UIElement child in stackPanel.Children)
            {
                if (child != expander)
                    child.Visibility = Visibility.Collapsed;
            }
        }
        private void SettingExpander_OnCollapsed(object sender, RoutedEventArgs e)
        {
            if (sender is not Expander expander)
                return;

            var parentContainer = VisualTreeHelper.GetParent(expander);

            if (parentContainer is not StackPanel stackPanel)
                return;

            foreach (UIElement child in stackPanel.Children)
            {
                if (child != expander)
                    child.Visibility = Visibility.Visible;
            }
        }
        private void LbxAccessLinks_OnDrop(object sender, DragEventArgs e)
        {
            AccessLinkDragDrop("QuickAccessLink", e);
        }
        private void LbxExcludedPaths_OnDrop(object sender, DragEventArgs e)
        {
            AccessLinkDragDrop("IndexSearchExcludedPath", e);
        }
    }
}