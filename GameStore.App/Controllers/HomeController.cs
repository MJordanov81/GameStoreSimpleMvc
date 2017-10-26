namespace GameStore.App.Controllers
{
    using GameStore.App.Data.Models;
    using GameStore.App.Models.Games;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class HomeController : BaseController
    {
        [Inject]
        private readonly IGameService games;

        [Inject]
        private readonly IUserService users;

        public IActionResult Index()
        {
            return this.IndexFiltered();
        }

        public IActionResult IndexFiltered(string filter = "")
        {
            int userId = 0;

            if (this.User.IsAuthenticated)
            {
                userId = users.GetUserId(this.User.Name);
            }

            IEnumerable<GameHomePageListingModel> games = this.games.GetFilteredGames(userId, filter);

            StringBuilder result = new StringBuilder();

            result.AppendLine(@"<div class=""card - group"">");

            foreach (GameHomePageListingModel game in games)
            {
                result.AppendLine(@"

                
                    <div class=""card col-4 thumbnail"">

                        <img class=""card -image-top img-fluid img-thumbnail""
                             onerror =""this.src='https://i.ytimg.com/vi/BqJyluskTfM/maxresdefault.jpg';""" +
                             $"src =\"{game.ThumbnailUrl}\" >" +

                        @"<div class=""card -body"">");

                result.AppendLine(
                            $"<div class=\"card - body\">" +
                            $"<h4 class=\"card-title\">{game.Title}</h4>" +
                            $"<p class=\"card-text\"><strong>Price</strong> - {game.Price}&euro;</p>" +
                            $"<p class=\"card-text\"><strong>Size</strong> - {game.Size} GB</p>" +
                            $"<p class=\"card-text\">{game.Description}</p>" +
                        $"</div>");

                result.AppendLine($"<div class=\"card - footer\" style=\"d - inline\">" +
                            $"<div >" +
                                $"<a style = \"display: {{{{{{adminDisplay}}}}}}; margin: 2px\" class=\"card-button btn btn-warning\" name=\"edit\" href=\"#\">Edit</a>" +
                                $"<a style = \"display: {{{{{{adminDisplay}}}}}}; margin 2px\" class=\"card-button btn btn-danger\" name=\"delete\" href=\"#\">Delete</a>" +
                                $"<a style=\"margin: 2px\" class=\"card-button btn btn-outline-primary\" name=\"info\" href=\"#\">Info</a>" +
                                $"<a style=\"margin: 2px\" class=\"card-button btn btn-primary\" name=\"buy\" href=\"#\">Buy</a>" +
                            $"</div>" +
                        $"</div>");
            }

            result.AppendLine(@"</div>");

            this.ViewModel["result"] = result.ToString().Trim();
            return View(@"index");
        }
    }
}
