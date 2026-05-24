using System;

namespace CyberSecurityBot
{
    public class ChatBot
    {
        private KeywordResponder _keywords;
        private SentimentDetector _sentiment;
        private MemoryStore _memory;
        private bool _awaitingName = true;
        private string _lastTopic = "";

        public ChatBot()
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
        }

        public string GetGreeting()
        {
            return "🛡️ Welcome to the Cybersecurity Awareness Assistant! 🛡️\n\nBefore we begin learning about online safety, what is your name?";
        }

        public string ProcessInput(string input) 
        {
            string cleanInput = input?.Trim() ?? "";
            if (string.IsNullOrEmpty(cleanInput))
            {
                return "I didn't quite catch that. Could you provide an input?";
            }

            if (_awaitingName)
            {
                _memory.Store("name", cleanInput);
                _awaitingName = false;
                return $"Nice to meet you, {_memory.UserName}! How can I help you stay safe online today?\n\nYou can ask me about 'passwords', 'phishing', 'privacy', 'scams', or 'browsing'.";
            }

            string lowerInput = cleanInput.ToLower();

            if (lowerInput == "exit" || lowerInput == "quit")
            {
                return "Stay secure out there! You can now close the window.";
            }
            if (lowerInput.Contains("how are you"))
            {
                return $"I'm running optimally, {_memory.UserName}, and fully ready to analyze cyber security topics!";
            }
            if (lowerInput.Contains("purpose") || lowerInput.Contains("what can i ask"))
            {
                return "My system is configured to educate you on threats. Ask me about passwords, phishing, or safety strategies.";
            }

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return _keywords.GetResponse(_lastTopic, out _);
                }
                return "We haven't started a technical topic yet. Which area would you like me to expand on?";
            }

            Sentiment detectedMood = _sentiment.Detect(cleanInput);
            string moodOpener = _sentiment.GetSentimentResponse(detectedMood);

            string botTip = _keywords.GetResponse(cleanInput, out string matchedKeyword);

            if (!string.IsNullOrEmpty(matchedKeyword))
            {
                _lastTopic = matchedKeyword;
                if (detectedMood == Sentiment.Curious || string.IsNullOrEmpty(_memory.FavouriteTopic))
                {
                    _memory.Store("topic", matchedKeyword);
                }

                return $"{moodOpener}{_memory.GetPersonalisedOpener()}{botTip}";
            }

            if (!string.IsNullOrEmpty(moodOpener))
            {
                return moodOpener + "Tell me a bit more specifically—are you looking into passwords, browsing safety, or phishing vulnerabilities?";
            }

            return $"I couldn't quite map that command, {_memory.UserName}. Could you try rephrasing it? Remember, you can query keywords like passwords, phishing, browsing, scams, or privacy.";
        }
    }
}