using System.ComponentModel.DataAnnotations;

namespace WktManager.DTOs
{
    public class WktCoordinateDto
    {
        
        [Required(ErrorMessage = "Name alanı zorunludur.")]
        public string Name { get; set; }
        public string WKT { get; set; }
    }
}
