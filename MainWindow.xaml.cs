using System;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Markdig.Wpf;

namespace CodingExercises
{
    public partial class MainWindow : Window
    {
        private string _filePath;
        private bool _isEditMode = true;
        private readonly string _userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebView2Cache");

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView2();
            UpdateUI();
            FilePathTextBox.Text = "C:\\Users\\morge\\OneDrive\\Obsidian\\Obsidian Vault\\Interview\\Progress\\Tasks Delegates.md";
        }

        private async void InitializeWebView2()
        {
            var environment = await CoreWebView2Environment.CreateAsync(null, _userDataFolder);
            await WebViewFiddle.EnsureCoreWebView2Async(environment);
            await WebViewChat.EnsureCoreWebView2Async(environment);

            WebViewFiddle.Source = new Uri("https://dotnetfiddle.net/");
            WebViewChat.Source = new Uri("https://chatgpt.com/?temporary-chat=true");
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            _filePath = FilePathTextBox.Text;
            if (File.Exists(_filePath))
            {
                MarkdownEditor.Text = File.ReadAllText(_filePath);
                _isEditMode = false; // Automatically switch to preview mode
                MarkdownViewer.Markdown = MarkdownEditor.Text;
                UpdateUI();
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
                MarkdownViewer.Visibility = Visibility.Collapsed;
                ToggleModeButton.Content = "📖";
            }
            else
            {
                MarkdownEditor.Visibility = Visibility.Collapsed;
                MarkdownViewer.Visibility = Visibility.Visible;
                ToggleModeButton.Content = "✏️";
            }
        }

        private void CheckAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            string taskText = ParsedTaskTextbox.Text;
            string answerText = ParsedAnswerTextbox.Text;

            string combinedText = $"I was doing coding exercise check it\r\nTask: {taskText}\nSolution: {answerText}";

            WebViewChat.ExecuteScriptAsync($@"
                var textarea = document.getElementById('prompt-textarea');
                textarea.value = `{combinedText}`;
                textarea.dispatchEvent(new Event('input', {{ bubbles: true }}));
                var event = new KeyboardEvent('keydown', {{ key: ' ' }});
                textarea.dispatchEvent(event);
                setTimeout(() => {{
                    document.querySelector('button[data-testid=""fruitjuice-send-button""]').click();
                }}, 100);
            ");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the task and solution text boxes
            ParsedTaskTextbox.Text = string.Empty;
            ParsedAnswerTextbox.Text = string.Empty;

            // Reload the chat web view
            WebViewChat.CoreWebView2.Navigate("https://chatgpt.com/?temporary-chat=true");
        }
    }
}
