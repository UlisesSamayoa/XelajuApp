using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransferApp.Models;

public class TransactionsController : Controller
{
    private readonly TransactionsService _service;
    private readonly ClientsService _clientes;
    private readonly BeneficiariesService _beneficiaries;
    private readonly ParametersService _parameters;

    public TransactionsController(TransactionsService service,ClientsService clientes,BeneficiariesService beneficiaries,ParametersService parameters)
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
    public async Task<IActionResult> Create(
    [FromForm] TransactionsModel m,
    IFormFile ImgJustify)
    {
        try
        {
            m.UserC = "admin";

            // CLIENTE
            var client =await _clientes.GetById(m.IdClient_fk);

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
            //bool validBeneficiary =await _beneficiaries.ValidateBeneficiaryByClient(m.IdClient_fk,m.IdBeneficiarie_fk ?? 0);
            bool validBeneficiary =await _beneficiaries.ValidateBeneficiaryByClient(m.IdBeneficiarie_fk,m.IdClient_fk);
            if (!validBeneficiary)
            {
                return BadRequest(new
                {
                    success = false,
                    type = "BENEFICIARY_INVALID",
                    message ="Beneficiary does not belong to selected client"
                });
            }

            var aml =await _parameters.ValidateClientTransactions(m.IdClient_fk.ToString());

            bool amountExceeded =(aml.TotalAmount + m.Amount)> aml.MaxAmount;

            bool txExceeded =(aml.TotalTransactions + 1)> aml.MaxTransactions;

            if (amountExceeded || txExceeded)
            {
                if (
                    string.IsNullOrWhiteSpace(
                        m.JustifyDetails)
                    ||
                    ImgJustify == null)
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
            await _service.Create(m);

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
}