using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLNHWebAPI.Models;
using QLNHWebAPI.ViewModel;

namespace QLNHWebAPI.Controllers
{
    public class ControllerCustome : ControllerBase

    {
        public ControllerCustome()
        { }

            public async Task<ActionResult<ResponeMessage>> ReturnMessagesucces<T>(T data)
            {
            var response = new ResponeMessage
            {
                Data = data,
                Message = "Thao tac thanh cong",
                IsSuccess = true
            };

            return   Ok(response);
                }
       

    }
}