using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class KeywordResponder
    {
        private Dictionary<string, List<string>> _responses;
        private Random _random = new Random();

        public KeywordResponder()
        {
            _responses = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    "Password Safety Deep-Dive: Use strings of at least 12-16 characters combining case variations, symbols, and digits. Never reuse keys across distinct profiles; consider using an encrypted utility like Bitwarden.",
                    "Authentication Best Practice: Passphrases made of 4 or more random combined words (e.g., 'CorrectHorseBatteryStaple') are harder for brute-force systems to guess than short complex characters."
                }},
                { "phishing", new List<string> {
                    "Phishing Awareness: Look closely for typosquatting variations in sender addresses (like secure-bank-login.com vs bank.com). Legitimate brands will never ask for credentials via email.",
                    "Email Defense: Always hover hyperlinks to verify their destination domain before clicking. If an alert claims urgency or critical suspension, verify it directly on the provider's official dashboard instead."
                }},
                { "browsing", new List<string> {
                    "Safe Web Navigation: Ensure the presence of HTTPS protocol extensions. Avoid completing online transactions or inputting financial data while connected to insecure public Wi-Fi systems without a VPN.",
                    "Browser Isolation: Keep extensions and browser engines completely up to date to patch zero-day runtime exploits. Utilize reliable script blockers to limit background execution threats."
                }},
                { "scam", new List<string> {
                    "Scam Recognition: If an offer sounds too beneficial to be true, it is almost certainly a trap. Watch out for social engineering triggers demanding upfront payment or crypto transfers.",
                    "Digital Fraud Prevention: Be skeptical of unexpected callers pretending to be tech support. Real engineers never cold-call users demanding remote control tools like AnyDesk to fix errors."
                }},
                { "privacy", new List<string> {
                    "Privacy Frameworks: Audit app permissions on mobile devices regularly. Disable background location access, limit telemetry sharing, and switch search engines to non-tracking alternatives.",
                    "Data Exposure Isolation: Use email aliasing systems to sign up for public forums. This isolates your main identity and stops corporate tracking profiles from connecting your online footprints."
                }}
            };
        }

        public string GetResponse(string input, out string matchedKeyword)
        {
            string cleanInput = input.ToLower();
            foreach (var keyword in _responses.Keys)
            {
                if (cleanInput.Contains(keyword))
                {
                    matchedKeyword = keyword;
                    int index = _random.Next(_responses[keyword].Count);
                    return _responses[keyword][index];
                }
            }
            matchedKeyword = "";
            return "";
        }
    }
}