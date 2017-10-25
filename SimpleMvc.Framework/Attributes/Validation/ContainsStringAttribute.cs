namespace SimpleMvc.Framework.Attributes.Validation
{
    public class ContainsStringAttribute : PropertyValidationAttribute
    {
        private readonly string[] strings;

        public ContainsStringAttribute(string[] strings)
        {
            this.strings = strings;
        }

        public override bool IsValid(object value)
        {
            string inputString = value as string;

            if (inputString == null)
            {
                return true;
            }

            foreach (string item in this.strings)
            {
                if (!inputString.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
