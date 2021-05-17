using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace M65_Discord
{
    class Initializer
    {
		private readonly CommandService _commands;
		private readonly DiscordSocketClient _client;
		private readonly dynamic _config;

		// Ask if there are existing CommandService and DiscordSocketClient
		// instance. If there are, we retrieve them and add them to the
		// DI container; if not, we create our own.
		public Initializer(CommandService commands = null, DiscordSocketClient client = null, dynamic config = null)
		{
			_commands = commands ?? new CommandService();
			_client = client ?? new DiscordSocketClient();
			_config = config;
		}

		public IServiceProvider BuildServiceProvider() => new ServiceCollection()
			.AddSingleton(_client)
			.AddSingleton(_commands)
			.AddSingleton(new ConfigProvider(_config))
			// You can pass in an instance of the desired type
			//.AddSingleton(new NotificationService())
			// ...or by using the generic method.
			//
			// The benefit of using the generic method is that 
			// ASP.NET DI will attempt to inject the required
			// dependencies that are specified under the constructor 
			// for us.
			.AddSingleton<CommandHandler>()
			.BuildServiceProvider();
	}

	public class ConfigProvider
    {
		public dynamic Config { get; }
		public ConfigProvider(dynamic config)
        {
			this.Config = config;
        }
    }
}
