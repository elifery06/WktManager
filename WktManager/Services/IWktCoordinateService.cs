using System.Collections.Generic;
using WktManager.DTOs;
using WktManager.Models;
using WktManager.Responses;

namespace WktManager.Services
{
    public interface IWktCoordinateService
    {
        Result GetAll();
        Result GetRange(int startIndex, int count);
        Result GetById(int id);
        Result Add(WktCoordinateDto dto);
        Result AddRange(List<WktCoordinateDto> dtos);
        Result Update(int id, WktCoordinateDto dto);
        Result Delete(int id);
    }
}
