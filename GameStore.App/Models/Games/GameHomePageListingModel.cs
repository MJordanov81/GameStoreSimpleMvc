using System;

namespace GameStore.App.Models.Games
{
    public class GameHomePageListingModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public double Size { get; set; }

        internal bool IsOwnedByUser()
        {
            throw new NotImplementedException();
        }
    }
}
