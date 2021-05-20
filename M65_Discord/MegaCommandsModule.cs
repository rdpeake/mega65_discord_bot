using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                //run m65-prefix -t <passage>
                var p = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this.provider.Config.m65_prefix,
                        Arguments = $"{(this.provider.Config.m65_port != null ? $"-l {provider.Config.m65_port}" : "")} -t {passage}",
                    }
                };

                p.Start();
                p.WaitForExit();

            } catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
            finally
            {
                //handle errors...

                await ReplyAsync("Done");
            }


        }

        [Command("upload")]
        [Summary("upload attached prg or d81 to the mega")]
        public async Task MegaUpload()
        {
            string filename = null;
            try
            {
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(Context.Message.Attachments.First().Url);
                filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Context.Message.Attachments.First().Filename);
                var baseName = System.IO.Path.GetFileNameWithoutExtension(filename);
                var extension = System.IO.Path.GetExtension(filename);
                await System.IO.File.WriteAllBytesAsync(filename, data);

                if (extension == ".prg")
                {
                    //run m65-prefix filename
                    var p = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = this.provider.Config.m65_prefix,
                            Arguments = $"{(this.provider.Config.m65_port != null ? $"-l {provider.Config.m65_port}" : "")} {filename}"
                        }
                    };

                    p.Start();
                    p.WaitForExit();
                }
                else
                {
                    //push via mega65_ftp
                    //run m65_ftp-prefix -c "put filename <filename_in_8.3>"
                    var p = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = this.provider.Config.m65_ftp_prefix,
                            Arguments = $"{(this.provider.Config.m65_port != null ? $"-l {provider.Config.m65_port}" : "")} -c \"put \\\"{filename}\\\" {baseName.Substring(0, Math.Min(8, baseName.Length)) }.{extension.Substring(1, Math.Min(3, extension.Length))}\""
                        }
                    };

                    p.Start();
                    p.WaitForExit();
                }

            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
            finally
            {
                //handle errors...
                await ReplyAsync($"file created: {filename} at { System.IO.File.GetCreationTime(filename)}");
            }
        }

        [Command("reset")]
        [Summary("reset connected mega")]
        public async Task MegaType()
        {
            try
            {
                //run m65-prefix -F

                var p = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this.provider.Config.m65_prefix,
                        Arguments = $"{(this.provider.Config.m65_port != null ? $"-l {provider.Config.m65_port}" : "")} -F"
                    }
                };

                p.Start();
                p.WaitForExit();
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
            finally
            {
                //handle errors...
                await ReplyAsync("Done");
            }
        }
    }
}
