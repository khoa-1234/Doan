using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class VanTayController:ControllerCustome
    {
        private readonly QlnhContext _context;
        public VanTayController(QlnhContext context)
        {
            _context = context;
        }
        // GET: api/VanTays
    


        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetVanTay()
        {
            var vantay = await (from vt in _context.VanTays
                                join nv in _context.NhanViens on vt.NhanVienId equals nv.NhanVienId
                                select new
                                {
                                    vt.VanTayId,
                                    vt.MaVanTayHex,
                                    vt.MoTa,
                                    NhanVienName = nv.HoTen // Lấy tên nhân viên từ bảng NhanVien
                                }).ToListAsync();

            return await ReturnMessagesucces(vantay);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ResponeMessage>> GetVanTayId(int id)
        {
            var vantay = await _context.VanTays.FindAsync(id);

            if (vantay == null)
            {
                return NotFound();
            }

            return await ReturnMessagesucces(vantay);
        }

        [HttpPut("Suavantay{id}")]
        public async Task<IActionResult> PutVanTay(int id, UpdatVanTayDTO updatVanTayDTO  )
        {
            if (id != updatVanTayDTO.VanTayId)
            {
                return BadRequest();
            }

            var existingVanTay = await _context.VanTays.FindAsync(id);
            if (existingVanTay == null)
            {
                return NotFound();
            }
           
            // Cập nhật các thuộc tính của existingNhanVien từ nhanVien
            existingVanTay.MaVanTayHex = updatVanTayDTO.MaVanTayHex;
            existingVanTay.MoTa = updatVanTayDTO.MoTa;
            existingVanTay.ThoiGianCapNhat = updatVanTayDTO.ThoiGianCapNhat;
            // Cập nhật các thuộc tính khác theo nhu cầu của bạn...

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VanTayExists(id))
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
        // Kiểm tra xem Vân Tay có tồn tại không

        [HttpPost("ThemVanTay")]
        public async Task<ActionResult<ResponeMessage>> ThemVanTay(ThemVanTayDTO themvanTayDTO)
        {
            //if (sanpham.hinhanhdaidien != null)
            //{
            //    var uploadfolderpath = path.combine(directory.getcurrentdirectory(), "uploads");
            //    var
            // uploadlam sau
            VanTay vantay1 = new VanTay
            {
                MaVanTayHex= themvanTayDTO.MaVanTayHex,
                MoTa = themvanTayDTO.MoTa,
                NhanVienId = themvanTayDTO.NhanVienId,
                ThoiGianTao = themvanTayDTO.ThoiGianTao
                
            };
            try
            {
                _context.VanTays.Add(vantay1);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Bạn có thể ghi log thông tin lỗi vào đây nếu cần
                // Ví dụ: _logger.LogError(ex, "Error adding NhanVien");

            }


            return await ReturnMessagesucces(vantay1);



        }
        [HttpDelete("XoaVanTay")]
        public async Task<ActionResult<ResponeMessage>> DeleteVanTay(string mavantayhex)
        {
            if (string.IsNullOrEmpty(mavantayhex))
            {
                return BadRequest();
            }

            // Tìm bản ghi dựa trên MaVanTayHex
            var vantay = await _context.VanTays
                .FirstOrDefaultAsync(v => v.MaVanTayHex == mavantayhex);

            if (vantay == null)
            {
                return NotFound();
            }

            // Xóa bản ghi
            _context.VanTays.Remove(vantay);
            await _context.SaveChangesAsync();

            return await ReturnMessagesucces(vantay);
        }
        private bool VanTayExists(int id)
        {
            return _context.VanTays.Any(e => e.VanTayId == id);
        }
        public class VanTayDTO
        {
            public int? VanTayId { get; set; }
            public string MaVanTayHex { get; set; }
            public string MoTa { get; set; }
            public DateTime? ThoiGianCapNhat { get; set; }
            public DateTime? ThoiGianTao { get; set; }
            public int NhanVienId { get; set; }
            public string? TenNhanVien { get; set; }
        }
        public class UpdatVanTayDTO
        {
            public int VanTayId { get; set; }
            public string MaVanTayHex { get; set; }
            public string MoTa { get; set; }
            public DateTime? ThoiGianCapNhat { get; set; }

        }
        public class ThemVanTayDTO
        {
            public string MaVanTayHex { get; set; }
            public string MoTa { get; set; }
            public DateTime? ThoiGianTao { get; set; }
            public int NhanVienId { get; set; }
        }
    }
}
