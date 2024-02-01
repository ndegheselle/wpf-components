﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfComponents.Lib.Components.Inputs
{
    /// <summary>
    /// Basé sur : https://stackoverflow.com/a/41986141/10404482 Exemple :
    /// <composants:ComboBoxFiltre><composants:ComboBoxFiltre.ItemsPanel><ItemsPanelTemplate><VirtualizingStackPanel
    /// VirtualizationMode="Recycling"/></ItemsPanelTemplate></composants:ComboBoxFiltre.ItemsPanel></composants:ComboBoxFiltre>
    ///
    /// </summary>
    public class ComboBoxSearch : ComboBox
    {
        private bool _preventTextUpdate;
        private TextBox _editableTextBox;

        public bool HideFilteredItems { get; set; } = true;

        public string? FilterMemberPath { get; set; }

        public ComboBoxSearch()
        {
            Style = (Style)Application.Current.FindResource(typeof(ComboBox));

            // Set default options
            IsEditable = true;
            StaysOpenOnEdit = true;
            IsTextSearchEnabled = false;
        }

        public override void OnApplyTemplate()
        {
            _editableTextBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
            _editableTextBox.FontStyle = FontStyles.Normal;
            if (SelectedItem != null)
                this.Text = ItemGetTextFrom(SelectedItem, DisplayMemberPath);

            base.OnApplyTemplate();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += DoesItemPassFilter;
            }

            if (oldValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(oldValue);
                if (view != null)
                    view.Filter -= DoesItemPassFilter;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    IsDropDownOpen = true;
                    if (SelectedItem == null)
                        SelectedIndex = Items.Count - 1;
                    break;
                case Key.Down:
                    IsDropDownOpen = true;
                    if (SelectedItem == null)
                        SelectedIndex = 0;
                    break;
                case Key.Tab:
                    case Key.Enter:
                    IsDropDownOpen = false;
                    break;
                case Key.Escape:
                    IsDropDownOpen = false;
                    SelectedItem = null;
                    break;
                default:
                    IsDropDownOpen = true;
                    break;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            RefreshFilter();
            base.OnPreviewKeyUp(e);
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Prevent having a value that doesn't match any item (could be misleading)
            if (SelectedItem == null)
                ClearFilter();

            base.OnPreviewLostKeyboardFocus(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            if (SelectedItem == null)
            {
                ClearFilter();
            }
            else if (HideFilteredItems == false)
            {
                Text = ItemGetTextFrom(SelectedItem, DisplayMemberPath);
            }

            base.OnDropDownClosed(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (_editableTextBox == null)
                return;

            // Show italic text if no item is selected
            if (e.AddedItems.Count == 0)
                _editableTextBox.FontStyle = FontStyles.Italic;
            else
                _editableTextBox.FontStyle = FontStyles.Normal;

            if (string.IsNullOrEmpty(Text) && SelectedItem != null)
                Text = ItemGetTextFrom(SelectedItem, DisplayMemberPath);

            e.Handled = true;
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null)
                return;

            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();

            SelectFromFilter();
        }

        private void SelectFromFilter()
        {
            if (string.IsNullOrEmpty(Text))
            {
                SelectedIndex = -1;
                return;
            }

            if (HideFilteredItems == false)
            {
                // Select closest to user input
                for (int i = 0; i < Items.Count; i++)
                {
                    if (DoesItemPassFilter(Items[i]))
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
            }
            else
            {
                // Select item that matches user input exactly
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Text == ItemGetTextFrom(Items[i], FilterMemberPath))
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
            }
            SelectedIndex = -1;
        }

        private void ClearFilter()
        {
            Text = string.Empty;
        }

        private bool DoesItemPassFilter(object value)
        {
            // If the filter is disabled, we don't filter the items
            if (HideFilteredItems == false)
                return true;

            if (value == null)
                return false;
            if (string.IsNullOrEmpty(Text))
                return true;

            return ItemGetTextFrom(value, FilterMemberPath)?.ToLower().Contains(Text.ToLower()) == true;
        }

        private string? ItemGetTextFrom(object item, string? propertyName)
        {
            if (item == null)
                return string.Empty;
            if (propertyName == null)
                return item.ToString();

            PropertyInfo? displayMemberProperty = item.GetType().GetProperty(propertyName);
            if (displayMemberProperty != null)
                return displayMemberProperty.GetValue(item)?.ToString();
            return item.ToString();
        }
    }
}
