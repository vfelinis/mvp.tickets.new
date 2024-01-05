using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Models;

namespace mvp.tickets.domain.Extensions
{
    public static class BaseResponseExtension
    {
        public static void HandleException(this IBaseResponse response, Exception ex)
        {
            response.IsSuccess = false;
            response.Code = ResponseCodes.Error;
            response.ErrorMessage = $"{ex.Message}. {ex.InnerException?.Message}";
        }
    }
}
