
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using System.Threading.Tasks;
using FitnessAplikacia.Models;
using Newtonsoft.Json;
namespace FitnessAplikacia.Services
{

    public class NotesService
    {
        private readonly HttpClient _httpClient;

        public NotesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NoteModel>> GetNotesForDateAsync(DateTime date)
        {
            var formattedDate = date.ToString("yyyy-MM-dd"); // Format date correctly
            try
            {
                // Assuming your API endpoint is expecting a date in 'yyyy-MM-dd' format
                var response = await _httpClient.GetAsync($"Notes/{formattedDate}");
                response.EnsureSuccessStatusCode();


                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);


                return JsonConvert.DeserializeObject<List<NoteModel>>(jsonResponse);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notes for {formattedDate}: {ex.Message}");
                return new List<NoteModel>(); // Return an empty list on failure
            }
        }


        public async Task<NoteModel> AddNoteAsync(NoteModel note)
        {
            var json = JsonConvert.SerializeObject(note);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Notes", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorContent}");
                return null; // Return null to handle in the calling code
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<NoteModel>();
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Notes/{id}");
                // Return true if the deletion was successful
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error deleting note with ID {id}: {ex.Message}");

                // Return false if there was an error
                return false;
            }

        }


    }
}