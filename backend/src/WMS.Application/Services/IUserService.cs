using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, string createdBy, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, string updatedBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> RestoreAsync(Guid id, string restoredBy, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserResponse>>> GetDeletedAsync(CancellationToken cancellationToken = default);
    Task<Result> AssignRoleAsync(Guid userId, Guid roleId, string assignedBy, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleAsync(Guid userId, Guid roleId, string removedBy, CancellationToken cancellationToken = default);
}
