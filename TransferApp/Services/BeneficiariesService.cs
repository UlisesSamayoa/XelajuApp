using TransferApp.Models;

public class BeneficiariesService
{
    private readonly BeneficiariesRepository _repo;

    public BeneficiariesService(BeneficiariesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BeneficiariesModel>> GetAll() => await _repo.GetAll();
    public async Task<BeneficiariesModel> GetById(int id) => await _repo.GetById(id);
    //public async Task Create(BeneficiariesModel m) => await _repo.Create(m);
    public async Task<int> Create(BeneficiariesModel m)
    {
        return await _repo.Create(m);
    }
    public async Task Update(BeneficiariesModel m) => await _repo.Update(m);
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
    public async Task<List<BeneficiariesModel>> GetByClient(int clientId)
    {
        if (clientId <= 0)
            throw new Exception("Invalid client");

        return await _repo.GetByClient(clientId);
    }

    public async Task<bool> ValidateBeneficiaryByClient(
    int idBeneficiarie,
    int idClient)
    {
        return await _repo.ValidateBeneficiaryByClient(idBeneficiarie,idClient);
    }
}