using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Markdig.Wpf;

namespace CodingExercises
{
    public partial class MainWindow : Window
    {
        private string _filePath;
        private bool _isEditMode = true;

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
            FilePathTextBox.Text = "C:\\Users\\morge\\OneDrive\\Obsidian\\Obsidian Vault\\Interview\\Progress\\Tasks Delegates.md";
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            _filePath = FilePathTextBox.Text;
            if (File.Exists(_filePath))
            {
                MarkdownEditor.Text = File.ReadAllText(_filePath);
            }
            else
            {
                MessageBox.Show("File not found.");
            }
        }

        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                SaveMarkdown();
                MarkdownViewer.Markdown = MarkdownEditor.Text;
            }
            _isEditMode = !_isEditMode;
            UpdateUI();
        }

        private void SaveMarkdown()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                File.WriteAllText(_filePath, MarkdownEditor.Text);
            }
        }

        private void UpdateUI()
        {
            if (_isEditMode)
            {
                MarkdownEditor.Visibility = Visibility.Visible;
                MarkdownViewerContainer.Visibility = Visibility.Collapsed;
                ToggleModeButton.Content = "Preview";
            }
            else
            {
                MarkdownEditor.Visibility = Visibility.Collapsed;
                MarkdownViewerContainer.Visibility = Visibility.Visible;
                ToggleModeButton.Content = "Edit";
            }
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_isEditMode)
            {
                var scrollViewer = MarkdownViewerContainer;
                if (scrollViewer != null)
                {
                    if (e.Delta < 0)
                    {
                        scrollViewer.LineDown();
                    }
                    else
                    {
                        scrollViewer.LineUp();
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
