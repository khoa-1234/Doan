using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhomMonAnsController : ControllerCustome
    {
        private readonly QlnhContext _context;

        public NhomMonAnsController(QlnhContext context)
        {
            _context = context;
     
        }
        // GET: api/NhomMonAns
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetNhomMonAns()
        {
            var nhomMonAns = await _context.NhomMonAns
                .Include(n => n.InverseParent) // Lấy các nhóm con
                .ToListAsync();

            var nhomMonAnDtos = BuildNhomMonAnTree(nhomMonAns);
            return await ReturnMessagesucces(nhomMonAnDtos);
        }
        public class NhomMonAnDto
        {
            public int NhomMonAnId { get; set; }
            public string? TenNhom { get; set; }
            public List<NhomMonAnDto> Children { get; set; } = new List<NhomMonAnDto>();
        }


        // Hàm xây dựng cây cho các nhóm món ăn
        private List<NhomMonAnDto> BuildNhomMonAnTree(List<NhomMonAn> nhomMonAns)
        {
            var lookup = nhomMonAns.ToLookup(n => n.ParentId); // Tạo lookup để truy xuất nhanh
            var rootNodes = lookup[null]; // Lấy các nút gốc (không có ParentId)

            return BuildTree(rootNodes, lookup); // Xây dựng cây
        }

        // Hàm đệ quy để xây dựng cây
        private List<NhomMonAnDto> BuildTree(IEnumerable<NhomMonAn> nodes, ILookup<int?, NhomMonAn> lookup)
        {
            return nodes.Select(n => new NhomMonAnDto
            {
                NhomMonAnId = n.NhomMonAnId,
                TenNhom = n.TenNhom,
                Children = BuildTree(lookup[n.NhomMonAnId], lookup) // Tìm các nút con
            }).ToList();
        }



        // GET: api/NhomMonAns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponeMessage>> GetNhomMonAn(int id)
        {
            var nhomMonAn = await _context.NhomMonAns.FindAsync(id);

            if (nhomMonAn == null)
            {
                return NotFound();
            }

            return await ReturnMessagesucces( nhomMonAn);
        }

        // PUT: api/NhomMonAns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponeMessage>> PutNhomMonAn(int id, NhomMonAnModalView nhomMonAn)
        {
            if (id != nhomMonAn.NhomMonAnId)
            {
                return BadRequest();
            }

            _context.Entry(nhomMonAn).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhomMonAnExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await ReturnMessagesucces(nhomMonAn);
        }

        // POST: api/NhomMonAns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostNhomMonAn")]
        public async Task<ActionResult<ResponeMessage>> PostNhomMonAn(NhomMonAnModalView nhomMonAn)
        {
            NhomMonAn a = new NhomMonAn
            {
                TenNhom = nhomMonAn.TenNhom
               
            };

            try
            {
                _context.NhomMonAns.Add(a);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Bạn có thể ghi log thông tin lỗi vào đây nếu cần
                // Ví dụ: _logger.LogError(ex, "Error adding NhanVien");
               
            }
            return await ReturnMessagesucces(a);
           
        }

        // DELETE: api/NhomMonAns/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponeMessage>> DeleteNhomMonAn(int? id)
        {
            var nhomMonAn = await _context.NhomMonAns.FindAsync(id);
            if (nhomMonAn == null)
            {
                return NotFound();
            }

            _context.NhomMonAns.Remove(nhomMonAn);
            await _context.SaveChangesAsync();

            return await ReturnMessagesucces(nhomMonAn);
        }

        private bool NhomMonAnExists(int id)
        {
            return _context.NhomMonAns.Any(e => e.NhomMonAnId == id);
        }
    }
}
