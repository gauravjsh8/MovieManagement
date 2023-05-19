using MovieManagementApi.Generic;

namespace MovieManagementApi.Models
{
    public class UserFavourite:GenericModel
    {
        public int Id { get; set; } 
        public string? USerId { get; set; } 
        public int MovieId { get; set; }    
        public Movie? Movie { get; set; }   
    }
}
