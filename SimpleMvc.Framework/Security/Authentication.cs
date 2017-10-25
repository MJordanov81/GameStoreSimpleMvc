namespace SimpleMvc.Framework.Security
{
    public class Authentication
    {
        internal Authentication()
        {
            this.IsAuthenticated = false;
            this.IsAdmin = false;
        }

        internal Authentication(string name, bool isAdmin)
        {
            this.IsAuthenticated = true;
            this.IsAdmin = isAdmin;
            this.Name = name;
        }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public string Name { get; private set; }
    }
}
