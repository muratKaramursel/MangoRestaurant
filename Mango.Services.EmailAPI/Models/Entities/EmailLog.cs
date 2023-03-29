using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models.Entities
{
    public class EmailLog
    {
        #region Properties
        [Key]
        public int EmailId { get; set; }
        public string EmailTitle { get; set; }
        public string EmailBody { get; set; }
        public string EmailAddress { get; set; }
        public DateTime EmailSent { get; set; }
        public bool Sended { get; set; }
        #endregion Properties
    }
}