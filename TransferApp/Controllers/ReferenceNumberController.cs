using Microsoft.AspNetCore.Mvc;
using TransferApp.Repositories;

namespace TransferApp.Controllers
{
    public class ReferenceNumberController : Controller
    {
        private readonly ReferenceNumberRepository _service;

        public ReferenceNumberController(ReferenceNumberRepository service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetReferencePreview(int senderCompany, int transactionType)
        {
            return Json(
                await _service.GetReferencePreview(senderCompany, transactionType)
            );
        }

        //[HttpGet]
        //public async Task<IActionResult> GetReferencePreview_PS(int senderCompany, int transactionType, int ClientID)
        //{
        //    return Json(
        //        await _service.GetReferencePreview_PS(senderCompany, transactionType, ClientID)
        //    );
        //}
        [HttpGet]
        public async Task<IActionResult> GetReferencePreview_PS(int senderCompany, int transactionType, int ClientID)
        {
            try
            {
                var preview = await _service.GetReferencePreview_PS(senderCompany, transactionType, ClientID);
                return Json(preview);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
