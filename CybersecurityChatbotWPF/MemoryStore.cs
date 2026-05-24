namespace CyberSecurityBot
{
    public class MemoryStore
    {
        public string UserName { get; set; } = "";
        public string FavouriteTopic { get; set; } = "";

        public void Store(string key, string value)
        {
            if (key.ToLower() == "name") UserName = value;
            if (key.ToLower() == "topic") FavouriteTopic = value;
        }

        public string Recall(string key)
        {
            if (key.ToLower() == "name") return UserName;
            if (key.ToLower() == "topic") return FavouriteTopic;
            return "";
        }

        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrEmpty(FavouriteTopic)) 
            {
                return $"As someone interested in {FavouriteTopic}, you should know: ";
            }
            return "";
        }
    }
}