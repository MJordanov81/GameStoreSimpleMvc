namespace GameStore.App.Infrastructure.Validation.Games
{
    using SimpleMvc.Framework.Attributes.Validation;

    /// <summary>
    /// Checks if the string starts with 'http://' or 'https://'.
    /// </summary>
    public class ThumbnailUrlAttribute : PropertyValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var thumbnailUrl = value as string;
            if (thumbnailUrl == null)
            {
                return true;
            }

            return thumbnailUrl.StartsWith("http://")
                || thumbnailUrl.StartsWith("https://");
        }
    }
}
