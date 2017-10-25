namespace GameStore.App.Services
{
    using Contracts;
    using Data.Contracts;
    using Data.Models;
    using Models.Games;
    using SimpleMvc.Framework.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameService : IGameService
    {
        [Inject]
        private readonly IRepository repository;

        public void Create(
            string title,
            string description,
            string thumbnailUrl,
            decimal price,
            double size,
            string videoId,
            DateTime releaseDate)
        {
            var game = new Game
            {
                Title = title,
                Description = description,
                Price = price,
                Size = size,
                ThumbnailUrl = thumbnailUrl,
                VideoId = videoId,
                ReleaseDate = releaseDate
            };

            this.repository.Add(game);
        }

        public void Update(
            int id,
            string title,
            string description,
            string thumbnailUrl,
            decimal price,
            double size,
            string videoId,
            DateTime releaseDate)
        {
            Game game = this.repository.Retrieve<Game>(id);

            game.Title = title;
            game.Description = description;
            game.ThumbnailUrl = thumbnailUrl;
            game.Price = price;
            game.Size = size;
            game.VideoId = videoId;
            game.ReleaseDate = releaseDate;

            this.repository.Edit(game);
        }

        public void Delete(int id)
        {
            this.repository.Detele<Game>(id);
        }

        public Game GetById(int id)
        {
            return this.repository.Retrieve<Game>(id);
        }

        public IEnumerable<GameListingAdminModel> All()
        {
            return this.repository
            .Retrieve<Game>()
            .Select(g => new GameListingAdminModel
            {
                Id = g.Id,
                Name = g.Title,
                Price = g.Price,
                Size = g.Size
            })
            .ToList();
        }
    }
}
