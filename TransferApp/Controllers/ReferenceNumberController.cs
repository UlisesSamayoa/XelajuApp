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
    }
}
