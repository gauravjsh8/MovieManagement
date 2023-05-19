using MovieManagementApi.Generic;

namespace MovieManagementApi.Models
{
    public class MovieDetails : GenericModel
    {
        public int Id { get; set; }
        public int Genre { get; set; }  
        public string? MovieLink { get; set; }   
        public int MovieId { get; set; }    
        public Movie? Movie { get; set; }

    }
}
