using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;

namespace WktManager.Models
{
    public class WktCoordinate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry WKT { get; set; }
      

    }
}
