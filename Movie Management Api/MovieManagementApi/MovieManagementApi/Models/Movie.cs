using MovieManagementApi.Generic;

namespace MovieManagementApi.Models
{
    public class Movie : GenericModel
    {
        public int Id { get; set; } 
        public string? Title { get; set; }  
        public string? Description { get; set; } 
        public string? UserId { get; set; } 
        public MovieDetails? MovieDetails { get; set; } 
        public List<MovieMedia>? Medias { get; set; }    
        public List<MovieRating>? MovieRatings { get; set; }
        public List<MovieReview>? MovieReviews { get; set;}
        public List<UserFavourite>? UserFavourites { get; set; }    
        public List<UserMovieLog>? UserMovieLogs { get; set; }  
    }
}
