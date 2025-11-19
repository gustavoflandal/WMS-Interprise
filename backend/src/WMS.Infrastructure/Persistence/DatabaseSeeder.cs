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

                new("companies.view", "Companies", "View", "Permissão para visualizar dados da empresa", "Administration"),
                new("companies.create", "Companies", "Create", "Permissão para criar empresa", "Administration"),
                new("companies.edit", "Companies", "Edit", "Permissão para editar dados da empresa", "Administration"),
                new("companies.delete", "Companies", "Delete", "Permissão para deletar empresa", "Administration"),

                new("warehouses.view", "Warehouses", "View", "Permissão para visualizar armazéns", "Administration"),
                new("warehouses.create", "Warehouses", "Create", "Permissão para criar armazém", "Administration"),
                new("warehouses.edit", "Warehouses", "Edit", "Permissão para editar armazém", "Administration"),
                new("warehouses.delete", "Warehouses", "Delete", "Permissão para deletar armazém", "Administration"),

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

            // 8. Criar empresa de teste
            var testCompany = new Company(
                tenantId: defaultTenant.Id,
                razaoSocial: "WMS Interprise Logística Ltda",
                cnpj: "12.345.678/0001-90",
                email: "contato@wmsinterprise.com.br",
                cep: "01310-100",
                logradouro: "Avenida Paulista",
                numero: "1578",
                bairro: "Bela Vista",
                cidade: "São Paulo",
                estado: "SP",
                nomeResponsavel: "João Silva",
                cpfResponsavel: "123.456.789-00",
                emailResponsavel: "joao.silva@wmsinterprise.com.br"
            );

            testCompany.UpdateInfo(
                nomeFantasia: "WMS Interprise",
                inscricaoEstadual: "123.456.789.012",
                inscricaoMunicipal: "987654321",
                email: "contato@wmsinterprise.com.br",
                telefone: "(11) 3456-7890",
                celular: "(11) 98765-4321",
                website: "https://www.wmsinterprise.com.br",
                cep: "01310-100",
                logradouro: "Avenida Paulista",
                numero: "1578",
                complemento: "Conjunto 1501",
                bairro: "Bela Vista",
                cidade: "São Paulo",
                estado: "SP",
                dataAbertura: new DateTime(2020, 1, 15),
                capitalSocial: 500000.00m,
                atividadePrincipal: "6204-0/00 - Consultoria em tecnologia da informação",
                regimeTributario: "Lucro Presumido",
                nomeResponsavel: "João Silva",
                cpfResponsavel: "123.456.789-00",
                cargoResponsavel: "Diretor Executivo",
                emailResponsavel: "joao.silva@wmsinterprise.com.br",
                telefoneResponsavel: "(11) 99876-5432"
            );

            testCompany.UpdateTimestamp("system");
            await _dbContext.Companies.AddAsync(testCompany);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Test company created (CNPJ: 12.345.678/0001-90)");

            // 9. Criar armazém de teste
            var testWarehouse = new Warehouse(
                tenantId: defaultTenant.Id,
                nome: "Armazém Principal - São Paulo",
                codigo: "SP-01",
                descricao: "Armazém principal localizado em São Paulo, com capacidade para 10.000 posições"
            );

            testWarehouse.UpdateInfo(
                nome: "Armazém Principal - São Paulo",
                descricao: "Armazém principal localizado em São Paulo, com capacidade para 10.000 posições",
                endereco: "Rua dos Galpões Industriais, 1000",
                cidade: "São Paulo",
                estado: "SP",
                cep: "08295-005",
                pais: "BRA",
                latitude: -23.4901m,
                longitude: -46.5693m,
                totalPosicoes: 10000,
                capacidadePesoTotal: 500000.00m, // 500 toneladas
                horarioAbertura: new TimeSpan(6, 0, 0), // 06:00
                horarioFechamento: new TimeSpan(22, 0, 0), // 22:00
                maxTrabalhadores: 50
            );

            testWarehouse.UpdateTimestamp("system");
            await _dbContext.Warehouses.AddAsync(testWarehouse);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ Test warehouse created (Code: SP-01)");

            // 10. Criar clientes de teste
            var customers = new List<Customer>
            {
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Empresa ABC Comércio Ltda",
                    tipo: "PJ",
                    numeroDocumento: "12.345.678/0001-90",
                    email: "contato@abccomercio.com.br",
                    telefone: "(11) 3456-7890"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Indústria XYZ Manufatura S/A",
                    tipo: "PJ",
                    numeroDocumento: "98.765.432/0001-12",
                    email: "vendas@xyzmanufatura.com.br",
                    telefone: "(31) 2588-9900"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Logística Global Transportes Ltda",
                    tipo: "PJ",
                    numeroDocumento: "55.444.333/0001-44",
                    email: "logistica@globaltransportes.com.br",
                    telefone: "(21) 3334-5555"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Varejo Premium Distribuidora",
                    tipo: "PJ",
                    numeroDocumento: "77.888.999/0001-55",
                    email: "compras@varejopremium.com.br",
                    telefone: "(85) 3333-4444"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Fernando Silva Santos",
                    tipo: "PF",
                    numeroDocumento: "123.456.789-00",
                    email: "fernando.silva@email.com",
                    telefone: "(11) 99999-8888"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Tech Solutions Brasil Consultoria",
                    tipo: "PJ",
                    numeroDocumento: "11.222.333/0001-66",
                    email: "info@techsolutions.com.br",
                    telefone: "(11) 2222-3333"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "E-commerce Virtual Store Ltda",
                    tipo: "PJ",
                    numeroDocumento: "44.555.666/0001-77",
                    email: "suporte@ecommercestore.com.br",
                    telefone: "(47) 3344-5566"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Marisa Silva Costa",
                    tipo: "PF",
                    numeroDocumento: "987.654.321-11",
                    email: "marisa.costa@email.com",
                    telefone: "(21) 98888-7777"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "Alimentos Premium Distribuição",
                    tipo: "PJ",
                    numeroDocumento: "33.666.999/0001-88",
                    email: "pedidos@alimentospremium.com.br",
                    telefone: "(41) 3333-4444"
                ),
                new Customer(
                    tenantId: defaultTenant.Id,
                    nome: "João Pedro Oliveira Ferreira",
                    tipo: "PF",
                    numeroDocumento: "456.789.123-22",
                    email: "joao.ferreira@email.com",
                    telefone: "(62) 99999-1111"
                )
            };

            await _dbContext.Customers.AddRangeAsync(customers);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("✅ 10 test customers created");

            _logger.LogInformation("✅ Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error seeding database");
            throw;
        }
    }
}
