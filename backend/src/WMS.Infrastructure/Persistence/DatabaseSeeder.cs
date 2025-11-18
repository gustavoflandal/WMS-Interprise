using WMS.Domain.Entities;
using WMS.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace WMS.Infrastructure.Persistence;

/// <summary>
/// Seeder para dados iniciais do sistema
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext dbContext,
        IPasswordService passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Executa o seed de dados iniciais
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Verificar se já existem tenants
            if (await _dbContext.Tenants.AnyAsync())
            {
                _logger.LogInformation("Database already seeded, skipping seed data");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // 1. Criar Tenant padrão
            var defaultTenant = new Tenant(
                name: "WMS Default",
                slug: "wms-default",
                contactEmail: "admin@wms.local"
            );
            await _dbContext.Tenants.AddAsync(defaultTenant);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Default tenant created");

            // 2. Criar Roles
            var adminRole = new Role(
                name: "Admin",
                description: "Administrador do sistema com acesso total",
                tenantId: defaultTenant.Id
            );

            var userRole = new Role(
                name: "User",
                description: "Usuário padrão do sistema",
                tenantId: defaultTenant.Id
            );

            var warehouseManagerRole = new Role(
                name: "WarehouseManager",
                description: "Gerente de warehouse",
                tenantId: defaultTenant.Id
            );

            await _dbContext.Roles.AddRangeAsync(adminRole, userRole, warehouseManagerRole);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Roles created");

            // 3. Criar Permissions
            var permissions = new List<Permission>
            {
                new("users.view", "Users", "View", "Permissão para visualizar lista de usuários", "Administration"),
                new("users.create", "Users", "Create", "Permissão para criar novo usuário", "Administration"),
                new("users.edit", "Users", "Edit", "Permissão para editar usuário", "Administration"),
                new("users.delete", "Users", "Delete", "Permissão para deletar usuário", "Administration"),

                new("roles.view", "Roles", "View", "Permissão para visualizar papéis", "Administration"),
                new("roles.create", "Roles", "Create", "Permissão para criar novo papel", "Administration"),
                new("roles.edit", "Roles", "Edit", "Permissão para editar papel", "Administration"),
                new("roles.delete", "Roles", "Delete", "Permissão para deletar papel", "Administration"),

                new("inventory.view", "Inventory", "View", "Permissão para visualizar inventário", "Inventory"),
                new("inventory.manage", "Inventory", "Manage", "Permissão para gerenciar inventário", "Inventory"),

                new("orders.view", "Orders", "View", "Permissão para visualizar pedidos", "Orders"),
                new("orders.create", "Orders", "Create", "Permissão para criar pedido", "Orders"),
                new("orders.manage", "Orders", "Manage", "Permissão para gerenciar pedidos", "Orders"),

                new("reports.view", "Reports", "View", "Permissão para visualizar relatórios", "Reports"),
                new("audit.view", "Audit", "View", "Permissão para visualizar logs de auditoria", "Administration"),
            };

            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Permissions created");

            // 4. Associar permissões ao role Admin
            foreach (var permission in permissions)
            {
                var rolePermission = new RolePermission(
                    roleId: adminRole.Id,
                    permissionId: permission.Id
                );
                await _dbContext.RolePermissions.AddAsync(rolePermission);
            }
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Admin permissions assigned");

            // 5. Criar usuário Admin
            var adminPasswordHash = _passwordHasher.HashPassword("Admin@123");
            var adminUser = new User(
                username: "admin",
                email: "admin@wms.local",
                passwordHash: adminPasswordHash,
                firstName: "Administrador",
                lastName: "Sistema",
                phone: "+55 11 9999-9999",
                tenantId: defaultTenant.Id
            );

            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Admin user created (username: admin, password: Admin@123)");

            // 6. Associar Admin ao role Admin
            var adminUserRole = new UserRole(
                userId: adminUser.Id,
                roleId: adminRole.Id
            );
            await _dbContext.UserRoles.AddAsync(adminUserRole);

            // 7. Criar usuários adicionais para teste
            var testPasswordHash = _passwordHasher.HashPassword("Test@123");
            var testUser = new User(
                username: "testuser",
                email: "test@wms.local",
                passwordHash: testPasswordHash,
                firstName: "Teste",
                lastName: "Usuário",
                phone: "+55 11 8888-8888",
                tenantId: defaultTenant.Id
            );

            await _dbContext.Users.AddAsync(testUser);
            await _dbContext.SaveChangesAsync();

            var testUserRole = new UserRole(
                userId: testUser.Id,
                roleId: userRole.Id
            );
            await _dbContext.UserRoles.AddAsync(testUserRole);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Test user created (username: testuser, password: Test@123)");

            _logger.LogInformation("✅ Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error seeding database");
            throw;
        }
    }
}
