namespace EmailProcessor
{
    public class EmailManager : IEmailManager
    {
        public bool SendEmail(string title, string body, List<string> toList, List<string> ccList, List<string> bccList)
        {
            return true;
        }
    }
}