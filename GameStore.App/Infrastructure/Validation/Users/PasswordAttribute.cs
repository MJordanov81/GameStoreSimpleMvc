namespace GameStore.App.Infrastructure.Validation.Users
{
    using SimpleMvc.Framework.Attributes.Validation;
    using System.Linq;

    /// <summary>
    /// Checks if string contains at least one upper case character, one lower case character and one digit.
    /// </summary>
    public class PasswordAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;
            if (password == null)
            {
                return true;
            }

            return password.Any(s => char.IsDigit(s))
                && password.Any(s => char.IsUpper(s))
                && password.Any(s => char.IsLower(s));
        }
    }
}
