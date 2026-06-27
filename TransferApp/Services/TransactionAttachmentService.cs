using TransferApp.Models;
using TransferApp.Repositories;

namespace TransferApp.Services
{
    public class TransactionAttachmentService
    {
        private readonly TransactionAttachmentRepository _repo;

        public TransactionAttachmentService(TransactionAttachmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TransactionAttachmentModel>> GetAttachments(int idTransaction)
        {
            return await _repo.GetAttachments(idTransaction);
        }

        public async Task<TransactionAttachmentModel?> GetAttachmentById(long id)
        {
            return await _repo.GetAttachmentById(id);
        }

        public async Task<long> CreateAttachment(TransactionAttachmentModel m)
        {
            return await _repo.CreateAttachment(m);
        }
    }
}
