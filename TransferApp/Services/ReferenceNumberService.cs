using TransferApp.Repositories;

namespace TransferApp.Services
{
    public class ReferenceNumberService
    {
        private readonly ReferenceNumberRepository _repo;
        private readonly CompanyRepository _repoCompany;

        public ReferenceNumberService(ReferenceNumberRepository repo)
        {
            _repo = repo;
        }
        public async Task<string> GetNextReferenceNumber(int senderCompany, int transactionType)
        {
            return await _repo.GetNextReferenceNumber(senderCompany, transactionType);
        }
        public async Task<string> GetReferencePreview(int companyId, int transactionType)
        {
            var company = await _repoCompany.GetById(companyId);
            if (company == null) throw new Exception("Company not found");
            if (company.TransactionType != transactionType) throw new Exception("The company does not belong to the selected transaction type");

            return $"{company.SwiftCode}-AUTO";
        }
    }
}
