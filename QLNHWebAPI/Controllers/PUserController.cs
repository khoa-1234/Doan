using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;
using System.Data;

namespace QLNHWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
  
    public class PUserController : ControllerCustome
    {
        private readonly QlnhContext _context;
        public PUserController(QlnhContext context)
        {
            _context = context;
        }

        //Get ALL PUSER
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetAllPUser()
        {
            var Puser = await _context.Pusers.ToListAsync();
            return await ReturnMessagesucces(Puser);
        }

        //Get USer by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponeMessage>> GetIdPUser(int id)
        {
            var user= await _context.Pusers.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return await ReturnMessagesucces(user); ;
        }

        //Update
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ResponeMessage>> PutUser(int id, [FromBody] PUserModalView model)
        {
            if (id != model.UserId)
            {
                return BadRequest("ID mismatch.");
            }

            var userIdParam = new SqlParameter("@UserID", id);
            var userNameParam = new SqlParameter("@UserName", model.Username);
            var passwordParam = new SqlParameter("@Password", model.PasswordHash);
            var statusParam = new SqlParameter("@Status", model.Status);
            var nhanvienIdParam = new SqlParameter("@NhanVienID", model.NhanVienId);
            var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC spUpdateUserInformation @UserID, @UserName, @Password, @Status,@NhanVienID, @ErrorMessage OUTPUT",
                    userIdParam, userNameParam, passwordParam, statusParam, nhanvienIdParam, errorMessageParam);

                //var errorMessage = (string)errorMessageParam.Value;
                var errorMessage = errorMessageParam.Value == DBNull.Value ? null : (string)errorMessageParam.Value;


                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                // Create a response object


                // Return success message using ReturnMessagesucces
                return await ReturnMessagesucces(model);
            }
            catch (Exception ex)
            {
              
                // Log the exception (ex) here if needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating user information.");
            }
        }
        private bool PUserExists(int id)
        {
            return _context.NhanViens.Any(e => e.NhanVienId == id);
        }
        //POST api/PUser
       
    }
}
