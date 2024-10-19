using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;
using System.Data;
using static QLNHWebAPI.Controllers.DatBanController;

namespace QLNHWebAPI.Controllers
{
   

        [Route("api/[controller]")]
        [ApiController]
        public class DonHangController : ControllerCustome

        {
            private readonly QlnhContext _context;

            // private readonly IResponseService _responseService;
            public DonHangController(QlnhContext context) //IResponseService responseService)
            {
                _context = context;

                //_responseService = responseService;
            }
            //Get ALL PUSER
            [HttpGet]
            public async Task<ActionResult<ResponeMessage>> GetAllPUser()
            {
                var datBans = await _context.DatBans.ToListAsync();
                return await ReturnMessagesucces(datBans);
            }
        // GET: api/donhang
        [HttpGet("vBep")]
        public async Task<ActionResult<ResponeMessage>> GetDonHangViews()
        {
            var donHangViews = await _context.VwXemDonHangs.ToListAsync();
            return await ReturnMessagesucces(donHangViews);
        }





    }
}
