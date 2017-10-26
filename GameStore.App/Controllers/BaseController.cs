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
            this.ViewModel["anonymousDisplay"] = "inline-block";
            this.ViewModel["userDisplay"] = "none";
            this.ViewModel["adminDisplay"] = "none";
            this.ViewModel["show-error"] = "none";
        }

        protected void ShowError(string error)
        {
            this.ViewModel["show-error"] = "inline-block";
            this.ViewModel["error"] = error;
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            if (this.User.IsAuthenticated)
            {
                this.ViewModel["userDisplay"] = "inline-block";
                this.ViewModel["anonymousDisplay"] = "none";

                if (this.User.IsAdmin)
                {
                    this.ViewModel["adminDisplay"] = "inline-block";
                }
            }
        }
    }
}
