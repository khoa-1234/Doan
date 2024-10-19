using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class KhuvucController:ControllerCustome
    {
        private readonly QlnhContext _context;

        // private readonly IResponseService _responseService;
        public KhuvucController(QlnhContext context) //IResponseService responseService)
        {
            _context = context;

            //_responseService = responseService;
        }
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> Get()
        {
            var bans = _context.KhuVucs.ToList();
            return await ReturnMessagesucces(bans);
        }
        [HttpPost]
        public async Task<ActionResult<ResponeMessage>> ThemBan(KhuVucModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var khuvuc1 = new KhuVuc
            {
                TenKhuVuc = model.TenKhuVuc,
                Mota = model.MoTa,
               
            };

            _context.KhuVucs.Add(khuvuc1);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Khu vuc đã được thêm thành công", Data = khuvuc1 });
        }


    }
}
