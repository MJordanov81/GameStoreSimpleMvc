namespace WebServer.Common
{
    using Contracts;

    public class ForbiddenView : IView
    {
        public string View()
        {
            return "<h1>403 You don't have authorization to access this page! :/</h1>";
        }
    }
}
