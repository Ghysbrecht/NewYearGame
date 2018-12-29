using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYearGame
{
    static class DefaultValues
    {
        // The MQTT broker address
        public static string MQTT_ADRESS = "127.0.0.1";

        // The maximum ammount of characters that a message may have
        public static int MAX_MESSAGE_LENGTH = 500;

        // The team names
        public static string[] TEAMNAMES =
        {
            "TEAM1",
            "TEAM2",
            "TEAM3",
            "TEAM4"
        };

        // Path to the JSON games file
        public static string JSON_PATH = @"C:\NY-GAME\Game.json";
    }
}
