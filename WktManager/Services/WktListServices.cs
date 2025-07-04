//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using WktManager.DTOs;
//using WktManager.Models;
//using WktManager.Responses;
//using Npgsql;

//namespace WktManager.Services
//{
//    public class WktListServices : IWktCoordinateService
//    {
//        private readonly string _connectionString;

//        public WktListServices(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        private bool IsValidWKT(string wkt)
//        {
//            if (string.IsNullOrWhiteSpace(wkt)) return false;
//            var trimmed = wkt.Trim();

//            var pattern = @"^(POINT\s*\(\s*-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
//                          @"LINESTRING\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
//                          @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)|" +
//                          @"POLYGON\s*\(\s*\(\s*(-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*,\s*)*" +
//                          @"-?\d+(\.\d+)?\s+-?\d+(\.\d+)?\s*\)\s*\))$";

//            return Regex.IsMatch(trimmed, pattern, RegexOptions.IgnoreCase);
//        }

//        public Result Add(WktCoordinateDto dto)
//        {
//            if (!IsValidWKT(dto.WKT))
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Geçerli bir WKT formatı giriniz.",
//                    Data = null
//                };
//            }

//            int newId;
//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            string sql = "INSERT INTO WktCoordinates (Name, WKT) VALUES (@name, @wkt) RETURNING Id;";

//            using var cmd = new NpgsqlCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@name", dto.Name);
//            cmd.Parameters.AddWithValue("@wkt", dto.WKT);

//            try
//            {
//                newId = (int)cmd.ExecuteScalar();
//            }
//            catch (Exception ex)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = $"Kayıt eklenirken hata oluştu: {ex.Message}",
//                    Data = null
//                };
//            }

//            var entity = new WktCoordinate
//            {
//                Id = newId,
//                Name = dto.Name,
//                WKT = dto.WKT
//            };

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla eklendi.",
//                Data = entity
//            };
//        }

//        public Result AddRange(List<WktCoordinateDto> dtos)
//        {
//            foreach (var dto in dtos)
//            {
//                if (!IsValidWKT(dto.WKT))
//                {
//                    return new Result
//                    {
//                        Success = false,
//                        Message = $"Geçersiz WKT: {dto.Name}",
//                        Data = null
//                    };
//                }
//            }

//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            using var transaction = conn.BeginTransaction();

//            var insertedEntities = new List<WktCoordinate>();

//            try
//            {
//                foreach (var dto in dtos)
//                {
//                    string sql = "INSERT INTO WktCoordinates (Name, WKT) VALUES (@name, @wkt) RETURNING Id;";
//                    using var cmd = new NpgsqlCommand(sql, conn, transaction);
//                    cmd.Parameters.AddWithValue("@name", dto.Name);
//                    cmd.Parameters.AddWithValue("@wkt", dto.WKT);

//                    int newId = (int)cmd.ExecuteScalar();

//                    insertedEntities.Add(new WktCoordinate
//                    {
//                        Id = newId,
//                        Name = dto.Name,
//                        WKT = dto.WKT
//                    });
//                }

//                transaction.Commit();
//            }
//            catch (Exception ex)
//            {
//                transaction.Rollback();
//                return new Result
//                {
//                    Success = false,
//                    Message = $"Toplu kayıt eklenirken hata oluştu: {ex.Message}",
//                    Data = null
//                };
//            }

//            return new Result
//            {
//                Success = true,
//                Message = $"{insertedEntities.Count} kayıt başarıyla eklendi.",
//                Data = insertedEntities
//            };
//        }

//        public Result GetAll()
//        {
//            var list = new List<WktCoordinate>();

//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            string sql = "SELECT Id, Name, WKT FROM WktCoordinates ORDER BY Id;";

//            using var cmd = new NpgsqlCommand(sql, conn);

//            using var reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                list.Add(new WktCoordinate
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    WKT = reader.GetString(2)
//                });
//            }

//            return new Result
//            {
//                Success = true,
//                Message = $"{list.Count} kayıt bulundu.",
//                Data = list
//            };
//        }

//        public Result GetRange(int startIndex, int count)
//        {
//            if (startIndex < 0 || count < 0)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "startIndex veya count negatif olamaz.",
//                    Data = null
//                };
//            }

//            var list = new List<WktCoordinate>();

//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            // PostgreSQL'de OFFSET-FETCH NEXT için syntax:
//            string sql = "SELECT Id, Name, WKT FROM WktCoordinates ORDER BY Id OFFSET @start LIMIT @count;";

//            using var cmd = new NpgsqlCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@start", startIndex);
//            cmd.Parameters.AddWithValue("@count", count);

//            using var reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                list.Add(new WktCoordinate
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    WKT = reader.GetString(2)
//                });
//            }

//            return new Result
//            {
//                Success = true,
//                Message = $"{list.Count} kayıt getirildi.",
//                Data = list
//            };
//        }

//        public Result GetById(int id)
//        {
//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            string sql = "SELECT Id, Name, WKT FROM WktCoordinates WHERE Id = @id;";

//            using var cmd = new NpgsqlCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@id", id);

//            using var reader = cmd.ExecuteReader();

//            if (reader.Read())
//            {
//                var entity = new WktCoordinate
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    WKT = reader.GetString(2)
//                };

//                return new Result
//                {
//                    Success = true,
//                    Message = "Kayıt bulundu.",
//                    Data = entity
//                };
//            }
//            else
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Kayıt bulunamadı.",
//                    Data = null
//                };
//            }
//        }

//        public Result Update(int id, WktCoordinateDto dto)
//        {
//            if (!IsValidWKT(dto.WKT))
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Geçerli bir WKT formatı giriniz.",
//                    Data = null
//                };
//            }

//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            string sql = "UPDATE WktCoordinates SET Name = @name, WKT = @wkt WHERE Id = @id;";

//            using var cmd = new NpgsqlCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@name", dto.Name);
//            cmd.Parameters.AddWithValue("@wkt", dto.WKT);
//            cmd.Parameters.AddWithValue("@id", id);

//            int rowsAffected = cmd.ExecuteNonQuery();

//            if (rowsAffected == 0)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Güncellenecek kayıt bulunamadı.",
//                    Data = null
//                };
//            }

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla güncellendi.",
//                Data = null
//            };
//        }

//        public Result Delete(int id)
//        {
//            using var conn = new NpgsqlConnection(_connectionString);
//            conn.Open();

//            string sql = "DELETE FROM WktCoordinates WHERE Id = @id;";

//            using var cmd = new NpgsqlCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@id", id);

//            int rowsAffected = cmd.ExecuteNonQuery();

//            if (rowsAffected == 0)
//            {
//                return new Result
//                {
//                    Success = false,
//                    Message = "Silinecek kayıt bulunamadı.",
//                    Data = null
//                };
//            }

//            return new Result
//            {
//                Success = true,
//                Message = "Kayıt başarıyla silindi.",
//                Data = null
//            };
//        }
//    }
//}
