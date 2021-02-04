using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;

// Http download example from C# Masterclass demonstrating
// asynchronous code.
public class HttpDownload
{
    private static bool IsDownloading = false;
    public static async void Download(Button dlButton)
    {

        if (!IsDownloading)
        {
            dlButton.Text = "Download in progress...";
            string myHtml = "Bla";
            await Task.Run(/*async*/() => // You only need the async keyword with functions that call await
            {
                GD.Print($"Thread # {System.Threading.Thread.CurrentThread.ManagedThreadId} during await task");
                IsDownloading = true;
                HttpClient webClient = new HttpClient();
                myHtml = webClient.GetStringAsync("https://google.com").Result;
                IsDownloading = false;
            });
            dlButton.Text = "Download Complete";
        }


        // ALERT! This version will cause the application to hang
        // during the download!
        /*
        if (!IsDownloading)
        {
            dlButton.Text = "Download in progress...";
            string myHtml = "Bla";

            GD.Print($"Thread # {System.Threading.Thread.CurrentThread.ManagedThreadId} during await task");
            IsDownloading = true;
            HttpClient webClient = new HttpClient();
            myHtml = webClient.GetStringAsync("https://google.com").Result;
            IsDownloading = false;

            dlButton.Text = "Download Complete";
        }
        */

        return;
    }
}