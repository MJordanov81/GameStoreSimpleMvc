namespace GameStore.App.Models.Games
{
    using Infrastructure.Validation;
    using Infrastructure.Validation.Games;
    using SimpleMvc.Framework.Attributes.Validation;
    using System;

    public class GameAdminModel
    {
        [Required]
        [Title]
        [Length(minLength: 3, maxLength: 100)]
        public string Title { get; set; }

        [Required]
        [Length(minLength: 20, maxLength: 500)]
        public string Description { get; set; }
        
        [ThumbnailUrl]
        public string ThumbnailUrl { get; set; }

        [NumberRange(0, double.MaxValue)]
        public decimal Price { get; set; }

        [NumberRange(0, double.MaxValue)]
        public double Size { get; set; }

        [Required]
        [Length(minLength: 11, maxLength: 11)]
        public string VideoId { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}
