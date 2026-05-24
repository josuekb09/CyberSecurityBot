using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace CyberSecurityBot
{
    public partial class MainWindow : Window
    {
        private ChatBot _chatBot;

        public MainWindow()
        {
            InitializeComponent();
            _chatBot = new ChatBot();
            PlayVoiceGreeting();

            ChatDisplay.Text += $"[SYSTEM BOT]: {_chatBot.GetGreeting()}\n\n";
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                if (File.Exists("greeting.wav"))
                {
                    SoundPlayer player = new SoundPlayer("greeting.wav");
                    player.Play();
                }
            }
            catch { }
        }

        private void SendMessage()
        {
            string cleanText = TxtInput.Text;
            if (string.IsNullOrWhiteSpace(cleanText)) return;

            ChatDisplay.Text += $"> User: {cleanText}\n\n";
            TxtInput.Clear();

            string systemOutput = _chatBot.ProcessInput(cleanText);
            ChatDisplay.Text += $"[SYSTEM BOT]: {systemOutput}\n\n";

            ChatScroll.ScrollToBottom();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}