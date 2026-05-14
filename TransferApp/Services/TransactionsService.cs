using TransferApp.Models;

public class TransactionsService
{
    private readonly TransactionsRepository _repo;

    public TransactionsService(TransactionsRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TransactionsModel>> GetAll() => await _repo.GetAll();
    //public async Task Create(TransactionsModel m, IFormFile ImgJustify) => await _repo.Create(m);
    public async Task Create(TransactionsModel m, IFormFile ImgJustify)
    {
        string ruta = SaveTransactionFile(
            ImgJustify,
            m.TransactionType,
            m.SenderDocumentNumber,
            m.ReferenceNumber
        );

        m.TransactionFile = ruta;

        await _repo.Create(m);
    }
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
    public async Task<TransactionsModel> GetById(int id)
    {
        return await _repo.GetById(id);
    }
    public async Task CreateSimple(SimpleTransactionsModel m)
    {
        await _repo.CreateSimple(m);
    }

    private string SaveTransactionFile(IFormFile file, int transactionType, string documentNumber, string referenceNumber)
    {
        if (file == null)
            return null;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            throw new Exception("Invalid file type");

        //tamaño máximo 5MB
        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("File too large (max 5MB)");

        string txPrefix = transactionType switch
        {
            1 => "CC",
            2 => "MO",
            3 => "MT",
            4 => "PS",
            _ => "OT"
        };
        var now = DateTime.Now;

        string year = now.Year.ToString();
        string month = now.ToString("MM");
        string day = now.ToString("dd");

        //ruta base C:\TransactionFiles, preguntar si sera C
        string basePath = @"C:\TransactionFiles";

        //estructura de carpetas
        string folderPath = Path.Combine(
            basePath,
            txPrefix,
            year,
            month,
            day
        );

        //crear carpetas si no existen
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        //limpiar caracteres
        documentNumber = documentNumber?.Replace("-", "").Replace(" ", "");
        referenceNumber = referenceNumber?.Replace("-", "").Replace(" ", "");

        string fileName = $"{now:yyyyMMdd}_{now:HHmmss}_{txPrefix}_{documentNumber}_{referenceNumber}{ext}";
        string fullPath = Path.Combine(folderPath, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return fullPath;
    }

    public async Task ChangeStatus(int idTransaction, string status)
    {
        if (
            status != "Completed" &&
            status != "Pending" &&
            status != "Rejected"
        )
        {
            throw new Exception(
                "Invalid status"
            );
        }

        await _repo.ChangeStatus(
            idTransaction,
            status
        );
    }

}