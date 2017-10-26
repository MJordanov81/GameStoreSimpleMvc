namespace GameStore.App.Controllers
{
    using Data.Models;
    using Models.Games;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using StaticData.Constants;
    using System.Linq;

    public class GamesController : BaseController
    {
        [Inject]
        private readonly IGameService games;

        [Admin]
        public IActionResult Add()
        {
            return this.View();
        }

        [Admin]
        [HttpPost]
        public IActionResult Add(GameAdminModel model)
        {
            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorMessages.GameError);
                return this.View();
            }

            this.games.Create(
                model.Title,
                model.Description,
                model.ThumbnailUrl,
                model.Price,
                model.Size,
                model.VideoId,
                model.ReleaseDate);

            return this.Redirect("/games/all");
        }

        [Admin]
        public IActionResult Edit(int id)
        {
            var game = this.games.GetById(id);

            if (game == null)
            {
                return this.NotFound();
            }

            this.SetGameViewData(game);
            
            return this.View();
        }

        [Admin]
        [HttpPost]
        public IActionResult Edit(int id, GameAdminModel model)
        {

            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorMessages.GameError);
                return this.View();
            }
            
            this.games.Update(
                id,
                model.Title,
                model.Description,
                model.ThumbnailUrl,
                model.Price,
                model.Size,
                model.VideoId,
                model.ReleaseDate);

            return this.Redirect("/games/all");
        }

        [Admin]
        public IActionResult Delete(int id)
        {
            var game = this.games.GetById(id);

            if (game == null)
            {
                return this.NotFound();
            }

            this.ViewModel["id"] = id.ToString();
            this.SetGameViewData(game);

            return this.View();
        }

        [Admin]
        [HttpPost]
        public IActionResult Destroy(int id)
        {
            var game = this.games.GetById(id);

            if (game == null)
            {
                return this.NotFound();
            }

            this.games.Delete(id);

            return this.Redirect("/games/all");
        }

        [Admin]
        public IActionResult All()
        {
            var allGames = this.games
                .All()
                .Select(g => $@"
                    <tr>
                        <td>{g.Id}</td>
                        <td>{g.Name}</td>
                        <td>{g.Size} GB</td>
                        <td>{g.Price} &euro;</td>
                        <td>
                            <a class=""btn btn-warning btn-sm"" href=""/games/edit?id={g.Id}"">Edit</a>
                            <a class=""btn btn-danger btn-sm"" href=""/games/delete?id={g.Id}"">Delete</a>
                        </td>
                    </tr>");

            this.ViewModel["games"] = string.Join(string.Empty, allGames);

            return this.View();
        }

        private void SetGameViewData(Game game)
        {
            this.ViewModel["title"] = game.Title;
            this.ViewModel["description"] = game.Description;
            this.ViewModel["thumbnail"] = game.ThumbnailUrl;
            this.ViewModel["price"] = game.Price.ToString("F2");
            this.ViewModel["size"] = game.Size.ToString("F1");
            this.ViewModel["video-id"] = game.VideoId;
            this.ViewModel["release-date"] = game.ReleaseDate.ToString("yyyy-MM-dd");
        }
    }
}
