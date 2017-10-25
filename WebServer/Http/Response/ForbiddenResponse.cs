namespace WebServer.Http.Response
{
    using Common;

    public class ForbiddenResponse : ContentResponse
    {
        public ForbiddenResponse()
            : base(Enums.HttpStatusCode.Forbidden, new ForbiddenView().View())
        {
        }
    }
}
