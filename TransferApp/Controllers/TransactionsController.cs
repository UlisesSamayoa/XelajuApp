using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransferApp.Models;

public class TransactionsController : Controller
{
    private readonly TransactionsService _service;
    private readonly ClientsService _clientes;
    private readonly BeneficiariesService _beneficiaries;
    private readonly ParametersService _parameters;

    public TransactionsController(TransactionsService service, ClientsService clientes, BeneficiariesService beneficiaries, ParametersService parameters)
    {
        _service = service;

        _clientes = clientes;

        _beneficiaries = beneficiaries;

        _parameters = parameters;
    }
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());
    [HttpGet]
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
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Json(new { success = true });
    }

    //TRANSACCION SIMPLE
    //[HttpPost]
    //public async Task<IActionResult> CreateSimpleTx(SimpleTransactionsModel m)
    //{
    //    await _service.CreateSimple(m);
    //    return Json(new { success = true });
    //}
    [HttpPost]
    public async Task<IActionResult> CreateSimpleTx(SimpleTransactionsModel m)
    {
        try
        {
            m.UserC = "admin";

            await _service.CreateSimple(m);

            return Json(new { success = true });
        }
        catch (SqlException ex)
        {
            // 🔴 errores controlados desde THROW en el SP
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

    [HttpGet]
    public async Task<IActionResult> ViewTransactionFile(int id)
    {
        var tx = await _service.GetById(id);

        if (tx == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(tx.TransactionFile))
            return NotFound();

        if (!System.IO.File.Exists(tx.TransactionFile))
            return NotFound();

        var ext = Path.GetExtension(tx.TransactionFile).ToLower();

        string contentType = ext switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

        var bytes = await System.IO.File.ReadAllBytesAsync(
            tx.TransactionFile
        );

        return File(bytes, contentType);
    }
    [HttpGet]
    public async Task<IActionResult> DownloadTransactionFile(int id)
    {
        var tx = await _service.GetById(id);

        if (tx == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(tx.TransactionFile))
            return NotFound();

        if (!System.IO.File.Exists(tx.TransactionFile))
            return NotFound();

        var ext = Path.GetExtension(tx.TransactionFile).ToLower();

        string contentType = ext switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

        var bytes = await System.IO.File.ReadAllBytesAsync(
            tx.TransactionFile
        );

        var fileName = Path.GetFileName(tx.TransactionFile);

        return File(bytes, contentType, fileName);
    }

    [HttpPost]
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

}