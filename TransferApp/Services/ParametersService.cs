using TransferApp.Models;
using TransferApp.ViewModels;

public class ParametersService
{
    private readonly ParametersRepository _repo;
    public ParametersService(ParametersRepository repo)
    {
        _repo = repo;
    }
    public async Task<List<ParametersModel>> GetAll()
    {
        return await _repo.GetAll();
    }
    public async Task<ParametersModel> GetById(int id)
    {
        return await _repo.GetById(id);
    }
    public async Task<int> Create(ParametersModel m)
    {
        return await _repo.Create(m);
    }
    public async Task Update(ParametersModel m)
    {
        await _repo.Update(m);
    }
    public async Task Delete(int id, string user)
    {
        await _repo.Delete(id, user);
    }

    public async Task<TransactionValidationModel> ValidateClientTransactions(string documentNumber)
    {
        return await _repo.ValidateClientTransactions(documentNumber);
    }
}