namespace GameStore.App.Controllers
{
    using Data;
    using Data.Models;
    using SimpleMvc.Framework.Controllers;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.ViewModel["anonymousDisplay"] = "flex";
            this.ViewModel["userDisplay"] = "none";
            this.ViewModel["adminDisplay"] = "none";
            this.ViewModel["show-error"] = "none";
        }

        protected void ShowError(string error)
        {
            this.ViewModel["show-error"] = "block";
            this.ViewModel["error"] = error;
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["anonymousDisplay"] = "none";
                this.ViewModel["userDisplay"] = "flex";

                using (var db = new GameStoreDbContext())
                {
                    User user = db
                        .Users
                        .First(u => u.Email == this.User.Name);

                    if (user.IsAdmin)
                    {
                        this.ViewModel["userDisplay"] = "none";
                        this.ViewModel["adminDisplay"] = "flex";
                    }
                }
            }
        }
    }
}
