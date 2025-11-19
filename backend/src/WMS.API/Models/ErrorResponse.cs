namespace WMS.API.Models
{
    /// <summary>
    /// Resposta padronizada para erros da API
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Código de status HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dicionário de erros de validação (campo: [lista de erros])
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; } = new();

        /// <summary>
        /// Timestamp do erro em UTC
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identificador único da requisição para logging/rastreamento
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ErrorResponse()
        {
        }

        /// <summary>
        /// Construtor com mensagem
        /// </summary>
        public ErrorResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        /// <summary>
        /// Construtor com mensagem e erros de validação
        /// </summary>
        public ErrorResponse(int statusCode, string message, Dictionary<string, string[]> errors)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }
    }
}
