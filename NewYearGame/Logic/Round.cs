using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYearGame.Logic
{
    public class Round
    {
        public string Title { get; set; }
        public string Objective { get; set; }
        public int MaxScore { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Answer { get; set; }

        public Round()
        {
            Title = "";
            Objective = "";
            MaxScore = 0;
            Type = "";
            FilePath = "";
            Answer = "";
        }

        public static Round CreateGameFromJson(JObject json)
        {
            return new Round()
            {
                Title = (string)json["Title"],
                Objective = (string)json["Objective"],
                MaxScore = (int)json["MaxScore"],
                Type = (string)json["Type"],
                FilePath = (string)json["FilePath"],
                Answer = (string)json["Answer"]
            };
        }
    }
}
