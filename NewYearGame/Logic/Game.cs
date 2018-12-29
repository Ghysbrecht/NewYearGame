using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYearGame.Logic
{
    public class Game
    {
        public string Name { get; set; }
        public List<Round> Rounds { get; set; }
        public int[] Scores { get; set; }

        public Game()
        {
            Name = "";
            Rounds = null;
        }

        public int GetAmmountOfRounds()
        {
            return Rounds == null ? 0 : Rounds.Count;
        }

        public static Game CreateGameFromJson(JObject json)
        {
            List<Round> list = new List<Round>();

            foreach(JObject round in (JArray)json["Rounds"])
            {
                list.Add(Round.CreateGameFromJson(round));
            }

            return new Game()
            {
                Name = (string)json["Name"],
                Rounds = list
            };
        }
    }
}
