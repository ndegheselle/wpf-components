﻿using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using  WpfComponents.Lib.Components.Filters;

namespace WpfComponents.App
{
    public enum EnumTest
    {
        None,
        One,
        Two,
    }

    public class TestClass
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public bool IsTest { get; set; }

        public EnumTest EnumTest { get; set; }

        public TestClass(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() { return $"{Name} : {Value}"; }

        public override bool Equals(object? obj) { return obj is TestClass value && Name == value.Name; }

        public override int GetHashCode() { return Name.GetHashCode(); }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow, INotifyPropertyChanged
    {
        public List<TestClass> TestValues
        {
            // Create a new instance because of CollectionViewSource.GetDefaultView
            get => new List<TestClass>
                {
                    new TestClass("One", 1),
                    new TestClass("Two", 2),
                    new TestClass("Three", 3),
                    new TestClass("Four", 4),
                    new TestClass("Five", 5),
                    new TestClass("Six", 6),
                    new TestClass("Seven", 7),
                    new TestClass("Eight", 8),
                    new TestClass("Nine", 9),
                    new TestClass("Ten", 10),
                };
        }

        public ObservableCollection<TestClass> SelectedTestValues
        {
            get;
            set;
        } = new ObservableCollection<TestClass>
            {
                new TestClass("One", 1),
                new TestClass("Two", 2),
                new TestClass("Three", 3),
            };

        public MainWindow()
        {
            InitializeComponent();
            // Foreach tab in MainContainer create a ListBoxItem and add it to the ListBox
            foreach (TabItem tab in MainContainer.Items)
            {
                var item = new ListBoxItem { Content = tab.Header, Tag = tab };
                item.Selected += (s, e) =>
                {
                    MainContainer.SelectedItem = (TabItem)((ListBoxItem)s).Tag;
                };
                SideMenu.Items.Add(item);
            }

            FileExplorerControl.RootPaths = new List<string>
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                };
        }

        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Filters.Filter<TestClass>(DatagridFiltred);
        }

        private void PhoneNumberInput_ValueChanged(object sender, List<object?> values)
        { Debug.WriteLine($"PhoneNumberInput.ValueChanged: {string.Join(',', values)}"); }

        private void NumericUpDownInput_ValueChanged(object sender, int e)
        {
            Debug.WriteLine($"NumerUpDown.ValueChanged: {e}");
        }

        private void DecimalUpDownInput_ValueChanged(object sender, double e)
        {
            Debug.WriteLine($"DecimalUpDown.ValueChanged: {e}");
        }

        private void TimePickerInput_ValueChanged(object sender, TimeSpan? e)
        {
            Debug.WriteLine($"TimePicker.ValueChanged: {e}");
        }
    }
}
