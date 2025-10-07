using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Validation
{
    public enum ValidationStatus
    {
        Success,
        NotFound,
        BadRequest,
    }
    public class ValidationResult
    {
        public string ErrorMessage { get; }
        public ValidationStatus Status { get; }

        public ValidationResult(ValidationStatus status, string errorMessage = "")
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }
    }
}
