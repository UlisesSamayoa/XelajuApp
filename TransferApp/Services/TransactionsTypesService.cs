using TransferApp.Models;

public class TransactionsTypesService
{
    private readonly TransactionsTypesRepository _repo;

    public TransactionsTypesService(TransactionsTypesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TransactionsTypesModel>> GetAll() => await _repo.GetAll();
    //public async Task<TransactionsTypesModel> GetById(int id) => await _repo.GetById(id);
    public async Task<TransactionsTypesModel> GetById(int id)
    {
        var transactionType = await _repo.GetById(id);
        if (transactionType == null)
        {
            return null;
        }
        transactionType.CommissionRanges = await _repo.GetCommissionRanges(transactionType.NumberT);
        return transactionType;
    }
    //public async Task<List<TransactionsTypesModel>> GetByNumber(int id) => await _repo.GetByNumber(id);
    public async Task<List<TransactionsTypesModel>> GetByNumber(int id)
    {
        var list = await _repo.GetByNumber(id);
        foreach (var item in list)
        {
            item.CommissionRanges = await _repo.GetCommissionRanges(item.NumberT);
        }
        return list;
    }
    //public async Task Create(TransactionsTypesModel m) => await _repo.Create(m);
    public async Task Create(TransactionsTypesModel m)
    {
        int idTypeTransaction = await _repo.Create(m);
        if (idTypeTransaction <= 0)
        {
            throw new Exception(
                "Could not create transaction type"
            );
        }
        if (m.CommissionRanges != null && m.CommissionRanges.Any())
        {
            foreach (var range in m.CommissionRanges)
            {
                range.IdTypeTransaction = idTypeTransaction;
                range.UserC = m.UserC;
                range.NumberT = m.NumberT;
                await _repo.CreateCommissionRange(range);
            }
        }
    }
    //public async Task Update(TransactionsTypesModel m) => await _repo.Update(m);
    public async Task Update(TransactionsTypesModel m)
    {
        await _repo.Update(m);
        await _repo.DeleteCommissionRanges(m.NumberT);
        if (m.CommissionRanges != null && m.CommissionRanges.Any())
        {
            foreach (var range in m.CommissionRanges)
            {
                range.NumberT = m.NumberT;
                range.UserC = m.UserU;

                await _repo.CreateCommissionRange(range);
            }
        }
    }
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
    public async Task<List<TransactionsTypesModel>> GetAllTypes()
    {
        return await _repo.GetAllTypes();
    }
}