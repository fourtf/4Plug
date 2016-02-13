using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace FPlug.Widgets
{
    public class SubmitFeedbackWindow : Dialog
    {
        public SubmitFeedbackWindow(string category)
        {
            Icon = App.Icon;
            Title = "Thanks for your feedback!";

            Resizable = false;

            VBox vbox = new VBox();

            var text = new TextEntry()
            {
                MinWidth = 500,
                MinHeight = 200,
                MultiLine = true,
                Text = ""
            };

            vbox.PackStart(text);
            {
                HBox box = new HBox();

                Button btn;
                box.PackEnd(btn = new Button(" Cancel "));
                btn.Clicked += (s, e) => { Close(); };

                box.PackEnd(btn = new Button(" Submit "));
                btn.Clicked += (s, e) =>
                {
                    string feedback = text.Text;
                    new Task(() =>
                        {
                            if (feedback.Length > 10)
                            {
                                string URL = "http://yuhrney.square7.ch/4Plug/feedback.php";
                                WebClient webClient = new WebClient();
                                webClient.Proxy = null;

                                NameValueCollection formData = new NameValueCollection();

                                formData["feedback"] = string.Format("{3} - {1} {2}\n{5}\n{4}", App.WindowTitle, Environment.OSVersion.Platform, Xwt.Toolkit.CurrentEngine.Type, DateTime.Now.ToString("dd MMM HH:mm:ss", CultureInfo.InvariantCulture), feedback, category).Trim();

                                byte[] responseBytes = webClient.UploadValues(URL, "POST", formData);
                                string responsefromserver = Encoding.UTF8.GetString(responseBytes);
                                Console.WriteLine(responsefromserver);
                                webClient.Dispose();
                            }

                        }).Start();
                    Close();
                };

                vbox.PackStart(box);

                Content = vbox;
            }
        }
    }
}
