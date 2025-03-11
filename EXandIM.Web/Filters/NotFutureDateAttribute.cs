using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Filters
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime bookDate = Convert.ToDateTime(value);

            if (bookDate > DateTime.Today)
            {
                return new ValidationResult("لا يمكن أن يكون التاريخ في المستقبل!");
            }

            return ValidationResult.Success;
        }
    }
}
