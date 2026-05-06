using TransferApp.Models;

public class TransactionsService
{
    private readonly TransactionsRepository _repo;

    public TransactionsService(TransactionsRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TransactionsModel>> GetAll() => await _repo.GetAll();
    public async Task Create(TransactionsModel m) => await _repo.Create(m);
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
}