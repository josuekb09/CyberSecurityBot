using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }

    public class SentimentDetector
    {
        private Dictionary<Sentiment, List<string>> _triggers;

        public SentimentDetector()
        {
            _triggers = new Dictionary<Sentiment, List<string>>
            {
                { Sentiment.Worried, new List<string> { "worried", "scared", "afraid", "anxious", "nervous", "unsafe" } },
                { Sentiment.Curious, new List<string> { "curious", "wondering", "interested", "want to know", "how does" } },
                { Sentiment.Frustrated, new List<string> { "frustrated", "annoyed", "confused", "don't understand" } },
                { Sentiment.Happy, new List<string> { "great", "thanks", "helpful", "awesome", "love it" } }
            };
        }

        public Sentiment Detect(string input)
        {
            string cleanInput = input.ToLower();
            foreach (var pair in _triggers)
            {
                foreach (var word in pair.Value)
                {
                    if (cleanInput.Contains(word)) return pair.Key;
                }
            }
            return Sentiment.Neutral;
        }

        public string GetSentimentResponse(Sentiment s)
        {
            return s switch
            {
                Sentiment.Worried => "It is completely understandable to feel concerned about online threats. Let's make sure you have the tools to stay protected. ",
                Sentiment.Curious => "I love that you're looking to expand your digital literacy! Understanding security is the best way to prevent issues. ",
                Sentiment.Frustrated => "Cybersecurity concepts can feel overwhelming at first, but we can break it down together easily. ",
                Sentiment.Happy => "Fantastic! I'm glad to hear that. Let's keep building on your security knowledge! ",
                _ => ""
            };
        }
    }
}