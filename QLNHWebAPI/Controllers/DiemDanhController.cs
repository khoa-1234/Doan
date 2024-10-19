using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;



namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiemDanhController:ControllerCustome
    {
        private readonly QlnhContext _context;
        public DiemDanhController(QlnhContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetDiemDanh()
        {
            var diemDanhList = await (from vt in _context.DiemDanhs
                                join nv in _context.NhanViens on vt.NhanVienId equals nv.NhanVienId
                                select new
                                {
                                    vt.NhanVienId,
                                    vt.MaVanTayHex,
           
                                    NhanVienName = nv.HoTen // Lấy tên nhân viên từ bảng NhanVien
                                }).ToListAsync();

            return await ReturnMessagesucces(diemDanhList);
        }

        // GET: api/DiemDanh/{nhanVienId}
        [HttpGet("{nhanVienId}")]
        public async Task<ActionResult<ResponeMessage>> GetDiemDanhByNhanVien(int nhanVienId)
        {
            // Lấy điểm danh của nhân viên theo ID
            var diemDanhList = await _context.DiemDanhs
                .Where(d => d.NhanVienId == nhanVienId)
                .Include(d => d.NhanVien) // Bao gồm thông tin nhân viên nếu cần
                .ToListAsync();

            // Kiểm tra nếu không tìm thấy điểm danh nào
            if (diemDanhList == null || diemDanhList.Count == 0)
            {
                return NotFound(new { Message = "Không tìm thấy điểm danh cho nhân viên này." });
            }
            return await ReturnMessagesucces(diemDanhList);
        }
        [HttpPost]
        public async Task<ActionResult<ResponeMessage>> PostDiemDanh(DiemDanhDto diemDanhDto)
        {

            //if (diemDanhDto == null)
            //{
            //    return BadRequest("Dữ liệu điểm danh không hợp lệ.");
            //}

            //// Lấy thời gian hiện tại
            //DateTime nowUtc = DateTime.UtcNow;
            //DateOnly currentDate = DateOnly.FromDateTime(nowUtc); // Lấy ngày hiện tại
            //TimeOnly currentTime = TimeOnly.FromDateTime(nowUtc); // Lấy giờ hiện tại

            //// Kiểm tra sự tồn tại của nhân viên dựa trên mã vân tay
            //var vanTay = await _context.VanTays
            //     .Where(v => v.MaVanTayHex == diemDanhDto.MaVanTayHex)
            //     .FirstOrDefaultAsync();

            //// Kiểm tra điểm danh hiện tại của nhân viên trong ngày hôm nay

            //if (vanTay == null)
            //{
            //    return NotFound(new { Message = "Nhân viên hoặc mã vân tay không tồn tại." });
            //}

            //// Kiểm tra điểm danh hiện tại của nhân viên

            //var nhanVienId = vanTay.NhanVienId;
            //// Kiểm tra điểm danh hiện tại của nhân viên trong ngày hôm nay
            //var diemDanhHienTai = await _context.DiemDanhs
            //    .Where(d => d.NhanVienId == nhanVienId && d.Ngay == currentDate)
            //    .OrderByDescending(d => d.ThoiGianVao)
            //    .FirstOrDefaultAsync();
            //if (diemDanhHienTai == null)
            //{
            //    // Nếu chưa có điểm danh nào trong ngày, tạo mới điểm danh với thời gian vào
            //    var diemDanh = new DiemDanh
            //    {
            //        NhanVienId = vanTay.NhanVienId,
            //        Ngay = currentDate,
            //        MaVanTayHex = diemDanhDto.MaVanTayHex,
            //        ThoiGianVao = currentTime,
            //        VanTayId = vanTay.VanTayId
            //    };

            //    _context.DiemDanhs.Add(diemDanh);
            //}
            //else
            //{
            //    if (diemDanhHienTai.ThoiGianRa == null)
            //    {
            //        // Nếu đã có điểm danh vào nhưng chưa có thời gian ra, cập nhật thời gian ra
            //        diemDanhHienTai.ThoiGianRa = currentTime;
            //        _context.DiemDanhs.Update(diemDanhHienTai);
            //    }
            //    else
            //    {
            //        // Nếu nhân viên đã điểm danh vào và ra trong ngày, không thực hiện thêm
            //        return BadRequest("Nhân viên đã điểm danh vào và ra trong ngày.");
            //    }
            //}

            //await _context.SaveChangesAsync();

            //return Ok(new { Message = "Điểm danh thành công" });
            return Ok(diemDanhDto);
        }
       // DTO cho điểm danh
        public class DiemDanhDto
        {
            public string MaVanTayHex { get; set; }
      
        }

    }
}
