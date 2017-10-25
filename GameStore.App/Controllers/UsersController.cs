namespace GameStore.App.Controllers
{
    using Models.Users;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes;
    using SimpleMvc.Framework.Attributes.Authentication;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using StaticData.Constants;

    public class UsersController : BaseController
    {
        [Inject]
        private readonly IUserService users;

        public IActionResult Register() => this.View();

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword
                || !this.IsValidModel(model))
            {
                this.ShowError(ErrorMessages.RegisterError);
                return this.View();
            }

            bool isRegistrationSuccessful = this.users.Create(
                model.Email,
                model.Password,
                model.Name);

            if (isRegistrationSuccessful)
            {
                return this.Redirect("/users/login");
            }
            else
            {
                this.ShowError(ErrorMessages.EmailExistsError);
                return this.View();
            }
        }

        public IActionResult Login() => this.View();

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorMessages.LoginError);
                return this.View();
            }

            if (this.users.UserExists(model.Email, model.Password))
            {
                bool isAdmin = this.users.IsAdmin(model.Email);

                this.SignIn(model.Email, isAdmin);
                return this.Redirect("/");
            }
            else
            {
                this.ShowError(ErrorMessages.LoginError);
                return this.View();
            }
        }

        [Authenticated]
        public IActionResult Logout()
        {
            this.SignOut();
            return this.Redirect("/");
        }
    }
}
