﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TLCGen.Controls
{
    /// <summary>
    /// Interaction logic for DetectorManagerView.xaml
    /// </summary>
    public partial class ItemsManagerView : UserControl
    {
        public ItemsManagerView()
        {
            InitializeComponent();
        }

        public Visibility SelectableItemsVisibility
        {
            get => (Visibility)GetValue(SelectableItemsVisibilityProperty);
            set => SetValue(SelectableItemsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectableItemsVisibilityProperty =
            DependencyProperty.Register("SelectableItemsVisibility", typeof(Visibility), typeof(ItemsManagerView), new PropertyMetadata(Visibility.Collapsed));


        public Visibility RemovableItemsVisibility
        {
            get => (Visibility)GetValue(RemovableItemsVisibilityProperty);
            set => SetValue(RemovableItemsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for RemovableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovableItemsVisibilityProperty =
            DependencyProperty.Register("RemovableItemsVisibility", typeof(Visibility), typeof(ItemsManagerView), new PropertyMetadata(Visibility.Collapsed));

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(ItemsManagerView), new PropertyMetadata("Items"));

        public bool ShowCaption
        {
            get => (bool)GetValue(ShowCaptionProperty);
            set => SetValue(ShowCaptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowCaption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register("ShowCaption", typeof(bool), typeof(ItemsManagerView), new PropertyMetadata(true));

        public Array SelectableItems
        {
            get => (Array)GetValue(SelectableItemsProperty);
            set => SetValue(SelectableItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectableItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectableItemsProperty =
            DependencyProperty.Register("SelectableItems", typeof(IList), typeof(ItemsManagerView), new PropertyMetadata(null));

        public Array RemovableItems
        {
            get => (Array)GetValue(RemovableItemsProperty);
            set => SetValue(RemovableItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for RemovableItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovableItemsProperty =
            DependencyProperty.Register("RemovableItems", typeof(IList), typeof(ItemsManagerView), new PropertyMetadata(null));

        public object SelectedItemToAdd
        {
            get => (object)GetValue(SelectedItemToAddProperty);
            set => SetValue(SelectedItemToAddProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItemToAdd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemToAddProperty =
            DependencyProperty.Register("SelectedItemToAdd", typeof(object), typeof(ItemsManagerView), new PropertyMetadata(null));

        public object SelectedItemToRemove
        {
            get => (object)GetValue(SelectedItemToRemoveProperty);
            set => SetValue(SelectedItemToRemoveProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItemToRemove.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemToRemoveProperty =
            DependencyProperty.Register("SelectedItemToRemove", typeof(object), typeof(ItemsManagerView), new PropertyMetadata(null));

        public ICommand AddItemCommand
        {
            get => (ICommand)GetValue(AddItemCommandProperty);
            set => SetValue(AddItemCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for AddItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(ItemsManagerView), new PropertyMetadata(null));

        public ICommand RemoveItemCommand
        {
            get => (ICommand)GetValue(RemoveItemCommandProperty);
            set => SetValue(RemoveItemCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for RemoveItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveItemCommandProperty =
            DependencyProperty.Register("RemoveItemCommand", typeof(ICommand), typeof(ItemsManagerView), new PropertyMetadata(null));

    }
}
