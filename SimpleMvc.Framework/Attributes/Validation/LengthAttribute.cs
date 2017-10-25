namespace SimpleMvc.Framework.Attributes.Validation
{
    public class LengthAttribute : PropertyValidationAttribute
    {
        private readonly int minLength;

        private readonly int maxLength;

        public LengthAttribute(int maxLength, int minLength = 0)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            string inputString = value as string;

            if (inputString == null)
            {
                return true;
            }

            return minLength <= inputString.Length && inputString.Length <= maxLength;
        }
    }
}
