using TransferApp.Models;

public class ClientCompaniesService
{
    private readonly ClientCompaniesRepository _repo;

    public ClientCompaniesService(
        ClientCompaniesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ClientCompaniesModel>> GetAll()
        => await _repo.GetAll();

    public async Task<int> Create(ClientCompaniesModel m)
    {
        return await _repo.Create(m);
    }

    public async Task Delete(int id, string user)
    {
        await _repo.Delete(id, user);
    }
    public async Task<List<CompaniesModel>> GetPaidServiceCompaniesByClient(int clientId)
    {
        return await _repo.GetPaidServiceCompaniesByClient(clientId);
    }

}