
using System.Net.Http.Json;
using RaceDayMVC.Models;

namespace RaceDayMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // LOGIN
        public async Task<HttpResponseMessage> LoginAsync(
            LoginViewModel model)
        {
            var loginData = new
            {
                Email = model.Email,
                PasswordHash = model.Password
            };

            return await _httpClient.PostAsJsonAsync(
                "api/RaceDay/login", loginData);
        }

        // REGISTER
        public async Task<HttpResponseMessage> RegisterAsync(
            RegisterViewModel model)
        {
            var registerData = new
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = model.Password,
                PhoneNumber = model.PhoneNumber,
                Role = "Participant"
            };

            return await _httpClient.PostAsJsonAsync(
                "api/RaceDay/register", registerData);
        }

        // GET ALL EVENTS
        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var events = await _httpClient.GetFromJsonAsync<List<EventViewModel>>(
                "api/RaceDay/events");

            return events ?? new List<EventViewModel>();
        }

        // GET ALL CATEGORIES
        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/RaceDay/categories");

            return categories ?? new List<CategoryViewModel>();
        }


        // GET ALL PARTICIPANTS
        public async Task<List<ParticipantViewModel>> GetParticipantsAsync()
        {
            var participants =
                await _httpClient.GetFromJsonAsync<List<ParticipantViewModel>>(
                    "api/RaceDay/participants");

            return participants ?? new List<ParticipantViewModel>();
        }
        // GET ALL ENROLMENTS
        public async Task<List<EnrolmentViewModel>> GetEnrolmentsAsync()
        {
            var enrolments =
                await _httpClient.GetFromJsonAsync<List<EnrolmentViewModel>>(
                    "api/RaceDay/enrolments");

            return enrolments ?? new List<EnrolmentViewModel>();
        }

        // GET CONFIRMED ENROLMENTS AVAILABLE FOR RESULTS
        public async Task<List<EnrolmentViewModel>>
            GetAvailableEnrolmentsForResultsAsync(string? apiSessionCookie)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/RaceDay/enrolments/available-for-results"
            );

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.Add("Cookie", apiSessionCookie);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new List<EnrolmentViewModel>();
            }

            var enrolments =
                await response.Content.ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return enrolments ?? new List<EnrolmentViewModel>();
        }

        // GET ALL RESULTS 
        public async Task<List<ResultViewModel>> GetResultsAsync() 
        { 
            var results =
                await _httpClient.GetFromJsonAsync<List<ResultViewModel>>( 
                    "api/RaceDay/results"); 
             
            return results ?? new List<ResultViewModel>();   
        }   
           
        // GET LOGGED-IN PARTICIPANT'S RESULTS  
        public async Task<List<ResultViewModel>> GetMyResultsAsync( 
            string? apiSessionCookie)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/RaceDay/my-results"
            );

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.Add("Cookie", apiSessionCookie);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new List<ResultViewModel>();
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<ResultViewModel>>();

            return results ?? new List<ResultViewModel>();
        }

        // CONFIRM ENROLMENT
        public async Task<HttpResponseMessage> ConfirmEnrolmentAsync(
            int id, string? apiSessionCookie)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/RaceDay/enrolments/{id}/confirm");

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.Add("Cookie", apiSessionCookie);
            }

            return await _httpClient.SendAsync(request);
        }
        // CREATE PARTICIPANT RESULT
        public async Task<HttpResponseMessage> CreateResultAsync(
            AddResultViewModel model,
            string? apiSessionCookie)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/RaceDay/results"
            );

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.Add("Cookie", apiSessionCookie);
            }

            request.Content = JsonContent.Create(new
            {
                EnrolmentID = model.EnrolmentID,
                FinishTime = model.FinishTime,
                Position = model.Position
            });

            return await _httpClient.SendAsync(request);
        }

        // GET ONE EVENT
        public async Task<EventViewModel?> GetEventAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<EventViewModel>(
                $"api/RaceDay/events/{id}");
        }

        // CREATE EVENT - ORGANISER ONLY
        public async Task<HttpResponseMessage> CreateEventAsync(
            EventViewModel model,
            string? apiSessionCookie)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/RaceDay/events"
            );

            // Forward the API login session
            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiSessionCookie
                );
            }

            request.Content = JsonContent.Create(new
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                EventType = model.EventType,
                Date = model.Date,
                Distance = model.Distance
            });

            return await _httpClient.SendAsync(request);
        }

        // EDIT EVENT - ORGANISER ONLY
        public async Task<HttpResponseMessage> UpdateEventAsync(
            int id,
            EventViewModel model,
            string? apiSessionCookie)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/RaceDay/events/{id}"
            );

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiSessionCookie
                );
            }

            request.Content = JsonContent.Create(new
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                EventType = model.EventType,
                Date = model.Date,
                Distance = model.Distance
            });

            return await _httpClient.SendAsync(request);
        }

        // DELETE EVENT - ORGANISER ONLY
        public async Task<HttpResponseMessage> DeleteEventAsync(
            int id,
            string? apiSessionCookie)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/RaceDay/events/{id}"
            );

            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiSessionCookie
                );
            }

            return await _httpClient.SendAsync(request);
        }


        // CREATE PARTICIPANT PROFILE
        public async Task<HttpResponseMessage> CreateParticipantProfileAsync(
            ParticipantProfileViewModel model,
            string? apiSessionCookie)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/RaceDay/participants/profile"
            );

            // Forward the API login session
            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiSessionCookie
                );
            }

            request.Content = JsonContent.Create(new
            {
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                City = model.City
            });

            return await _httpClient.SendAsync(request);
        }

        // ENROL PARTICIPANT IN AN EVENT
        public async Task<HttpResponseMessage> EnrolParticipantAsync(
            EnrolmentRequest model,
            string? apiSessionCookie)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/RaceDay/enrolments"
            );

            // Forward the API login session
            if (!string.IsNullOrEmpty(apiSessionCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiSessionCookie
                );
            }

            request.Content = JsonContent.Create(new
            {
                EventID = model.EventID,
                CategoryID = model.CategoryID
            });

            return await _httpClient.SendAsync(request);
        }

    }

}
