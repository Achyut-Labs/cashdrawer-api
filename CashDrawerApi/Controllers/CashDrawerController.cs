using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace CashDrawerApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CashDrawerController : ControllerBase
    {
        private readonly ILogger<CashDrawerController> _logger;

        public CashDrawerController(ILogger<CashDrawerController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "Open")]
        public IActionResult Open()
        {
            try
            {
                var printerName = Request.Headers["PrinterName"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(printerName))
                    return new BadRequestObjectResult("PrinterName is not supplied in header");
                var codeOpenCashDrawer = new byte[] { 27, 112, 48, 55, 121 };
                var pUnmanagedBytes = Marshal.AllocCoTaskMem(5);
                Marshal.Copy(codeOpenCashDrawer, 0, pUnmanagedBytes, 5);
                var success = RawPrinterHelper.SendBytesToPrinter(printerName, pUnmanagedBytes, 5);
                if(!success)
                    return new BadRequestObjectResult("Request processed unsuccessfully");
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return new OkObjectResult("Request processed successfully");
            }
            catch (Exception e)
            {
                return BadRequest($"Unknown error occurred while processing request. Error is : {e.Message}");
            }
          
        }
    }
}