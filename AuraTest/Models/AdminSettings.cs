using System.ComponentModel.DataAnnotations;

namespace AuraTest.Models
{
    public class AdminSettings
    {
        [Key]
        public int Id { get; set; }
        public string FirstFilter { get; set; }
        public string SecoundFilter { get; set; }
        public string ThirdFilter { get; set; }
        public string ForthFilter { get; set; }
        
    }
}
