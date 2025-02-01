using Newtonsoft.Json.Linq;

public partial class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        using HttpClient client = new();

        try
        {
            HttpResponseMessage response = client.GetAsync($"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={Uri.EscapeDataString(team)}").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            JObject json = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            List<Partida> partidas = json["data"].ToObject<List<Partida>>() ?? new List<Partida>();

            return partidas.Sum(partida => partida.Team1Goals);
        }
        catch (Exception) { throw; }
    }
}