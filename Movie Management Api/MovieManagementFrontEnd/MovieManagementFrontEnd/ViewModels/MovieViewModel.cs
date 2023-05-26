namespace MovieManagementFrontEnd.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }    
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public int Genre { get;set; }
        public string? MovieLink { get; set;}
        public string? MediaPath { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int AverageRating { get; set; }  
        public int Total { get; set; }
        public IFormFile? FormFile { get; set; }
        public List<MovieReviewViewModel>? MovieReviews { get; set; }
    }
}
