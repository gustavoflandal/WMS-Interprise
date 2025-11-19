namespace WMS.Application.DTOs.Responses;

public record CustomerResponse(
    Guid Id,
    string Nome,
    string Tipo,
    string? NumeroDocumento,
    string? Email,
    string? Telefone,
    int Status,
    string StatusDescricao,
    Guid? CriadoPor,
    Guid? AtualizadoPor,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
