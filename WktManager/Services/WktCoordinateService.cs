using System.Text.RegularExpressions;
using WktManager.DTOs;
using WktManager.Models;
using WktManager.Responses;
using WktManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WktManager.Services
{
    public class WktCoordinateService : IWktCoordinateService
    {
        private readonly AppDbContext _context;

        public WktCoordinateService(AppDbContext context)
        {
            _context = context;
        }

        private bool IsValidWKT(string wkt)
        {
            if (string.IsNullOrWhiteSpace(wkt)) return false;
            var trimmed = wkt.Trim();

            var pattern = @"^(POINT\s*\(\s*-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
                          @"LINESTRING\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
                          @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
                          @"POLYGON\s*\(\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
                          @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)\s*\))$";

            return Regex.IsMatch(trimmed, pattern, RegexOptions.IgnoreCase);
        }

        public Result Add(WktCoordinateDto dto)
        {
            if (!IsValidWKT(dto.WKT))
            {
                return new Result
                {
                    Success = false,
                    Message = "Geçerli bir WKT formatı giriniz.",
                    Data = null
                };
            }
            try
            {
                var entity = new WktCoordinate
                {
                    Name = dto.Name,
                    WKT = dto.WKT
                };

                _context.WktCoordinates.Add(entity);
                _context.SaveChanges();

                return new Result
                {
                    Success = true,
                    Message = "Kayıt başarıyla eklendi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"kayıt oluşturulurken Hata oluştu:(( :{ex.Message}"

                };
            }
                
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
                        Message = $"Geçersiz WKT: {dto.Name}",
                        Data = null
                    };
                }
            }

            var entities = dtos.Select(dto => new WktCoordinate
            {
                Name = dto.Name,
                WKT = dto.WKT
            }).ToList();

            _context.WktCoordinates.AddRange(entities);
            _context.SaveChanges();

            return new Result
            {
                Success = true,
                Message = $"{entities.Count} kayıt başarıyla eklendi.",
                Data = entities
            };
        }

        public Result GetAll()
        {
            var list = _context.WktCoordinates.ToList();

            return new Result
            {
                Success = true,
                Message = $"{list.Count} kayıt bulundu.",
                Data = list
            };
        }

        public Result GetRange(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0)
            {
                return new Result
                {
                    Success = false,
                    Message = "startIndex veya count negatif olamaz.",
                    Data = null
                };
            }

            var totalCount = _context.WktCoordinates.Count();

            if (startIndex >= totalCount)
            {
                return new Result
                {
                    Success = false,
                    Message = "startIndex kayıt sayısından büyük olamaz.",
                    Data = null
                };
            }

            var range = _context.WktCoordinates
                                .Skip(startIndex)
                                .Take(count)
                                .ToList();

            return new Result
            {
                Success = true,
                Message = $"{range.Count} kayıt getirildi.",
                Data = range
            };
        }

        public Result GetById(int id)
        {
            var entity = _context.WktCoordinates.Find(id);
            if (entity == null)
            {
                return new Result
                {
                    Success = false,
                    Message = "Böyle bir Kayıt bulunamadı.",
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

        public Result Update(int id, WktCoordinateDto dto)
        {
            try
            {
                var existing = _context.WktCoordinates.Find(id);
                if (existing == null)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Güncellenecek kayıt bulunamadı.",
                        Data = null
                    };
                }

                if (!IsValidWKT(dto.WKT))
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Geçerli bir WKT formatı giriniz.",
                        Data = null
                    };
                }

                existing.Name = dto.Name;
                existing.WKT = dto.WKT;

                _context.SaveChanges();

                return new Result
                {
                    Success = true,
                    Message = "Kayıt başarıyla güncellendi.",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"Güncelleme sırasında hata oluştu: {ex.Message}"
                };
            }
            }

        public Result Delete(int id)
        {
            try
            {
                var existing = _context.WktCoordinates.Find(id);
                if (existing == null)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Silinecek kayıt bulunamadı.",
                        Data = null
                    };
                }

                _context.WktCoordinates.Remove(existing);
                _context.SaveChanges();

                return new Result
                {
                    Success = true,
                    Message = "Kayıt başarıyla silindi.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"Silme sırasında hata oluştu: {ex.Message}",
                    Data=null
                    
                };
            }
        }
    }
}










