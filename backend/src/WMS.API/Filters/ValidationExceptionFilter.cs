using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WMS.API.Models;

namespace WMS.API.Filters
{
    /// <summary>
    /// Filter que intercepta erros de validação de modelo e retorna uma resposta padronizada
    /// </summary>
    public class ValidationExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Coletar erros de validação
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => ToCamelCase(kvp.Key),
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new ErrorResponse(
                    statusCode: 400,
                    message: "Um ou mais erros de validação ocorreram",
                    errors: errors
                )
                {
                    TraceId = context.HttpContext.TraceIdentifier
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// Converte string para camelCase
        /// Ex: FirstName -> firstName
        /// </summary>
        private static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
                return str;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
