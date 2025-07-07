using System.Text.RegularExpressions;
using WktManager.DTOs;
using WktManager.Models;
using WktManager.Repositories;
using WktManager.Responses;


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

    public Result GetAll()
    {
        var list = _unitOfWork.Repository<WktCoordinate>().GetAll();

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
        {
            return new Result
            {
                Success = false,
                Message = "Böyle bir kayıt bulunamadı.",
                Data = null
            };
        }

        return new Result
        {
            Success = true,
            Message = "Kayıt bulundu.",
            Data = entity
        };
    }

    public Result GetRange(int startIndex, int count)
    {
        if (startIndex < 0 || count < 0)
        {
            return new Result
            {
                Success = false,
                Message = "startIndex veya count negatif olamaz."
            };
        }

        var all = _unitOfWork.Repository<WktCoordinate>().GetAll();

        if (startIndex >= all.Count)
        {
            return new Result
            {
                Success = false,
                Message = "startIndex kayıt sayısından büyük olamaz."
            };
        }

        var range = all.Skip(startIndex).Take(count).ToList();

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
        {
            return new Result
            {
                Success = false,
                Message = "Geçerli bir WKT formatı giriniz."
            };
        }

        var entity = new WktCoordinate
        {
            Name = dto.Name,
            WKT = dto.WKT
        };

        _unitOfWork.Repository<WktCoordinate>().Add(entity);
        _unitOfWork.Commit();

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla eklendi.",
            Data = entity
        };
    }

    public Result AddRange(List<WktCoordinateDto> dtos)
    {
        foreach (var dto in dtos)
        {
            if (!IsValidWKT(dto.WKT))
            {
                return new Result
                {
                    Success = false,
                    Message = $"Geçersiz WKT: {dto.Name}"
                };
            }
        }

        var entities = dtos.Select(dto => new WktCoordinate
        {
            Name = dto.Name,
            WKT = dto.WKT
        }).ToList();

        _unitOfWork.Repository<WktCoordinate>().AddRange(entities);
        _unitOfWork.Commit();

        return new Result
        {
            Success = true,
            Message = $"{entities.Count} kayıt başarıyla eklendi.",
            Data = entities
        };
    }

    public Result Update(int id, WktCoordinateDto dto)
    {
        var repository = _unitOfWork.Repository<WktCoordinate>();

        var existing = repository.GetById(id);
        if (existing == null)
        {
            return new Result
            {
                Success = false,
                Message = "Güncellenecek kayıt bulunamadı."
            };
        }

        if (!IsValidWKT(dto.WKT))
        {
            return new Result
            {
                Success = false,
                Message = "Geçerli bir WKT formatı giriniz."
            };
        }

        existing.Name = dto.Name;
        existing.WKT = dto.WKT;

        repository.Update(existing);
        _unitOfWork.Commit();

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla güncellendi.",
            Data = existing
        };
    }

    public Result Delete(int id)
    {
        var repository = _unitOfWork.Repository<WktCoordinate>();

        var existing = repository.GetById(id);
        if (existing == null)
        {
            return new Result
            {
                Success = false,
                Message = "Silinecek kayıt bulunamadı."
            };
        }

        repository.Delete(existing);
        _unitOfWork.Commit();

        return new Result
        {
            Success = true,
            Message = "Kayıt başarıyla silindi."
        };
    }
}

//public class WktCoordinateService : IWktCoordinateService
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IRepository<WktCoordinate> _repository;

//    public WktCoordinateService(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;
//        _repository = _unitOfWork.Repository<WktCoordinate>();
//    }

//    private bool IsValidWKT(string wkt)
//    {
//        if (string.IsNullOrWhiteSpace(wkt)) return false;
//        var trimmed = wkt.Trim();

//        var pattern = @"^(POINT\s*\(\s*-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
//                      @"LINESTRING\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
//                      @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
//                      @"POLYGON\s*\(\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
//                      @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)\s*\))$";

//        return Regex.IsMatch(trimmed, pattern, RegexOptions.IgnoreCase);
//    }

//    public Result GetAll()
//    {
//        var list = _repository.GetAll().ToList();

//        return new Result
//        {
//            Success = true,
//            Message = $"{list.Count} kayıt bulundu.",
//            Data = list
//        };
//    }

//    public Result GetById(int id)
//    {
//        var entity = _repository.GetById(id);
//        if (entity == null)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = "Böyle bir kayıt bulunamadı.",
//                Data = null
//            };
//        }

//        return new Result
//        {
//            Success = true,
//            Message = "Kayıt bulundu.",
//            Data = entity
//        };
//    }

//    public Result GetRange(int startIndex, int count)
//    {
//        if (startIndex < 0 || count < 0)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = "startIndex veya count negatif olamaz.",
//                Data = null
//            };
//        }

//        var all = _repository.GetAll();
//        var totalCount = all.Count;

//        if (startIndex >= totalCount)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = "startIndex kayıt sayısından büyük olamaz.",
//                Data = null
//            };
//        }

//        var range = all.Skip(startIndex).Take(count).ToList();

//        return new Result
//        {
//            Success = true,
//            Message = $"{range.Count} kayıt getirildi.",
//            Data = range
//        };
//    }

//    public Result Add(WktCoordinateDto dto)
//    {
//        if (!IsValidWKT(dto.WKT))
//        {
//            return new Result
//            {
//                Success = false,
//                Message = "Geçerli bir WKT formatı giriniz.",
//                Data = null
//            };
//        }

//        try
//        {
//            var entity = new WktCoordinate
//            {
//                Name = dto.Name,
//                WKT = dto.WKT
//            };

//            _repository.Add(entity);
//            _unitOfWork.Commit();

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla eklendi.",
//                Data = entity
//            };
//        }
//        catch (Exception ex)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = $"Kayıt oluşturulurken hata oluştu: {ex.Message}"
//            };
//        }
//    }

//    public Result AddRange(List<WktCoordinateDto> dtos)
//    {
//        foreach (var dto in dtos)
//        {
//            if (!IsValidWKT(dto.WKT))
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = $"Geçersiz WKT: {dto.Name}",
//                    Data = null
//                };
//            }
//        }

//        var entities = dtos.Select(dto => new WktCoordinate
//        {
//            Name = dto.Name,
//            WKT = dto.WKT
//        }).ToList();

//        _repository.AddRange(entities);
//        _unitOfWork.Commit();

//        return new Result
//        {
//            Success = true,
//            Message = $"{entities.Count} kayıt başarıyla eklendi.",
//            Data = entities
//        };
//    }

//    public Result Update(int id, WktCoordinateDto dto)
//    {
//        try
//        {
//            var existing = _repository.GetById(id);
//            if (existing == null)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Güncellenecek kayıt bulunamadı.",
//                    Data = null
//                };
//            }

//            if (!IsValidWKT(dto.WKT))
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Geçerli bir WKT formatı giriniz.",
//                    Data = null
//                };
//            }

//            existing.Name = dto.Name;
//            existing.WKT = dto.WKT;

//            _repository.Update(existing);
//            _unitOfWork.Commit();

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla güncellendi.",
//                Data = existing
//            };
//        }
//        catch (Exception ex)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = $"Güncelleme sırasında hata oluştu: {ex.Message}"
//            };
//        }
//    }

//    public Result Delete(int id)
//    {
//        try
//        {
//            var existing = _repository.GetById(id);
//            if (existing == null)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Silinecek kayıt bulunamadı.",
//                    Data = null
//                };
//            }

//            _repository.Delete(existing);
//            _unitOfWork.Commit();

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla silindi.",
//                Data = null
//            };
//        }
//        catch (Exception ex)
//        {
//            return new Result
//            {
//                Success = false,
//                Message = $"Silme sırasında hata oluştu: {ex.Message}"
//            };
//        }
//    }
//}
