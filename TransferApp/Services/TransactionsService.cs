using TransferApp.Models;
using TransferApp.Repositories;

public class TransactionsService
{
    private readonly TransactionsRepository _repo;
    private readonly TransactionAttachmentRepository _repoAttach;
    private readonly ReferenceNumberRepository _repoRefe;
    private readonly CompanyRepository _repoCompany;

    public TransactionsService(TransactionsRepository repo, TransactionAttachmentRepository repoAttach, ReferenceNumberRepository repoRefe, CompanyRepository repoCompany)
    {
        _repo = repo;
        _repoAttach = repoAttach;
        _repoRefe = repoRefe;
        _repoCompany = repoCompany;
    }

    public async Task<List<TransactionsModel>> GetAll() => await _repo.GetAll();

    public async Task Create(TransactionsModel m, List<IFormFile> ImgJustify)
    {
        // Validar compañía
        var company = await _repoCompany.GetById(m.SenderCompany);
        if (company == null) throw new Exception("Company not found");
        // Validar que la compañía pertenece al tipo seleccionado
        if (company.TransactionType != m.TransactionType) throw new Exception("Invalid company for selected transaction type");
        // Generar referencia
        m.ReferenceNumber = await _repoRefe.GetNextReferenceNumber(m.SenderCompany, m.TransactionType);
        int idTransaction = await _repo.Create(m);
        // ====================================
        // UPDATE REFERENCE SEQUENCE
        // ====================================

        long sequence =
            ExtractSequence(
                m.ReferenceNumber);

        await _repoRefe.SaveReferenceSequence(m.SenderCompany, m.TransactionType, sequence);

        if (ImgJustify != null && ImgJustify.Any())
        {
            foreach (var file in ImgJustify)
            {
                string filePath = SaveTransactionFile(
                    file,
                    m.TransactionType,
                    m.SenderName,
                    m.SenderDocumentNumber,
                    m.ReferenceNumber
                );
                try
                {
                    await _repoAttach.CreateAttachment(
                        new TransactionAttachmentModel
                        {
                            IdTransaction = idTransaction,
                            FileName = Path.GetFileName(filePath),
                            OriginalFileName = file.FileName,
                            FileExtension = Path.GetExtension(file.FileName),
                            ContentType = file.ContentType,
                            FilePath = filePath,
                            AttachmentType = "TRANSACTION_DOCUMENT",
                            FileSize = file.Length,
                            CreatedBy = m.UserC
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
    public async Task CreateDomestic(TransactionsModel m, List<IFormFile> ImgJustify)
    {
        var company = await _repoCompany.GetById(m.ReceiverCompany);
        if (company == null) throw new Exception("Company not found");
        if (company.TransactionType != m.TransactionType) throw new Exception("Invalid company for selected transaction type");
        m.ReferenceNumber = await _repoRefe.GetNextReferenceNumber(m.ReceiverCompany, m.TransactionType);
        int idTransaction = await _repo.CreateDomestic(m);
        long sequence =
            ExtractSequence(
                m.ReferenceNumber);

        await _repoRefe.SaveReferenceSequence(m.ReceiverCompany, m.TransactionType, sequence);

        if (ImgJustify != null && ImgJustify.Any())
        {
            foreach (var file in ImgJustify)
            {
                string filePath = SaveTransactionFile(
                    file,
                    m.TransactionType,
                    m.ReceiverName,
                    m.ReceiverDocumentNumber,
                    m.ReferenceNumber
                );
                try
                {
                    await _repoAttach.CreateAttachment(
                        new TransactionAttachmentModel
                        {
                            IdTransaction = idTransaction,
                            FileName = Path.GetFileName(filePath),
                            OriginalFileName = file.FileName,
                            FileExtension = Path.GetExtension(file.FileName),
                            ContentType = file.ContentType,
                            FilePath = filePath,
                            AttachmentType = "TRANSACTION_DOCUMENT",
                            FileSize = file.Length,
                            CreatedBy = m.UserC
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
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
    //public async Task CreateMorder(SimpleTransactionsModel m)
    //{
    //    await _repo.CreateMorder(m);
    //}
    public async Task CreateMorder(SimpleTransactionsModel m, List<IFormFile> files)
    {
        List<TransactionAttachmentModel> attachments = new();
        // ====================================
        // GENERATE REFERENCE
        // ====================================
        m.ReferenceNumber = await _repoRefe.GetNextReferenceNumber(m.Company, m.TransactionType);
        if (files != null && files.Any())
        {
            foreach (var file in files)
            {
                string filePath = SaveTransactionFile(
                    file,
                    m.TransactionType,
                    m.SenderName,
                    m.SenderDocumentNumber,
                    m.ReferenceNumber
                );
                attachments.Add(
                    new TransactionAttachmentModel
                    {
                        FileName = Path.GetFileName(filePath),
                        OriginalFileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName),
                        ContentType = file.ContentType,
                        FilePath = filePath,
                        AttachmentType = "TRANSACTION_DOCUMENT",
                        FileSize = file.Length,
                        CreatedBy = m.UserC
                    });
            }
        }
        int idTransaction = await _repo.CreateMorder(m);
        if (idTransaction <= 0)
        {
            throw new Exception(
                $"Could not create transaction {m.ReferenceNumber}"
            );
        }
        // ====================================
        // UPDATE SEQUENCE
        // ====================================

        long sequence = ExtractSequence(m.ReferenceNumber);
        await _repoRefe.SaveReferenceSequence(m.Company, m.TransactionType, sequence);
        foreach (var attach in attachments)
        {
            await _repoAttach.CreateAttachment(
                new TransactionAttachmentModel
                {
                    IdTransaction = idTransaction,
                    FileName = attach.FileName,
                    OriginalFileName = attach.OriginalFileName,
                    FileExtension = attach.FileExtension,
                    ContentType = attach.ContentType,
                    FilePath = attach.FilePath,
                    AttachmentType = attach.AttachmentType,
                    FileSize = attach.FileSize,
                    CreatedBy = attach.CreatedBy
                });
        }
    }


    public async Task CreatePService(SimpleTransactionsModel m, List<IFormFile> files)
    {
        List<TransactionAttachmentModel> attachments = new();
        // ====================================
        // GENERATE REFERENCE
        // ====================================
        m.ReferenceNumber = await _repoRefe.GetNextReferenceNumber_PS(m.ServiceCompanyPS, m.Company, m.TransactionType, m.IdClient_fk);
        if (files != null && files.Any())
        {
            foreach (var file in files)
            {
                string filePath = SaveTransactionFile(
                    file,
                    m.TransactionType,
                    m.SenderName,
                    m.SenderDocumentNumber,
                    m.ReferenceNumber
                );
                attachments.Add(
                    new TransactionAttachmentModel
                    {
                        FileName = Path.GetFileName(filePath),
                        OriginalFileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName),
                        ContentType = file.ContentType,
                        FilePath = filePath,
                        AttachmentType = "TRANSACTION_DOCUMENT",
                        FileSize = file.Length,
                        CreatedBy = m.UserC
                    });
            }
        }
        int idTransaction = await _repo.CreatePService(m);
        if (idTransaction <= 0)
        {
            throw new Exception(
                $"Could not create transaction {m.ReferenceNumber}"
            );
        }
        // ====================================
        // UPDATE SEQUENCE
        // ====================================
        long sequence = ExtractSequence(m.ReferenceNumber);
        await _repoRefe.SaveReferenceSequence(m.Company, m.TransactionType, sequence);
        foreach (var attach in attachments)
        {
            await _repoAttach.CreateAttachment(
                new TransactionAttachmentModel
                {
                    IdTransaction = idTransaction,
                    FileName = attach.FileName,
                    OriginalFileName = attach.OriginalFileName,
                    FileExtension = attach.FileExtension,
                    ContentType = attach.ContentType,
                    FilePath = attach.FilePath,
                    AttachmentType = attach.AttachmentType,
                    FileSize = attach.FileSize,
                    CreatedBy = attach.CreatedBy
                });
        }
    }

    //private string SaveTransactionFile(IFormFile file, int transactionType, string documentNumber, string referenceNumber)
    //{
    //    if (file == null)
    //        return null;
    //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
    //    var ext = Path.GetExtension(file.FileName).ToLower();
    //    if (!allowedExtensions.Contains(ext))
    //        throw new Exception("Invalid file type");

    //    //tamaño máximo 5MB
    //    if (file.Length > 5 * 1024 * 1024)
    //        throw new Exception("File too large (max 5MB)");

    //    string txPrefix = transactionType switch
    //    {
    //        1 => "CC",
    //        2 => "MO",
    //        3 => "MT",
    //        4 => "PS",
    //        _ => "OT"
    //    };
    //    var now = DateTime.Now;

    //    string year = now.Year.ToString();
    //    string month = now.ToString("MM");
    //    string day = now.ToString("dd");

    //    //ruta base C:\TransactionFiles, preguntar si sera C
    //    string basePath = @"C:\TransactionFiles";

    //    //estructura de carpetas
    //    string folderPath = Path.Combine(
    //        basePath,
    //        txPrefix,
    //        year,
    //        month,
    //        day
    //    );

    //    //crear carpetas si no existen
    //    if (!Directory.Exists(folderPath))
    //        Directory.CreateDirectory(folderPath);

    //    //limpiar caracteres
    //    documentNumber = documentNumber?.Replace("-", "").Replace(" ", "");
    //    referenceNumber = referenceNumber?.Replace("-", "").Replace(" ", "");

    //    string fileName = $"{now:yyyyMMdd}_{now:HHmmss}_{txPrefix}_{documentNumber}_{referenceNumber}{ext}";
    //    string fullPath = Path.Combine(folderPath, fileName);
    //    using (var stream = new FileStream(fullPath, FileMode.Create))
    //    {
    //        file.CopyTo(stream);
    //    }
    //    return fullPath;
    //}

    private string SaveTransactionFile(IFormFile file, int transactionType, string clientName, string clientDocument, string referenceNumber)
    {
        if (file == null)
            return null;

        var allowedExtensions =
            new[] { ".jpg", ".jpeg", ".png", ".pdf" };

        var ext =
            Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(ext))
            throw new Exception("Invalid file type");

        // MAX 5MB
        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("File too large (max 5MB)");

        // TX PREFIX
        string txPrefix = transactionType switch
        {
            1 => "CC",
            2 => "MO",
            3 => "MT",
            4 => "PS",
            5 => "DT",
            _ => "OT"
        };

        var now = DateTime.Now;

        string year = now.Year.ToString();
        string month = now.ToString("MM");
        string day = now.ToString("dd");

        // BASE PATH
        string basePath = @"C:\TransactionFiles";

        // CLEAN CLIENT DATA
        clientName = CleanPathValue(clientName);
        clientDocument = CleanPathValue(clientDocument);

        // CLIENT FOLDER
        string clientFolder =
            $"{clientName}_{clientDocument}";

        // FOLDER STRUCTURE
        string folderPath = Path.Combine(
            basePath,
            clientFolder,
            year,
            txPrefix,
            month,
            day
        );

        // CREATE FOLDERS
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // CLEAN REFERENCE
        referenceNumber = referenceNumber?
            .Replace("-", "")
            .Replace(" ", "");

        // UNIQUE FILE NAME
        string fileName =
            $"{now:yyyyMMdd_HHmmss_fff}_{txPrefix}_{referenceNumber}{ext}";

        string fullPath =
            Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(
            fullPath,
            FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fullPath;
    }

    private string CleanPathValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "UNKNOWN";

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c.ToString(), "");
        }

        return value
            .Trim()
            .Replace(" ", "_");
    }

    public async Task ChangeStatus(int idTransaction, string status, string transactionsStatusComment)
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
            status,
            transactionsStatusComment
        );
    }

    public async Task<long> CreateAttachment(TransactionAttachmentModel model)
    {
        return await _repoAttach.CreateAttachment(model);
    }

    public async Task<List<TransactionAttachmentModel>> GetAttachments(int idTransaction)
    {
        return await _repoAttach.GetAttachments(idTransaction);
    }
    //public async Task CreateSimpleBatch(SimpleTransactionsBatchModel m, List<IFormFile> files)
    //{
    //    if (m.Checks == null || !m.Checks.Any())
    //    {
    //        throw new Exception("No checks received");
    //    }

    //    // ====================================
    //    // VALIDATE DUPLICATED REFERENCES
    //    // ====================================
    //    var duplicatedReferences = m.Checks
    //        .Where(x => !string.IsNullOrWhiteSpace(x.ReferenceNumber))
    //        .GroupBy(x => x.ReferenceNumber.Trim())
    //        .Where(g => g.Count() > 1)
    //        .Select(g => g.Key)
    //        .ToList();

    //    if (duplicatedReferences.Any())
    //    {
    //        throw new Exception(
    //            $"Duplicated references detected: {string.Join(", ", duplicatedReferences)}"
    //        );
    //    }

    //    // ====================================
    //    // SAVE FILES ONCE
    //    // ====================================
    //    List<TransactionAttachmentModel> attachments = new();
    //    string batchReferenceImg = $"{m.Checks.First().ReferenceNumber}_{m.Checks.Last().ReferenceNumber}";
    //    if (files != null && files.Any())
    //    {
    //        foreach (var file in files)
    //        {
    //            string filePath = SaveTransactionFile(
    //                file,
    //                m.TransactionType,
    //                m.SenderName,
    //                m.SenderDocumentNumber,
    //                batchReferenceImg
    //            );

    //            attachments.Add(
    //                new TransactionAttachmentModel
    //                {
    //                    FileName = Path.GetFileName(filePath),
    //                    OriginalFileName = file.FileName,
    //                    FileExtension = Path.GetExtension(file.FileName),
    //                    ContentType = file.ContentType,
    //                    FilePath = filePath,
    //                    AttachmentType = "TRANSACTION_DOCUMENT",
    //                    FileSize = file.Length,
    //                    CreatedBy = m.UserC
    //                });
    //        }
    //    }

    //    // ====================================
    //    // CREATE EACH CHECK
    //    // ====================================
    //    foreach (var check in m.Checks)
    //    {
    //        var tx = new SimpleTransactionsModel
    //        {
    //            IdClient_fk = m.IdClient_fk,
    //            ReferenceNumber = check.ReferenceNumber,
    //            TransactionType = m.TransactionType,
    //            Company = check.Company,
    //            Amount = check.Amount,
    //            Commission = check.Commission,
    //            TotalAmount = check.TotalAmount,
    //            FixedCommission = check.FixedCommission,
    //            IssueDateCheck = check.IssueDateCheck,
    //            SenderName = m.SenderName,
    //            SenderDocumentType = m.SenderDocumentType,
    //            SenderDocumentNumber = m.SenderDocumentNumber,
    //            SenderPhone = m.SenderPhone,
    //            SenderAddress = m.SenderAddress,
    //            JustifyDetails = m.JustifyDetails,
    //            Justify_AgentName = m.Justify_AgentName,
    //            Justify_DateError = m.Justify_DateError,
    //            UserC = m.UserC
    //        };

    //        int idTransaction = await _repo.CreateSimple(tx);

    //        if (idTransaction <= 0)
    //        {
    //            throw new Exception(
    //                $"Could not create transaction {check.ReferenceNumber}"
    //            );
    //        }

    //        // ====================================
    //        // LINK ATTACHMENTS
    //        // ====================================
    //        foreach (var attach in attachments)
    //        {
    //            await _repoAttach.CreateAttachment(
    //                new TransactionAttachmentModel
    //                {
    //                    IdTransaction = idTransaction,
    //                    FileName = attach.FileName,
    //                    OriginalFileName = attach.OriginalFileName,
    //                    FileExtension = attach.FileExtension,
    //                    ContentType = attach.ContentType,
    //                    FilePath = attach.FilePath,
    //                    AttachmentType = attach.AttachmentType,
    //                    FileSize = attach.FileSize,
    //                    CreatedBy = attach.CreatedBy
    //                });
    //        }
    //    }
    //}
    public async Task CreateSimpleBatch(
     SimpleTransactionsBatchModel m,
     List<IFormFile> files)
    {
        if (m.Checks == null || !m.Checks.Any())
        {
            throw new Exception("No checks received");
        }

        // ====================================
        // GENERATE REAL REFERENCES
        // ====================================

        var generatedReferences = new Dictionary<string, long>();
        foreach (var check in m.Checks)
        {
            var preview = await _repoRefe.GetReferencePreview(check.Company, m.TransactionType);
            if (string.IsNullOrWhiteSpace(preview))
            {
                throw new Exception($"Could not generate reference for company {check.Company}");
            }
            var match = System.Text.RegularExpressions.Regex.Match(preview, @"^(.*)-(\d+)$");
            if (!match.Success)
            {
                throw new Exception($"Invalid reference format: {preview}");
            }
            string swiftCode = match.Groups[1].Value;
            long nextSequence = long.Parse(match.Groups[2].Value);
            string key = $"{check.Company}_{m.TransactionType}";
            if (generatedReferences.ContainsKey(key))
            {
                nextSequence = generatedReferences[key] + 1;
            }
            generatedReferences[key] = nextSequence;
            check.ReferenceNumber = $"{swiftCode}-{nextSequence:D8}";
        }

        // ====================================
        // VALIDATE DUPLICATED REFERENCES
        // ====================================

        var duplicatedReferences = m.Checks
            .Where(x => !string.IsNullOrWhiteSpace(x.ReferenceNumber))
            .GroupBy(x => x.ReferenceNumber.Trim())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedReferences.Any())
        {
            throw new Exception(
                $"Duplicated references detected: {string.Join(", ", duplicatedReferences)}"
            );
        }
        // ====================================
        // SAVE FILES ONCE
        // ====================================
        List<TransactionAttachmentModel> attachments = new();
        string batchReferenceImg = $"{m.Checks.First().ReferenceNumber}_{m.Checks.Last().ReferenceNumber}";

        if (files != null && files.Any())
        {
            foreach (var file in files)
            {
                string filePath =
                    SaveTransactionFile(file, m.TransactionType, m.SenderName, m.SenderDocumentNumber, batchReferenceImg);
                attachments.Add(
                    new TransactionAttachmentModel
                    {
                        FileName = Path.GetFileName(filePath),
                        OriginalFileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName),
                        ContentType = file.ContentType,
                        FilePath = filePath,
                        AttachmentType = "TRANSACTION_DOCUMENT",
                        FileSize = file.Length,
                        CreatedBy = m.UserC
                    });
            }
        }

        // ====================================
        // CREATE EACH CHECK
        // ====================================

        foreach (var check in m.Checks)
        {
            var tx =
                new SimpleTransactionsModel
                {
                    IdClient_fk = m.IdClient_fk,
                    ReferenceNumber = check.ReferenceNumber,
                    TransactionType = m.TransactionType,
                    Company = check.Company,
                    Amount = check.Amount,
                    Commission = check.Commission,
                    TotalAmount = check.TotalAmount,
                    FixedCommission = check.FixedCommission,
                    IssueDateCheck = check.IssueDateCheck,
                    SenderName = m.SenderName,
                    SenderDocumentType = m.SenderDocumentType,
                    SenderDocumentNumber = m.SenderDocumentNumber,
                    SenderPhone = m.SenderPhone,
                    SenderAddress = m.SenderAddress,
                    JustifyDetails = m.JustifyDetails,
                    Justify_AgentName = m.Justify_AgentName,
                    Justify_DateError = m.Justify_DateError,
                    CalculationMode = check.CalculationMode,
                    UserC = m.UserC
                };
            int idTransaction = await _repo.CreateSimple(tx);

            if (idTransaction <= 0)
            {
                throw new Exception(
                    $"Could not create transaction {check.ReferenceNumber}"
                );
            }

            // ====================================
            // LINK ATTACHMENTS
            // ====================================

            foreach (var attach in attachments)
            {
                await _repoAttach.CreateAttachment(
                    new TransactionAttachmentModel
                    {
                        IdTransaction = idTransaction,
                        FileName = attach.FileName,
                        OriginalFileName = attach.OriginalFileName,
                        FileExtension = attach.FileExtension,
                        ContentType = attach.ContentType,
                        FilePath = attach.FilePath,
                        AttachmentType = attach.AttachmentType,
                        FileSize = attach.FileSize,
                        CreatedBy = attach.CreatedBy
                    });
            }
            foreach (var item in generatedReferences)
            {
                var parts = item.Key.Split('_');
                int company = int.Parse(parts[0]);
                int transactionType = int.Parse(parts[1]);
                long lastSequence = item.Value;
                await _repoRefe.SaveReferenceSequence(company, transactionType, lastSequence);
            }
        }
    }
    private long ExtractSequence(string referenceNumber)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            referenceNumber,
            @"-(\d+)$");

        if (!match.Success)
            throw new Exception(
                $"Invalid reference format: {referenceNumber}");

        return long.Parse(match.Groups[1].Value);
    }
    //public async Task AddEvidence(int idTransaction, List<IFormFile> files, string user, int transactionType, string clientName, string clientDocument, string referenceNumber)
    //{
    //    if (files == null || !files.Any())
    //        throw new Exception("No files selected.");

    //    foreach (var file in files)
    //    {
    //        string filePath = SaveTransactionFile(file, transactionType, clientName, clientDocument, referenceNumber);

    //        await _repoAttach.CreateAttachment(
    //            new TransactionAttachmentModel
    //            {
    //                IdTransaction = idTransaction,
    //                FileName = Path.GetFileName(filePath),
    //                OriginalFileName = file.FileName,
    //                FileExtension = Path.GetExtension(file.FileName),
    //                ContentType = file.ContentType,
    //                FilePath = filePath,
    //                AttachmentType = "ADDITIONAL_EVIDENCE",
    //                FileSize = file.Length,
    //                CreatedBy = user
    //            });
    //    }
    //}

    public async Task AddEvidence(int idTransaction, List<IFormFile> files, string user)
    {
        if (files == null || !files.Any())
            throw new Exception("No files selected.");
        var transaction = await _repo.GetById(idTransaction);
        if (transaction == null)
            throw new Exception("Transaction not found.");
        foreach (var file in files)
        {
            string filePath = SaveTransactionFile(file, transaction.TransactionType, transaction.SenderName, transaction.SenderDocumentNumber, transaction.ReferenceNumber);
            await _repoAttach.CreateAttachment(
                new TransactionAttachmentModel
                {
                    IdTransaction = idTransaction,
                    FileName = Path.GetFileName(filePath),
                    OriginalFileName = file.FileName,
                    FileExtension = Path.GetExtension(file.FileName),
                    ContentType = file.ContentType,
                    FilePath = filePath,
                    AttachmentType = "ADDITIONAL_EVIDENCE",
                    FileSize = file.Length,
                    CreatedBy = user
                });
        }
    }

}