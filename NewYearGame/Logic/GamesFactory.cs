using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYearGame.Logic
{
    class GamesFactory
    {
        public static List<Game> createFromJson(string json)
        {
            List<Game> gamesList = new List<Game>();

            dynamic data = JObject.Parse(json);

            JArray games = (JArray)data["Games"];
            foreach(JObject game in games)
            {
                gamesList.Add(Game.CreateGameFromJson(game));
            }

            return gamesList;
        }
    }
}
