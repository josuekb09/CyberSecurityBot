using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityBot
{
    public class ActivityLog
    {
        private readonly List<string> _actions = new List<string>();

        public void Add(string action)
        {
            string entry = $"{DateTime.Now:HH:mm:ss} - {action}";
            _actions.Insert(0, entry);
        }

        public List<string> GetRecent(int count = 10)
        {
            return _actions.Take(count).ToList();
        }
    }
}