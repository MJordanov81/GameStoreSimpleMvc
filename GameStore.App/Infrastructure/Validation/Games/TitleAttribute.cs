namespace GameStore.App.Infrastructure.Validation.Games
{
    using SimpleMvc.Framework.Attributes.Validation;

    /// <summary>
    /// Checks if a string starts with a capital letter.
    /// </summary>
    public class TitleAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var title = value as string;
            if (title == null)
            {
                return true;
            }

            return char.IsUpper(title[0]);
        }
    }
}
