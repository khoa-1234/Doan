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
    public class NhanViensController : ControllerCustome
    {
        private readonly QlnhContext _context;

        public NhanViensController(QlnhContext context)
        {
            _context = context;
        }

        // GET: api/NhanViens
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetNhanViens()
        {
            var nhanvien =  await _context.NhanViens.ToListAsync();
            return await ReturnMessagesucces(nhanvien) ;
        }

        // GET: api/NhanViens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponeMessage>> GetNhanVien(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            return await ReturnMessagesucces(nhanVien);
        }
      
        [HttpPost("GetNhanVienIdByUserId")]
        public async Task<IActionResult> GetNhanVienIdByUserId(GetNhanVienIdByUser username)
        {
            // Lấy UserId từ UserName
            var user = await _context.Pusers
                .Where(u => u.Username == username.username)
                .Select(u => new { u.UserId })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng với UserName đã cho." });
            }

            // Sử dụng UserId để lấy NhanVienId
            var nhanVien = await _context.NhanViens
                .Where(nv => nv.UserId == user.UserId)
                .Select(nv => new { nv.NhanVienId })
                .FirstOrDefaultAsync();

            if (nhanVien == null)
            {
                return NotFound(new { Message = "Không tìm thấy nhân viên tương ứng với UserId đã cho." });
            }

            return Ok(nhanVien);
        }
        public class GetNhanVienIdByUser
        {

            public string? username { get; set; }
        }
        // PUT: api/NhanViens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhanVien(int id, NhanVienModalView nhanVien)
        {
            // Kiểm tra nếu id không khớp với NhanVienId trong body của yêu cầu
            if (id != nhanVien.NhanvienId)
            {
                return BadRequest("ID không khớp với thông tin nhân viên.");
            }

            var existingNhanVien = await _context.NhanViens.FindAsync(id);
            if (existingNhanVien == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            // Cập nhật các thuộc tính của existingNhanVien từ nhanVien
            existingNhanVien.HoTen = nhanVien.HoTen;
            existingNhanVien.NgaySinh = nhanVien.NgaySinh;
            existingNhanVien.SoDienThoai = nhanVien.SoDienThoai;
            existingNhanVien.BoPhan = nhanVien.BoPhan;
            existingNhanVien.HinhAnhDaiDien = nhanVien.hinhAnhDaiDien;
            existingNhanVien.ChucVu = nhanVien.ChucVu;
            existingNhanVien.DiaChi = nhanVien.DiaChi;

            // Cập nhật các thuộc tính khác theo nhu cầu của bạn...

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienExists(id))
                {
                    return NotFound("Nhân viên không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanViens.Any(e => e.NhanVienId == id);
        }



        // POST: api/NhanViens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //public async Task<ActionResult<ResponeMessage>> PostNhanVien(NhanVienModalView nhanVien)
        //{
        //    NhanVien a = new NhanVien
        //    {
        //        HoTen = nhanVien.HoTen,
        //        ChucVu = nhanVien.ChucVu,
        //        NgaySinh = nhanVien.NgaySinh,
        //        SoDienThoai = nhanVien.SoDienThoai,
        //        Email = nhanVien.Email,
        //        DiaChi = nhanVien.DiaChi,
        //        BoPhan = nhanVien.BoPhan
        //    };

        //    try
        //    {
        //        _context.NhanViens.Add(a);
        //        _context.SaveChanges();
        //        return await ReturnMessagesucces(a);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Bạn có thể ghi log thông tin lỗi vào đây nếu cần
        //        // Ví dụ: _logger.LogError(ex, "Error adding NhanVien");
        //        return BadRequest();
        //    }
        //}
        [HttpPost]
 public async Task<ActionResult<ResponeMessage>> PostNhanVien([FromForm] NhanVienModalView model, IFormFile HinhAnhDaiDien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nhanVien = new NhanVien
            {
                HoTen = model.HoTen,
                ChucVu = model.ChucVu,
                NgaySinh = model.NgaySinh,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                BoPhan = model.BoPhan,
                HinhAnhDaiDien = await SaveImage(HinhAnhDaiDien)
            };

            _context.NhanViens.Add(nhanVien);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Nhân viên đã được thêm thành công", Data = nhanVien });
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/NhanVien", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{fileName}";
        }
        // Endpoint truy xuất hình ảnh không cần token
        [HttpGet("images/NhanVien/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/NhanVien", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "File not found." });
            }

            var contentType = GetContentType(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType);
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" }
        };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        // DELETE: api/NhanViens/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponeMessage>> DeleteNhanVien(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            _context.NhanViens.Remove(nhanVien);
            await _context.SaveChangesAsync();

            return await ReturnMessagesucces(nhanVien);
        }

   
    }
}
