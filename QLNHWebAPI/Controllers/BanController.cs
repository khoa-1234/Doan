using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BanController:ControllerCustome
    {
        private readonly QlnhContext _context;
  
        // private readonly IResponseService _responseService;
        public BanController(QlnhContext context) //IResponseService responseService)
        {
            _context = context;
   
            //_responseService = responseService;
        }
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> Get()
        {
            var bans = _context.Bans.ToList();
            return await ReturnMessagesucces(bans);
        }
        [HttpPost]
        public async Task<ActionResult<ResponeMessage>> ThemBan( BanModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ban = new Ban
            {
                SoBan = model.SoBan,
                SoGhe = model.SoGhe,
                KhuVucId = model.KhuVucId,
                ThoiGianTao = DateTime.Now
            };

            _context.Bans.Add(ban);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Nhân viên đã được thêm thành công", Data = ban });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> SuaBan(int id, BanModelView ban)
        {
            // Kiểm tra nếu id không khớp với NhanVienId trong body của yêu cầu
            if (id != ban.BanId)
            {
                return BadRequest("ID không khớp với Ban.");
            }

            var existingBan = await _context.Bans.FindAsync(id);
            if (existingBan == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            // Cập nhật các thuộc tính của existingNhanVien từ nhanVien
            existingBan.SoBan = ban.SoBan;
            existingBan.KhuVucId = ban.KhuVucId;
            existingBan.SoGhe = ban.SoGhe;
            existingBan.ThoiGianCapNhat = DateTime.Now;

            // Cập nhật các thuộc tính khác theo nhu cầu của bạn...

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BanExist(id))
                {
                    return NotFound("Ban không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { success = true, message = "Cập nhật bàn thành công." });
        }

        private bool BanExist(int id)
        {
            return _context.Bans.Any(e => e.BanId == id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponeMessage>> DeleteBan(int id)
        {
            var ban = await _context.Bans.FindAsync(id);
            if (ban == null)
            {
                return NotFound();
            }

            _context.Bans.Remove(ban);
            await _context.SaveChangesAsync();

            return await ReturnMessagesucces(ban);
        }

        //BanNvpv bolocngaythang nam
        [HttpGet("timkiembantheongaythangnam")]
        public async Task<ActionResult<ResponeMessage>> GetBanStatus([FromQuery] DateTime ngay)
        {
            if (ngay == default)
            {
                return BadRequest("Ngày không hợp lệ.");
            }

            var result = new List<BanInfo>();

            // Tạo kết nối đến database và gọi stored procedure
            var connection = _context.Database.GetDbConnection();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_LayTrangThaiBanTheoNgay";
                command.CommandType = CommandType.StoredProcedure;

                // Thêm tham số
                var parameter = new SqlParameter("@Ngay", ngay);
                command.Parameters.Add(parameter);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var banInfo = new BanInfo
                        {
                            BanId = reader.GetInt32(0),
                            SoBan = reader.GetInt32(1),
                            SoGhe = reader.GetInt32(2),
                            KhuVucId = reader.GetInt32(3),
                            TenKhuVuc = reader.GetString(4),
                            TrangThai = reader.GetString(5),
                            NgayGioDat = reader.IsDBNull(6) ? "Chưa có" : reader.GetString(6),
                            KhachHangId = reader.IsDBNull(7) ? "Không có" : reader.GetString(7),
                            DatBanId = reader.IsDBNull(8) ? "Không có" : reader.GetString(8),
                            DonHangId = reader.IsDBNull(9) ? "Không có" : reader.GetString(9),
                        };
                        result.Add(banInfo);
                    }
                }
            }

            return Ok(result);

        }

    }
}
