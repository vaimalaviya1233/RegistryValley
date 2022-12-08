﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RegistryValley.App.Dialogs;
using RegistryValley.App.Models;
using RegistryValley.App.ViewModels;
using Windows.UI.Popups;

namespace RegistryValley.App.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var provider = App.Current.Services;
            ViewModel = provider.GetRequiredService<MainViewModel>();
        }

        public MainViewModel ViewModel { get; }

        private void OnDirTreeViewExpanding(TreeView sender, TreeViewExpandingEventArgs args)
        {
            ViewModel.Loading = true;

            var item = (KeyItem)args.Item;

            if (item.RootHive != HKEY.NULL)
            {
                item.Children.Clear();

                var children = ViewModel.RegValleyEnumKeys(item.RootHive, item.Path);

                if (children != null)
                {
                    foreach (var child in children)
                        item.Children.Add(child);

                    //args.Node.HasUnrealizedChildren = false;
                }
            }

            ViewModel.Loading = false;
        }

        private void OnDirTreeViewCollapsed(TreeView sender, TreeViewCollapsedEventArgs args)
        {
            var item = (KeyItem)args.Item;
            item.Children.Clear();
        }

        private void OnDirTreeViewItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            ViewModel.Loading = true;
            var item = (KeyItem)args.InvokedItem;

            if (item.RootHive != HKEY.NULL)
            {
                ViewModel.RegValleyEnumValues(item.RootHive, item.Path, item.Name);
            }

            ViewModel.Loading = false;
        }

        private async void OnValueListViewDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var item = (ValueItem)ValueListView.SelectedItem;

            var dialog = new ValueViewerDialog
            {
                ViewModel = new()
                {
                    ValueItem = item,
                },

                // WinUI3: https://github.com/microsoft/microsoft-ui-xaml/issues/2504
                XamlRoot = this.Content.XamlRoot,
            };
 
            var result = await dialog.ShowAsync();
        }

        private async void OnKeyPermissionsMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            var item = (KeyItem)((MenuFlyoutItem)sender).Tag;

            var dialog = new KeyPermissionsViewerDialog
            {
                ViewModel = new()
                {
                    KeyItem = item,
                },

                // WinUI3: https://github.com/microsoft/microsoft-ui-xaml/issues/2504
                XamlRoot = this.Content.XamlRoot,
            };

            var result = await dialog.ShowAsync();
        }
    }
}
