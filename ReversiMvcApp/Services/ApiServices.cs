using ReversiMvcApp.Models;

namespace ReversiMvcApp.Services;

public class ApiServices
{
    private readonly HttpClient _httpClient;

    public ApiServices()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7230/");
    }
    
    public List<Spel> KrijgAlleOPenSpellen() {
        List<Spel> objecten = new();
        // Ophalen uit API
        var resultaat = _httpClient.GetAsync("/api/spel/openstaand").Result;
        if (resultaat.IsSuccessStatusCode)
        {
            objecten = resultaat.Content.ReadAsAsync<List<Spel>>().Result;
        }
        
        return objecten;
    }

    public List<Spel> KrijgAlleSpellenVanGebruiker(string Guuid)
    {
        List<Spel> objecten = new();
        // Ophalen uit API
        var resultaat = _httpClient.GetAsync($"/api/spel/van-gebruiker?Guuid={Guuid}").Result;
        if (resultaat.IsSuccessStatusCode)
        {
            objecten = resultaat.Content.ReadAsAsync<List<Spel>>().Result;
        }
        
        return objecten;
    }

    // public Spel krijgSpelDatOpenStaatVoorGebruiker()
    // {
    //     
    // }
        //
        // public List<Spel> GetAll(string spelerToken) {
        //     List<Spel> objecten = new();
        //     // Ophalen uit API
        //     var resultaat = httpClient.GetAsync($"/api/player/{spelerToken}").Result;
        //     if (resultaat.IsSuccessStatusCode) {
        //         objecten = resultaat.Content.ReadAsAsync<List<Spel>>().Result;
        //     }
        //     
        //     return objecten;
        // }
        //

        public Spel JoinSpel(string token, string spelerToken)
        {
            Spel spel = null;
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", token), 
                new KeyValuePair<string, string>("spelertoken", spelerToken) 
            });
        
            HttpResponseMessage result = _httpClient.PostAsync("/api/spel/join", formContent).Result;
            
            if (result.IsSuccessStatusCode) {
                spel = result.Content.ReadAsAsync<Spel>().Result;
            }
            
            return spel;
        }
        
        public Spel KrijgSpel(string id) {
            Spel objecten = null;
            // Ophalen uit API
            var resultaat = _httpClient.GetAsync($"/api/spel/krijg-spel?token={id}").Result;
            if (resultaat.IsSuccessStatusCode) {
                objecten = resultaat.Content.ReadAsAsync<Spel>().Result;
            }
            
            return objecten;
        }
        
        // public bool Delete(string id, string spelerToken) {
        //     // Ophalen uit API
        //     var resultaat = httpClient.DeleteAsync($"/api/spel/{id}/?token={spelerToken}").Result;
        //     
        //     return resultaat.IsSuccessStatusCode;
        // }
        
        public Spel NieuwSpel(string spelerToken, string omschrijving) {
            Spel spel = null;
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("spelerToken", spelerToken), 
                new KeyValuePair<string, string>("omschrijving", omschrijving) 
            });
        
            HttpResponseMessage result = _httpClient.PostAsync("/api/spel/aanmaken", formContent).Result;
            
            if (result.IsSuccessStatusCode) {
                spel = result.Content.ReadAsAsync<Spel>().Result;
            }
            
            return spel;
        }
        
        // public Spel JoinSpel(string id, string spelerToken) {
        //     Spel objecten = null;
        //
        //     HttpResponseMessage result = httpClient.PutAsync(
        //         $"/api/spel/{id}/join/?token={spelerToken}", new StringContent("")
        //     ).Result;
        //     if (result.IsSuccessStatusCode) {
        //         objecten = result.Content.ReadAsAsync<Spel>().Result;
        //     }
        //     
        //     return objecten;
        // }
        //
        // public Spel DoeZet(string id, string spelerToken, int rij, int kolom) {
        //     Spel objecten = null;
        //
        //     HttpResponseMessage result = httpClient.PutAsync(
        //         $"/api/spel/{id}/zet?token={spelerToken}&rij={rij}&kolom={kolom}", new StringContent("")
        //     ).Result;
        //     if (result.IsSuccessStatusCode) {
        //         objecten = result.Content.ReadAsAsync<Spel>().Result;
        //     }
        //     
        //     return objecten;
        // }
}