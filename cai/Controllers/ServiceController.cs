using System;
using Microsoft.AspNetCore.Mvc;
using cai.Domain;
using Microsoft.Extensions.Options;
using cai.Service.ControllerService;
using System.Threading.Tasks;

namespace cai.Controllers
{
    public class ServiceController : ControllerBase
    {
        private readonly IControllerService _svc;

        public ServiceController(IOptions<TaskSettings> taskSettings, IControllerService svc)
        {
            _svc = svc ?? throw new ArgumentNullException(nameof(svc));
        }

        [HttpGet]
        [Route("getdata")]
     
        public async Task<ActionResult> Status(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId)) return BadRequest("Task Id not defined");
            try
            {
                await _svc.RunTask(taskId);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            return Ok("Request sent");
        }
        
    }
}
