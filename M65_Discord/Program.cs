using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace M65_Discord
{
	public class Program
	{
		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		private DiscordSocketClient _client;



		public async Task MainAsync()
		{
			//  You can assign your bot token to a string, and pass that in to connect.
			//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
			//var token = "token";

			// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
			//var token = Environment.GetEnvironmentVariable("M65_DISCORD_TOKEN");
			// var token = File.ReadAllText("token.txt");
			dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
			var token = config.token.ToString();

			_client = new DiscordSocketClient();

			var init = new Initializer(null, _client, config);
			var provider = init.BuildServiceProvider();
			await ((CommandHandler)provider.GetService(typeof(CommandHandler))).InstallCommandsAsync();

			_client.Log += Log;


			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
