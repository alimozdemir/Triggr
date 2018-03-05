using System.ComponentModel.DataAnnotations;

namespace Triggr.UI.Models
{
    public class IdFormViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class IdStringFormViewModel
    {
        [Required]
        public string Id { get; set; }
    }
}