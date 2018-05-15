using GolfApp.Algorithm;
using GolfApp.Input;
using GolfApp.Output;
using GolfApp.Structures;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GolfUI
{
    public partial class MainWindow : Window
    {
        private State State { get; } = new State();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = State;
        }

        private void LoadTask(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new OpenFileDialog { DefaultExt = ".txt" };
            if (dialogWindow.ShowDialog() == true)
            {
                var filename = dialogWindow.FileName;
                var taskParser = new TextFileTaskParser(filename);

                try
                {
                    State.LoadTask(taskParser.Parse());
                }
                catch (Exception)
                {
                    MessageBox.Show("Error while reading file.");
                }
            }
        }

        private void SaveTask(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new SaveFileDialog { DefaultExt = ".txt" };
            if (dialogWindow.ShowDialog() == true)
            {
                var filename = dialogWindow.FileName;
                var taskParser = new TextFileTaskSaver(filename);
                try
                {
                    taskParser.Save(State.Task);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error while saving file.");
                }
            }
        }

        private void FindMatching(object sender, RoutedEventArgs e)
        {
            State.FindPlanarMatching();
        }

        private void SaveMatching(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new SaveFileDialog { DefaultExt = ".txt" };
            if (dialogWindow.ShowDialog() == true)
            {
                var filename = dialogWindow.FileName;
                var matchingPrinter = new TextFileMatchingPrinter(filename);

                try
                {
                    matchingPrinter.Print(State.Matching);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error while saving file.");
                }
            }
        }

        private void ClearMatching(object sender, RoutedEventArgs e)
        {
            State.ClearMatching();
        }

    }
}
