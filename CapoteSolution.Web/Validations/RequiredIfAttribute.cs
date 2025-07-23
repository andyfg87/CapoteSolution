using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Validations
{
    public class RequiredIfAttribute: ValidationAttribute
    {
        private string _dependentProperty;
        private object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var dependentValue = type.GetProperty(_dependentProperty)?.GetValue(instance);

            if (dependentValue?.ToString() == _targetValue?.ToString() && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
