using TransferApp.Models;

public class TransactionsTypesService
{
    private readonly TransactionsTypesRepository _repo;

    public TransactionsTypesService(TransactionsTypesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TransactionsTypesModel>> GetAll() => await _repo.GetAll();
    public async Task<TransactionsTypesModel> GetById(int id) => await _repo.GetById(id);
    public async Task Create(TransactionsTypesModel m) => await _repo.Create(m);
    public async Task Update(TransactionsTypesModel m) => await _repo.Update(m);
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
}