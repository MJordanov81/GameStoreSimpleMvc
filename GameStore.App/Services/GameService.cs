namespace GameStore.App.Services
{
    using Contracts;
    using Data.Models;
    using GameStore.App.Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Games;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameService : IGameService
    {
        public void Create(
            string title,
            string description,
            string thumbnailUrl,
            decimal price,
            double size,
            string videoId,
            DateTime releaseDate)
        {
            using (var db = new GameStoreDbContext())
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

                db.Games.Add(game);
                db.SaveChanges();
            }
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
            using (var db = new GameStoreDbContext())
            {
                var game = db.Games.Find(id);

                game.Title = title;
                game.Description = description;
                game.ThumbnailUrl = thumbnailUrl;
                game.Price = price;
                game.Size = size;
                game.VideoId = videoId;
                game.ReleaseDate = releaseDate;

                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new GameStoreDbContext())
            {
                var game = db.Games.Find(id);
                db.Games.Remove(game);

                db.SaveChanges();
            }
        }

        public Game GetById(int id)
        {
            using (var db = new GameStoreDbContext())
            {
                return db.Games.Find(id);
            }
        }

        public IEnumerable<GameListingAdminModel> All()
        {
            using (var db = new GameStoreDbContext())
            {
                return db
                    .Games
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

        public IList<GameHomePageListingModel> GetFilteredGames(int userId, string filter)
        {
            using (GameStoreDbContext db = new GameStoreDbContext())
            {
                var games = db.Games.Include(g => g.Orders).ToList();

                if (filter.ToLower() == "owned")
                {
                    games = games.Where(g => g.Orders.Select(o => o.UserId).Contains(userId)).ToList();
                }

                return games.Select(g => new GameHomePageListingModel
                {
                    Title = g.Title,
                    Description = g.Description.Length > 300 ? g.Description.Substring(0, 300) + "..." : g.Description,
                    ThumbnailUrl = g.ThumbnailUrl,
                    Price = g.Price,
                    Size = g.Size
                }).ToList();
            }
        }
    }
}
