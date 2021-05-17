using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace M65_Discord
{
    [Group("mega")]
    public class MegaCommandsModule : ModuleBase<SocketCommandContext>
    {
        public ConfigProvider provider { get; set; }

        [Command("type")]
        [Summary("Type a command into the connected mega65")]
        public async Task MegaType([Remainder][Summary("The text to type")] string passage)
        {

            await ReplyAsync("Done");
        }

        [Command("upload")]
        [Summary("upload attached prg or d81 to the mega")]
        public async Task MegaUpload()
        {
            using var client = new HttpClient();
            var data = await client.GetByteArrayAsync(Context.Message.Attachments.First().Url);
            var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Context.Message.Attachments.First().Filename);
            await System.IO.File.WriteAllBytesAsync(filename, data);

            await ReplyAsync($"file created: {filename} at { System.IO.File.GetCreationTime(filename)}");

        }

        [Command("reset")]
        [Summary("reset connected mega")]
        public async Task MegaType()
        {
            await ReplyAsync("Done");
        }
    }
}
