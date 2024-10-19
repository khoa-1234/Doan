using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KhachHangsController : ControllerCustome
    {
        private readonly QlnhContext _context;

        public KhachHangsController(QlnhContext context)
        {
            _context = context;
        }

        // GET: api/NhanViens
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetKhachHang()
        {
            var khachHangs =  await _context.KhachHangs.ToListAsync();
            return await ReturnMessagesucces(khachHangs) ;
        }

        // GET: api/NhanViens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponeMessage>> GetKhachHangid(int id)
        {
            var khachHangs = await _context.KhachHangs.FindAsync(id);

            if (khachHangs == null)
            {
                return NotFound();
            }

            return await ReturnMessagesucces(khachHangs);
        }
        // GET: 
        [HttpGet("Laykhachhangbangsodienthoai")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponeMessage>> GetKhachHangsdt(string sodienthoai)
        {
            // Tìm kiếm khách hàng dựa trên số điện thoại
            var khachHang = await _context.KhachHangs
                                          .FirstOrDefaultAsync(kh => kh.SoDienThoai == sodienthoai);

            if (khachHang == null)
            {
                // Trả về mã 404 nếu không tìm thấy khách hàng
                return NotFound(new { message = "Không tìm thấy khách hàng với số điện thoại này." });
            }


            return await ReturnMessagesucces(khachHang);
        }

        // PUT: api/NhanViens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKhachHang(int id, KhachHangModelView khachHang)
        {
            if (id != khachHang.KhachHangId)
            {
                return BadRequest();
            }

            var existingKhachHang = await _context.KhachHangs.FindAsync(id);
            if (existingKhachHang == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính của existingNhanVien từ nhanVien
            existingKhachHang.HoTen = khachHang.HoTen;
            existingKhachHang.SoDienThoai = khachHang.SoDienThoai;
            existingKhachHang.Email = khachHang.Email;
            existingKhachHang.DiaChi = khachHang.DiaChi;

            
            // Cập nhật các thuộc tính khác theo nhu cầu của bạn...

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhachHangExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.KhachHangId == id);
        }


        // POST: api/NhanViens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResponeMessage>> PostKhachHang(KhachHangModelView khachHang)
        {
            KhachHang a = new KhachHang
            {
                HoTen = khachHang.HoTen,
             
                SoDienThoai = khachHang.SoDienThoai,
                Email = khachHang.Email,
                DiaChi = khachHang.DiaChi
               
            };

            try
            {
                _context.KhachHangs.Add(a);
                _context.SaveChanges();
                return await ReturnMessagesucces(a);
            }
            catch (Exception ex)
            {
                // Bạn có thể ghi log thông tin lỗi vào đây nếu cần
                // Ví dụ: _logger.LogError(ex, "Error adding NhanVien");
                return BadRequest();
            }
        }


        // DELETE: api/NhanViens/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponeMessage>> DeleteKhachHang(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();

            return await ReturnMessagesucces(khachHang);
        }

   
    }
}
