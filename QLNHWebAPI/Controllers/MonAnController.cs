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

    public class MonAnController : ControllerCustome

    {
        private readonly QlnhContext _context;

       // private readonly IResponseService _responseService;
        public MonAnController(QlnhContext context) //IResponseService responseService)
        {
            _context = context;

            //_responseService = responseService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponeMessage>> GetAll()
        {
            var monAns = await _context.MonAns.ToListAsync();

            return await ReturnMessagesucces(monAns);
        }
        [HttpGet("MonAnTheoNhom")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponeMessage>> Get(int nhomMonAnId)
        {
            var monAns = await _context.MonAns
       .Where(m => m.NhomMonAnId == nhomMonAnId)
       .ToListAsync();

            return await ReturnMessagesucces(monAns);
        }
        //[HttpGet("{id}")]
        // public async Task<ActionResult<SanPhamModalView>> GetSanPham(int id)

        [HttpPost("Filter")]
        public async Task<ActionResult<ResponeMessage>> PostSanPhamFilter(string? searchTerm)
            {
            // Chuyển đổi searchTerm thành int (nếu có thể)
            int? nhomMonAnId = null;
            if (int.TryParse(searchTerm, out int parsedId))
            {
                nhomMonAnId = parsedId;
            }

            var sanphams = await _context.MonAns
                .Where(p => (string.IsNullOrEmpty(searchTerm) || p.TenMonAn.Contains(searchTerm))
                        || (nhomMonAnId.HasValue && p.NhomMonAnId == nhomMonAnId))
                .ToListAsync();

            // Trả về kết quả thành công
            return await ReturnMessagesucces(sanphams);

        }

        [HttpPost]
        public async Task<ActionResult<ResponeMessage>> AddMonAn([FromForm] MonAnModalView monan, [FromForm] IFormFile HinhAnhDaiDien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var monan1 = new MonAn
            {
                TenMonAn = monan.TenMonAn,
                MoTa = monan.MoTa,
               
                NhomMonAnId = monan.NhomMonAnId,
               
                ThoiGianTao= DateTime.Now
                
            };
            _context.MonAns.Add(monan1);
            await _context.SaveChangesAsync();

              
            
            
            return await ReturnMessagesucces(monan1);


        
        }
        private async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/MonAn", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{fileName}";
        }
        // Endpoint truy xuất hình ảnh không cần token
        [HttpGet("images/MonAn/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/MonAn", fileName);

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
        [HttpPut("UpdateSanPham{id}")]
        public async Task<ActionResult<ResponeMessage>> UpdateSanPham(int id,MonAnModalView monAn)
        {
            if (id != monAn.MonAnId)
            {
                return BadRequest();
            }

            var existingMonAN = await _context.MonAns.FindAsync(id);
            if (existingMonAN == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính của existingNhanVien từ nhanVien
            existingMonAN.TenMonAn = monAn.TenMonAn;
            existingMonAN.MoTa = monAn.MoTa;
            existingMonAN.NhomMonAnId = monAn.NhomMonAnId;
            existingMonAN.ThoiGianCapNhat = DateTime.Now;



            // Cập nhật các thuộc tính khác theo nhu cầu của bạn...

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!existingMon(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await ReturnMessagesucces(existingMonAN);
        }
        private bool existingMon(int id)
        {
            return _context.MonAns.Any(e => e.MonAnId == id);
        }
        [HttpDelete("XoaMonAn")]
        public async Task<ActionResult<ResponeMessage>> DeleteSanPham(int? id)
        {
            var sanpham = await _context.MonAns.FindAsync(id);
            if(sanpham==null)
            { return NotFound(); }
            _context.MonAns.Remove(sanpham);
            await _context.SaveChangesAsync();


            return await ReturnMessagesucces(sanpham);
        }

    }
}
