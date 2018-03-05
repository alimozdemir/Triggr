using System.ComponentModel.DataAnnotations;

namespace Triggr.UI.Models
{
    public class AddRepositoryViewModel
    {
        [Required]
        public string Url { get; set; }
        public string Token { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
    }
}