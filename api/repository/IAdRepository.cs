using System.Collections.Generic;
using System.Threading.Tasks;
using api.model;

namespace api.repository;

public interface IAdRepository
{
	Task<List<Ad>> GetAllAsync();
	Task<List<Ad>> GetAllForUserAsync(string userId);
	Task<List<Ad>> GetAllUnapprovedAsync();
	Task<Ad?> GetByIdAsync(string id);
	Task<Ad?> CreateAsync(Ad ad);
	Task<Ad?> UpdateAsync(Ad ad);
	Task<Ad?> DeleteAsync(string id);
	Task<Ad?> ChangeApprovalStatusAsync(string id, AdApprovalStatus status);
	Task<Ad?> ChangeAdStatusAsync(string id, AdStatus newStatus);
}