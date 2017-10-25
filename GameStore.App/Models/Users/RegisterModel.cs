namespace GameStore.App.Models.Users
{
    using Infrastructure.Validation;
    using Infrastructure.Validation.Users;
    using SimpleMvc.Framework.Attributes.Validation;

    public class RegisterModel
    {
        [Required]
        [ContainsString(new[] { ".", "@"})]
        public string Email { get; set; }
        
        public string Name { get; set; }

        [Required]
        [Password]
        [Length(minLength: 6, maxLength: 50)]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
