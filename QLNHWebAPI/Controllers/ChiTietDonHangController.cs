using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietDonHangController : ControllerBase
    {
        private readonly QlnhContext _context;

        public ChiTietDonHangController(QlnhContext context)
        {
            _context = context;
        }

        // GET: api/ChiTietDonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietDonHang>>> GetChiTietDonHangs()
        {
            var chiTietDonHangs = await _context.ChiTietDonHangs.ToListAsync();
            return Ok(chiTietDonHangs);
        }

        // GET: api/ChiTietDonHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietDonHang>> GetChiTietDonHang(int id)
        {
            var chiTietDonHang = await _context.ChiTietDonHangs.FindAsync(id);

            if (chiTietDonHang == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng" });
            }

            return Ok(chiTietDonHang);
        }

        // POST: api/ChiTietDonHang
        [HttpPost]
        public async Task<ActionResult<ChiTietDonHang>> PostChiTietDonHang(ChiTietDonHang chiTietDonHang)
        {
            // Check if the provided data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ChiTietDonHangs.Add(chiTietDonHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChiTietDonHang), new { id = chiTietDonHang.ChiTietDonHangId }, chiTietDonHang);
        }

        // PUT: api/ChiTietDonHang/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChiTietDonHang(int id, ChiTietDonHang chiTietDonHang)
        {
            if (id != chiTietDonHang.ChiTietDonHangId)
            {
                return BadRequest("ID mismatch.");
            }

            // Find the existing order detail
            var existingChiTietDonHang = await _context.ChiTietDonHangs.FindAsync(id);
            if (existingChiTietDonHang == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng" });
            }

            // Update the fields
            existingChiTietDonHang.ChiTietMonAnId = chiTietDonHang.ChiTietMonAnId;
            existingChiTietDonHang.SoLuong = chiTietDonHang.SoLuong;
            existingChiTietDonHang.Gia = chiTietDonHang.Gia;
            existingChiTietDonHang.TrangThai = chiTietDonHang.TrangThai;
            existingChiTietDonHang.ThoiGianBatDau = chiTietDonHang.ThoiGianBatDau;
            existingChiTietDonHang.ThoiGianKetThuc = chiTietDonHang.ThoiGianKetThuc;

            _context.Entry(existingChiTietDonHang).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Chi tiết đơn hàng đã được cập nhật thành công!" });
        }

        // DELETE: api/ChiTietDonHang/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChiTietDonHang(int id)
        {
            var chiTietDonHang = await _context.ChiTietDonHangs.FindAsync(id);
            if (chiTietDonHang == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng" });
            }

            _context.ChiTietDonHangs.Remove(chiTietDonHang);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Chi tiết đơn hàng đã được xóa thành công!" });
        }
    }
}
