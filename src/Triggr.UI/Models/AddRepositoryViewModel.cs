using System.ComponentModel.DataAnnotations;

namespace Triggr.UI.Models
{
    public class AddRepositoryViewModel
    {
        [Required]
        public string Url { get; set; }
    }
}