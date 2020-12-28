using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows;
using System.Timers;
using System.Diagnostics;
using Windows.UI.Notifications;

namespace YTDLGUI
{

    public partial class App : System.Windows.Application
    {
        NotifyIcon icon;
        System.Drawing.Icon defaultIcon;
        System.Drawing.Icon downloading;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            createNotifyIcon();
        }
        private void createNotifyIcon()
        {
            defaultIcon = new System.Drawing.Icon("./icon.ico");
            downloading = new System.Drawing.Icon("./downloading.ico");
            icon = new NotifyIcon();

            icon.Icon = defaultIcon;
            icon.Visible = true;
            icon.Text = "YTDLGUI";
            var menu = new ContextMenuStrip();

            icon.DoubleClick += new System.EventHandler(this.DoubleClick);

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            ToolStripMenuItem menuItem3 = new ToolStripMenuItem();
            menuItem.Text = "&設定";
            menuItem.Click += (s, e) =>
            {
                var setting = new MainWindow();
                setting.Show();
            };

            menuItem3.Text = "&終了";
            menuItem3.Click += (s, e) =>
            {
                System.Windows.Application.Current.Shutdown();
            };

            menu.Items.Add(menuItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(menuItem3);

            icon.ContextMenuStrip = menu;
        }
        private void DoubleClick(object Sender, EventArgs e)
        {
            string urlText = System.Windows.Clipboard.GetText();
            string regex = "^?v=([^&]+)";
            Match match = Regex.Match(urlText, regex);
            if (match.Success)
            {
                this.YTDownload("https://youtube.com/watch?" + match.Value);
            }

        }
        public void YTDownload(string url)
        {
            icon.Icon = downloading;
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "youtube-dl.exe";
            info.Arguments = $"-f bestvideo+bestaudio -o output/%(title)s-%(id)s.%(ext)s {url} ";
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            Process proc = new Process();
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit();

            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

            var textNodes = template.GetElementsByTagName("text");
            textNodes.Item(0).InnerText = $"完了 {url}";

            var notifier = ToastNotificationManager.CreateToastNotifier("YoutubeDownloaderGUI");
            var notification = new ToastNotification(template);
            notifier.Show(notification);
            icon.Icon = defaultIcon;
        }
    }
}
