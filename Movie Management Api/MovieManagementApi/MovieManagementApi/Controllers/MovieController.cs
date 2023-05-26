using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieManagementApi.Data;
using MovieManagementApi.Models;
using MovieManagementApi.ViewModels;

namespace MovieManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("addmovie")]
        public IActionResult AddMovie(MovieViewModel movieViewModel)
        {
            using var dbTran = _context.Database.BeginTransaction();
            try
            {
                //create a object of movie to map on movieViewMOdel
                Movie movie = new Movie();
                if (movieViewModel != null)
                {
                    movie.Title = movieViewModel.Title;
                    movie.Description = movieViewModel.Description;
                    movie.UserId = movieViewModel.UserId;
                    movie.CreatedDateTime = DateTime.Now;
                    _context.Movie.Add(movie);
                    _context.SaveChanges();
                    MovieDetails movieDetails = new MovieDetails();
                    movieDetails.MovieId = movie.Id;
                    movieDetails.Genre = movieViewModel.Genre;
                    movieDetails.MovieLink = movieViewModel.MovieLink;
                    movieDetails.CreatedDateTime = DateTime.Now;
                    _context.MovieDetails.Add(movieDetails);
                    _context.SaveChanges();
                    MovieMedia movieMedia = new MovieMedia();
                    movieMedia.MovieId = movie.Id;
                    movieMedia.MediaPath = movieViewModel.MediaPath;
                    movieMedia.CreatedDateTime = DateTime.Now;
                    _context.MovieMedia.Add(movieMedia);
                    _context.SaveChanges();
                    dbTran.Commit();
                    return Ok(movieViewModel);

                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                dbTran.Rollback();
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("allmovies")]
        public IActionResult GetAllMovies([FromQuery] MovieRequestViewModel movieRequest)
        {
            try
            {
                var allMovies = new List<Movie>();
                List<MovieViewModel> movieViewModels = new List<MovieViewModel>();
                if (movieRequest != null)
                {
                    if (movieRequest.SearchName != null
                        && movieRequest.SearchName != "")
                    {
                        //search movie by name
                        if (movieRequest.Limit > 0 && movieRequest.Offset >= 0)
                        {

                            allMovies = _context.Movie.Where(x => x.Title.Contains(movieRequest.SearchName)).
                                Include(x => x.MovieDetails).Include(x => x.Medias)
                                .Skip(movieRequest.Offset).Take(movieRequest.Limit).ToList();
                        }
                        else
                        {
                            allMovies = _context.Movie.Where(x => x.Title.Contains(movieRequest.SearchName))
                                .Include(x => x.MovieDetails)
                                .Include(x => x.Medias).ToList();
                        }

                    }


                    else
                    {

                        //without search
                        if (movieRequest.Limit > 0 && movieRequest.Offset >= 0)
                        {
                            allMovies = _context.Movie
                                .Include(x => x.MovieDetails)
                                .Include(x => x.Medias).Skip(movieRequest.Offset)
                                .Take(movieRequest.Limit).ToList();
                        }
                        else
                        {
                            allMovies = _context.Movie
                                .Include(x => x.MovieDetails)
                                .Include(x => x.Medias).ToList();
                        }
                    }
                    if (allMovies != null)
                    {
                        foreach (var item in allMovies)
                        {
                            MovieViewModel movieViewModel = new MovieViewModel();
                            movieViewModel.MovieId = item.Id;
                            movieViewModel.Title = item.Title;
                            movieViewModel.CreatedDateTime = item.CreatedDateTime;
                            movieViewModel.Description = item.Description;
                            movieViewModel.Genre = item.MovieDetails.Genre;
                            movieViewModel.MovieLink = item.MovieDetails.MovieLink;
                            movieViewModel.MediaPath = item.Medias?.FirstOrDefault()?.MediaPath;
                            movieViewModels.Add(movieViewModel);



                        }
                        if (movieViewModels != null)
                        {
                            movieViewModels[0].Total = _context.Movie.Count();
                        }
                        return Ok(movieViewModels);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet()]
        public IActionResult GetMovieById([FromQuery] int id)
        {
            try
            {
                MovieViewModel movieViewModel = new MovieViewModel();
                var movie = _context.Movie.Where(x => x.Id == id)
                    .Include(x => x.MovieDetails)
                    .Include(x => x.Medias).Include(x=>x.MovieReviews).
                    Include(x=>x.MovieRatings).FirstOrDefault();
                if (movie != null)
                {
                    movieViewModel.MovieId = movie.Id;
                    movieViewModel.Title = movie.Title;
                    movieViewModel.CreatedDateTime = movie.CreatedDateTime;
                    movieViewModel.Description = movie.Description;
                    movieViewModel.Genre = movie.MovieDetails.Genre;
                    movieViewModel.MovieLink = movie.MovieDetails.MovieLink;
                    movieViewModel.MediaPath = movie.Medias?.FirstOrDefault()?.MediaPath;
                    movieViewModel.MovieReviews = new List<MovieReviewViewModel>();
                    if(movie.MovieReviews != null)
                    {
                        foreach(var item in movie.MovieReviews)
                        {
                            MovieReviewViewModel movieReviewViewModel = new MovieReviewViewModel();
                            movieReviewViewModel.MovieId =item.MovieId;
                            movieReviewViewModel.UserId = item.UserId;
                            movieReviewViewModel.ReviewComment = item.Comments;
                            movieViewModel.MovieReviews.Add(movieReviewViewModel);

                        }

                    }
                    movieViewModel.AverageRating = 0;
                    if(movie.MovieRatings != null && movie.MovieRatings.Count>0)
                    {
                        int rating = 0;
                        foreach(var item in movie.MovieRatings)
                        {
                            rating=rating+Convert.ToInt32(item.Rating);
                        }
                        movieViewModel.AverageRating=rating/movie.MovieRatings.Count;
                    }

                    return Ok(movieViewModel);


                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }




        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            try
            {
                var movie = _context.Movie.FirstOrDefault(x => x.Id == id);
                if (movie != null)
                {
                    _context.Movie.Remove(movie);
                    _context.SaveChanges();
                    return Ok("Movie deleted successfully.");
                }

                return NotFound("Movie not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
            [HttpPost("addreview")]
            public IActionResult AddRating(MovieReviewViewModel movieReviewViewModel)
        {
            using var dbTran= _context.Database.BeginTransaction();
            try
            {
                //create an object of movie to map on movieViewModel
                MovieReview movieReview = new MovieReview();
                if (movieReviewViewModel != null)
                {
                    movieReview.MovieId = movieReviewViewModel.MovieId;
                    movieReview.UserId = movieReviewViewModel.UserId;
                    movieReview.Comments = movieReviewViewModel.ReviewComment;
                    movieReview.CreatedDateTime = DateTime.Now;
                    _context.MovieReview.Add(movieReview);
                    _context.SaveChanges();
                    dbTran.Commit();
                    return Ok(movieReview);
                }
                else
                {

                    return BadRequest();

                }
            }
            catch (Exception ex)
            {
                dbTran.Rollback();
                return BadRequest(ex.Message);
            }


                
            }
        [HttpPost("addrating")]
        public IActionResult AddRating(MovieRatingViewModel movieRatingViewModel)
        {
            using var dbTran = _context.Database.BeginTransaction();
            try
            {
                //create an object of movie to map on movieViewModel
                MovieRating movieRating = new MovieRating();
                if (movieRatingViewModel != null)
                {
                    movieRating.MovieId = movieRatingViewModel.MovieId;
                    movieRating.UserId = movieRatingViewModel.UserId;
                    movieRating.Rating = movieRatingViewModel.Rating;
                    movieRating.CreatedDateTime = DateTime.Now;
                    _context.MovieRating.Add(movieRating);
                    _context.SaveChanges();
                    dbTran.Commit();
                    return Ok(movieRating);
                }
                else
                {

                    return BadRequest();

                }
            }
            catch (Exception ex)
            {
                dbTran.Rollback();
                return BadRequest(ex.Message);
            }



        }
    }
           

        



    
}
