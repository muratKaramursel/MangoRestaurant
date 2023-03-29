namespace EmailProcessor
{
    public interface IEmailManager
    {
        public bool SendEmail(string title, string body, List<string> toList, List<string> ccList, List<string> bccList);
    }
}