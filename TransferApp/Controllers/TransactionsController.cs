using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using TransferApp.Models;
using TransferApp.Repositories;
using TransferApp.Security;

public class TransactionsController : Controller
{
    private readonly TransactionsService _service;
    private readonly ClientsService _clientes;
    private readonly BeneficiariesService _beneficiaries;
    private readonly ParametersService _parameters;
    private readonly TransactionAttachmentRepository _AttachRepo;
    public TransactionsController(TransactionsService service, ClientsService clientes, BeneficiariesService beneficiaries, ParametersService parameters, TransactionAttachmentRepository AttachRepo)
    {
        _service = service;
        _clientes = clientes;
        _beneficiaries = beneficiaries;
        _parameters = parameters;
        _AttachRepo = AttachRepo;
    }

    [Permission("Transactions.View")]
    public IActionResult Index() => View();

    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> GetAttachments(int idTransaction)
    {
        try
        {
            var attachments = await _service.GetAttachments(idTransaction);
            return Json(attachments);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var transaction = await _service.GetById(id);

            if (transaction == null)
            {
                return NotFound(new
                {
                    message = "Transaction not found"
                });
            }

            return Json(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> Create([FromForm] TransactionsModel m, List<IFormFile> ImgJustify)
    {
        try
        {
            m.UserC = "admin";

            // CLIENTE
            var client = await _clientes.GetById(m.IdClient_fk);

            if (client == null)
            {
                return BadRequest(new
                {
                    success = false,
                    type = "CLIENT_NOT_FOUND",
                    message = "Client does not exist"
                });
            }

            // BENEFICIARIO            
            bool validBeneficiary = await _beneficiaries.ValidateBeneficiaryByClient(m.IdBeneficiarie_fk, m.IdClient_fk);
            if (!validBeneficiary)
            {
                return BadRequest(new
                {
                    success = false,
                    type = "BENEFICIARY_INVALID",
                    message = "Beneficiary does not belong to selected client"
                });
            }

            var aml = await _parameters.ValidateClientTransactions(m.IdClient_fk.ToString(), m.TransactionType);

            bool amountExceeded = (aml.TotalAmount + m.Amount) > aml.MaxAmount;

            bool txExceeded = (aml.TotalTransactions + 1) > aml.MaxTransactions;

            if (amountExceeded || txExceeded)
            {
                bool noFiles = ImgJustify == null || !ImgJustify.Any();
                if (
                    string.IsNullOrWhiteSpace(m.JustifyDetails) || noFiles)
                {
                    return BadRequest(new
                    {
                        success = false,
                        type = "AML_JUSTIFY_REQUIRED",
                        message =
                            "AML limits exceeded. Justification required."
                    });
                }
            }
            await _service.Create(m, ImgJustify);

            return Ok(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                type = "SERVER_ERROR",
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> CreateDomestic([FromForm] TransactionsModel m, List<IFormFile> Domestic_ImgJustify)
    {
        try
        {
            m.UserC = "admin";

            // CLIENTE
            var client = await _clientes.GetById(m.IdClient_fk);

            if (client == null)
            {
                return BadRequest(new
                {
                    success = false,
                    type = "CLIENT_NOT_FOUND",
                    message = "Client does not exist"
                });
            }
            await _service.CreateDomestic(m, Domestic_ImgJustify);

            return Ok(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                type = "SERVER_ERROR",
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("Transactions.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Json(new { success = true });
    }

    //TRANSACCION SIMPLE
    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> CreateMorder(SimpleTransactionsModel m, List<IFormFile> Morder_ImgJustify)
    {
        try
        {
            m.UserC = "admin";
            await _service.CreateMorder(m, Morder_ImgJustify);
            return Json(new { success = true });
        }
        catch (SqlException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                code = ex.Number
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Unexpected error",
                detail = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> CreatePService(SimpleTransactionsModel m, List<IFormFile> PService_ImgJustify)
    {
        try
        {
            m.UserC = "admin";
            await _service.CreatePService(m, PService_ImgJustify);
            return Json(new { success = true });
        }
        catch (SqlException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                code = ex.Number
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Unexpected error",
                detail = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> CreateSimpleTx([FromForm] SimpleTransactionsBatchModel m, [FromForm] string Checks, List<IFormFile> SimpleTx_ImgJustify)
    {
        try
        {
            m.UserC = "admin";
            if (Checks != null)
            {
                m.Checks = JsonConvert.DeserializeObject<List<SimpleTransactionDetailModel>>(Checks);
            }
            await _service.CreateSimpleBatch(m, SimpleTx_ImgJustify);
            return Json(new { success = true });
        }
        catch (SqlException ex)
        {
            return BadRequest(new { message = ex.Message, code = ex.Number });
        }
        catch (Exception ex)
        {
            //return BadRequest(new { message = "Unexpected error", detail = ex.Message });
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> ViewAttachment(long id)
    {
        var file = await _AttachRepo.GetAttachmentById(id);
        if (file == null)
            return NotFound();
        if (!System.IO.File.Exists(file.FilePath))
            return NotFound();
        var bytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
        return File(bytes, file.ContentType);
    }

    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> DownloadAttachment(long id)
    {
        var file = await _AttachRepo.GetAttachmentById(id);
        if (file == null)
            return NotFound();
        if (!System.IO.File.Exists(file.FilePath))
            return NotFound();
        var bytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
        return File(
            bytes,
            file.ContentType,
            file.OriginalFileName
        );
    }

    [HttpPost]
    [Permission("Transactions.Edit")]
    public async Task<IActionResult> ChangeStatus(int idTransaction, string status, string transactionsStatusComment)
    {
        try
        {
            await _service.ChangeStatus(
                idTransaction,
                status,
                transactionsStatusComment
            );
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Permission("Transactions.Create")]
    public async Task<IActionResult> AddEvidence(int idTransaction, List<IFormFile> files, string user, int transactionType, string clientName, string clientDocument, string referenceNumber)
    {
        try
        {
            await _service.AddEvidence(idTransaction, files, user);
            return Json(new
            {
                success = true,
                message = "Evidence uploaded successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    public async Task<IActionResult> Preview2(int id)
    {
        var model = await _service.GetTransactionPreview(id);

        switch (model.Transaction.NumberT)
        {
            case "1":
                return PartialView("Preview/_CheckCashing", model);
            case "2":
                return PartialView("Preview/_MoneyOrder", model);
            case "3":
                return PartialView("Preview/_MoneyTransfer", model);
            case "4":
                return PartialView("Preview/_PaidService", model);
            case "5":
                return PartialView("Preview/_DomesticTransfer", model);
            default: return BadRequest();
        }
    }
    [HttpGet]
    [Permission("Transactions.View")]
    public async Task<IActionResult> Preview(int id)
    {
        var model = await _service.GetTransactionPreview(id);

        if (model == null)
            return NotFound();

        string view = model.Transaction.NumberT switch
        {
            "1" => "Preview/_CheckCashing",
            "2" => "Preview/_MoneyOrder",
            "3" => "Preview/_MoneyTransfer",
            "4" => "Preview/_PaidService",
            "5" => "Preview/_DomesticTransfer",
            _ => "Preview/_MoneyTransfer"
        };

        return PartialView(view, model);
    }




}