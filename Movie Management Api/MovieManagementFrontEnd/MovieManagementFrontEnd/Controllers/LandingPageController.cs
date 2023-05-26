using Microsoft.AspNetCore.Mvc;
using MovieManagementFrontEnd.ViewModels;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MovieManagementFrontEnd.Controllers
{
    public class LandingPageController : Controller
    {
        public async Task<IActionResult> Index(int limit, int offset, string searchName)
        {
            if (limit <= 0)
            {
                limit = 6;
            }
            if (offset <= 0)
            {
                offset = 0;
            }

            if (searchName == null)
            {
                searchName = "";
            }
            MovieRequestViewModel movieRequestViewModel = new MovieRequestViewModel();
            movieRequestViewModel.Limit = limit;
            movieRequestViewModel.Offset = offset;
            movieRequestViewModel.SearchName = searchName;

            string apiurl = "https://localhost:7175/api/Movie/allmovies";
            string querystring = $"?limit={movieRequestViewModel.Limit}&offset={movieRequestViewModel.Offset}" +
                $"&searchName={Uri.EscapeDataString(movieRequestViewModel.SearchName)}";

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiurl + querystring);
            var responsestring = response.Content.ReadAsStringAsync();
            List<MovieViewModel>? movies = new List<MovieViewModel>();
            if (response.IsSuccessStatusCode)
            {
                movies = JsonConvert.DeserializeObject<List<MovieViewModel>>(responsestring.Result);
            }
            {
                return View(movies);
            }
        }
    }
}

