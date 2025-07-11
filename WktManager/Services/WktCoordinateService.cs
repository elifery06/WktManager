using System.Text.RegularExpressions;
using WktManager.DTOs;
using WktManager.Models;
using WktManager.Repositories;
using WktManager.Responses;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;

public class WktCoordinateService : IWktCoordinateService
{
    private readonly IUnitOfWork _unitOfWork;

    public WktCoordinateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private bool IsValidWKT(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return false;

        var pattern = @"^(POINT\s*\(\s*-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
                      @"LINESTRING\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
                      @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
                      @"POLYGON\s*\(\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
                      @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)\s*\))$";

        return Regex.IsMatch(wkt.Trim(), pattern, RegexOptions.IgnoreCase);
    }

    private Geometry ToGeometry(string wkt)
    {
        return new WKTReader().Read(wkt);
    }

    public Result GetAll()
    {
        var list = _unitOfWork.Repository<WktCoordinate>().GetAll()
            .Select(e => new WktCoordinateDto
            {
                Id=e.Id,
                Name = e.Name,
                WKT = e.WKT.AsText()
            })
            .ToList();

        return new Result
        {
            Success = true,
            Message = $"{list.Count} kayıt bulundu.",
            Data = list
        };
    }

    public Result GetById(int id)
    {
        var entity = _unitOfWork.Repository<WktCoordinate>().GetById(id);

        if (entity == null)
            return new Result
            {
                Success = false,
                Message = "Böyle bir kayıt bulunamadı."
            };

        var dto = new WktCoordinateDto
        {Id= entity.Id,
            
            Name = entity.Name,
            WKT = entity.WKT.AsText()
        };

        return new Result
        {
            Success = true,
            Message = "Kayıt bulundu.",
            Data = dto
        };
    }
    public Result GetPaged(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _unitOfWork.Repository<WktCoordinate>().GetAll();

        var total = query.Count();

        var pagedData = query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new WktCoordinateDto
            {
                Id = e.Id,
                Name = e.Name,
                WKT = e.WKT.AsText()
            })
            .ToList();

        var resultData = new
        {
            Total = total,
            Data = pagedData
        };

        return new Result
        {
            Success = true,
            Message = $"{pagedData.Count} kayıt getirildi.",
            Data = resultData
        };
    }

    public Result GetRange(int startIndex, int count)
    {
        if (startIndex < 0 || count < 0)
            return new Result
            {
                Success = false,
                Message = "startIndex veya count negatif olamaz."
            };

        var all = _unitOfWork.Repository<WktCoordinate>().GetAll().ToList();

        if (startIndex >= all.Count)
            return new Result
            {
                Success = false,
                Message = "startIndex kayıt sayısından büyük olamaz."
            };

        var range = all.Skip(startIndex).Take(count)
            .Select(e => new WktCoordinateDto
            {
                Id= e.Id,
                Name = e.Name,
                WKT = e.WKT.AsText()
            })
            .ToList();

        return new Result
        {
            Success = true,
            Message = $"{range.Count} kayıt getirildi.",
            Data = range
        };
    }

    public Result Add(WktCoordinateDto dto)
    {
        if (!IsValidWKT(dto.WKT))
            return new Result
            {
                Success = false,
                Message = "Geçerli bir WKT formatı giriniz."
            };

        var entity = new WktCoordinate
        {
            Id= dto.Id,
            Name = dto.Name,
            WKT = ToGeometry(dto.WKT)
        };

        _unitOfWork.Repository<WktCoordinate>().Add(entity);
        _unitOfWork.Commit();

        var resultDto = new WktCoordinateDto
        {
            Id= entity.Id,
            Name = entity.Name,
            WKT = entity.WKT.AsText()
        };

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla eklendi.",
            Data = resultDto
        };
    }

    public Result AddRange(List<WktCoordinateDto> dtos)
    {
        foreach (var dto in dtos)
        {
            if (!IsValidWKT(dto.WKT))
                return new Result
                {
                    Success = false,
                    Message = $"Geçersiz WKT: {dto.Name}"
                };
        }

        var entities = dtos.Select(dto => new WktCoordinate
        {
            Id= dto.Id,
            Name = dto.Name,
            WKT = ToGeometry(dto.WKT)
        }).ToList();

        _unitOfWork.Repository<WktCoordinate>().AddRange(entities);
        _unitOfWork.Commit();

        var resultDtos = entities.Select(e => new WktCoordinateDto
        {
            Id= e.Id,
            Name = e.Name,
            WKT = e.WKT.AsText()
        }).ToList();

        return new Result
        {
            Success = true,
            Message = $"{entities.Count} kayıt başarıyla eklendi.",
            Data = resultDtos
        };
    }

    public Result Update(int id, WktCoordinateDto dto)
    {
        var repository = _unitOfWork.Repository<WktCoordinate>();
        var existing = repository.GetById(id);

        if (existing == null)
            return new Result
            {
                Success = false,
                Message = "Güncellenecek kayıt bulunamadı."
            };

        if (!IsValidWKT(dto.WKT))
            return new Result
            {
                Success = false,
                Message = "Geçerli bir WKT formatı giriniz."
            };

        existing.Name = dto.Name;
        existing.WKT = ToGeometry(dto.WKT);

        repository.Update(existing);
        _unitOfWork.Commit();

        var updatedDto = new WktCoordinateDto
        {
            Id=existing.Id,
            Name = existing.Name,
            WKT = existing.WKT.AsText()
        };

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla güncellendi.",
            Data = updatedDto
        };
    }

    public Result Delete(int id)
    {
        var repository = _unitOfWork.Repository<WktCoordinate>();
        var existing = repository.GetById(id);

        if (existing == null)
            return new Result
            {
                Success = false,
                Message = "Silinecek kayıt bulunamadı."
            };

        repository.Delete(existing);
        _unitOfWork.Commit();

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla silindi."
        };
    }
}
