using WktManager.DTOs;
using WktManager.Responses;
using System.Collections.Generic;

public interface IWktCoordinateService
{
    Result GetAll();
    Result GetPaged(int page, int pageSize);
    Result GetById(int id);
    Result GetRange(int startIndex, int count);
    Result Add(WktCoordinateDto dto);
    Result AddRange(List<WktCoordinateDto> dtos);
    Result Update(int id, WktCoordinateDto dto);
    Result Delete(int id);
}
