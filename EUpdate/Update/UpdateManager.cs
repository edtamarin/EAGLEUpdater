using EUpdate.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace EUpdate.Update
{
    class UpdateManager
    {
        private string _updatePageURL = "https://eagle-updates.circuits.io/downloads/latest.html";

        // properties
        private string _updateLink;
        private bool? _keepVersion;
        private bool? _patchNotes;

        // constructor
        public UpdateManager(bool? keepVer, bool? patchN)
        {
            _updateLink = "";
            _keepVersion = keepVer;
            _patchNotes = patchN;
        }

        // get the URL for the update file
        public void GetUpdateURL()
        {
            var doc = new HtmlDocument();
            // ignore line breaks
            HtmlAgilityPack.HtmlNode.ElementsFlags["br"] = HtmlAgilityPack.HtmlElementFlag.Empty;
            doc.OptionWriteEmptyNodes = true;
            try
            {
                var webRequest = WebRequest.Create(_updatePageURL);
                Stream stream = webRequest.GetResponse().GetResponseStream();
                doc.Load(stream);
                stream.Close();
            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("Can't connect: " + e);
            }
            // parse the webpage to get the dl links
            var node = doc.DocumentNode.SelectSingleNode("//body/div[@class='content']");
            List<HtmlNode> listNodes = node.Descendants("span").ToList();
            foreach (HtmlNode hNode in listNodes)
            {
                // get the windows dl link
                // TODO: other versions?
                if (hNode.InnerText.Contains("Windows"))
                {
                    _updateLink = hNode.FirstChild.Attributes["href"].Value;
                }
            }
        }

        public string GetUpdateFileLink()
        {
            return _updateLink;
        }

        //public void GetUpdateFile()
        //{
        //    using (WebClient wc = new WebClient())
        //    {
        //        wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
        //        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
        //        wc.DownloadFileAsync(new System.Uri(_updateLink),KnownFolders.GetPath(KnownFolder.Downloads));
        //    }
        //}

        //private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    var mainWin = Application.Current.Windows
        //        .Cast<Window>()
        //            .FirstOrDefault(window => window is MainWindow) as MainWindow;
        //    mainWin.dlBar.Visibility = Visibility.Hidden;
        //}

        //private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    var mainWin = Application.Current.Windows
        //                    .Cast<Window>()
        //                        .FirstOrDefault(window => window is MainWindow) as MainWindow;
        //    mainWin.dlBar.Value = e.ProgressPercentage;
        //}
    }
}
