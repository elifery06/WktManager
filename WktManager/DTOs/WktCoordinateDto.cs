using System.ComponentModel.DataAnnotations;

namespace WktManager.DTOs
{
    public class WktCoordinateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name alanı zorunludur.")]
        public string Name { get; set; }
        public string WKT { get; set; }
    }
}
