using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Interfaces;
using WMS.Domain.Entities;
using WMS.Shared.Interfaces;

namespace WMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetWithRolesAsync(id, cancellationToken);

        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found");
        }

        var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>();
        var response = new UserResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Phone: user.Phone,
            IsActive: user.IsActive,
            EmailConfirmed: user.EmailConfirmed,
            LastLoginAt: user.LastLoginAt,
            CreatedAt: user.CreatedAt,
            Roles: roles,
            Permissions: new List<string>()
        );

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var responses = users.Select(u => new UserResponse(
            Id: u.Id,
            Username: u.Username,
            Email: u.Email,
            FirstName: u.FirstName,
            LastName: u.LastName,
            Phone: u.Phone,
            IsActive: u.IsActive,
            EmailConfirmed: u.EmailConfirmed,
            LastLoginAt: u.LastLoginAt,
            CreatedAt: u.CreatedAt,
            Roles: new List<string>(),
            Permissions: new List<string>()
        ));

        return Result<IEnumerable<UserResponse>>.Success(responses);
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        // Validate
        if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
        {
            return Result<UserResponse>.Failure("Username already exists");
        }

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result<UserResponse>.Failure("Email already exists");
        }

        // Hash password
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<UserResponse>.Failure("Password is required");
        }

        var passwordHash = _passwordService.HashPassword(request.Password);

        var newUser = new User(
            username: request.Username,
            email: request.Email,
            passwordHash: passwordHash,
            firstName: request.FirstName ?? "",
            lastName: request.LastName ?? "",
            phone: request.Phone
        );

        await _unitOfWork.Users.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UserResponse(
            Id: newUser.Id,
            Username: newUser.Username,
            Email: newUser.Email,
            FirstName: newUser.FirstName,
            LastName: newUser.LastName,
            Phone: newUser.Phone,
            IsActive: newUser.IsActive,
            EmailConfirmed: newUser.EmailConfirmed,
            LastLoginAt: newUser.LastLoginAt,
            CreatedAt: newUser.CreatedAt,
            Roles: new List<string>(),
            Permissions: new List<string>()
        );

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found");
        }

        // Atualizar perfil se os campos foram fornecidos
        if (request.FirstName != null || request.LastName != null || request.Phone != null)
        {
            user.UpdateProfile(
                firstName: request.FirstName ?? user.FirstName,
                lastName: request.LastName ?? user.LastName,
                phone: request.Phone ?? user.Phone
            );
        }

        // Atualizar status de ativação
        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
            {
                user.Activate();
            }
            else
            {
                user.Deactivate();
            }
        }

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UserResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Phone: user.Phone,
            IsActive: user.IsActive,
            EmailConfirmed: user.EmailConfirmed,
            LastLoginAt: user.LastLoginAt,
            CreatedAt: user.CreatedAt,
            Roles: new List<string>(),
            Permissions: new List<string>()
        );

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        await _unitOfWork.Users.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<UserResponse>> RestoreAsync(Guid id, string restoredBy, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken, includeDeleted: true);
        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found");
        }

        if (!user.IsDeleted)
        {
            return Result<UserResponse>.Failure("User is not deleted");
        }

        user.Restore(restoredBy);
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UserResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Phone: user.Phone,
            IsActive: user.IsActive,
            EmailConfirmed: user.EmailConfirmed,
            LastLoginAt: user.LastLoginAt,
            CreatedAt: user.CreatedAt,
            Roles: user.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>(),
            Permissions: new List<string>()
        );

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        var deletedUsers = await _unitOfWork.Users.GetDeletedAsync(cancellationToken);

        var response = deletedUsers.Select(u => new UserResponse(
            Id: u.Id,
            Username: u.Username,
            Email: u.Email,
            FirstName: u.FirstName,
            LastName: u.LastName,
            Phone: u.Phone,
            IsActive: u.IsActive,
            EmailConfirmed: u.EmailConfirmed,
            LastLoginAt: u.LastLoginAt,
            CreatedAt: u.CreatedAt,
            Roles: u.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>(),
            Permissions: new List<string>()
        ));

        return Result<IEnumerable<UserResponse>>.Success(response);
    }

    public async Task<Result> AssignRoleAsync(Guid userId, Guid roleId, string assignedBy, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            return Result.Failure("Role not found");
        }

        // Check if user already has this role
        if (user.UserRoles?.Any(ur => ur.RoleId == roleId) == true)
        {
            return Result.Failure("User already has this role");
        }

        user.AddRole(role);
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveRoleAsync(Guid userId, Guid roleId, string removedBy, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            return Result.Failure("Role not found");
        }

        user.RemoveRole(roleId);
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

