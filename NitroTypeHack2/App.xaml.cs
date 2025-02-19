﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NitroTypeHack2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool isCheatRunning = false;

        public async static void simulateTypingText(
            string text,
            int typingDelay,
            int accuracy,
            Microsoft.Web.WebView2.Wpf.WebView2 webview2)
        {
            isCheatRunning = true;

            if (Globals.useNitros)
            {
                var words = text.Split('\u00A0');
                string longest = words.OrderByDescending(n => n.Length).First();
                text = string.Join(" ", words);
                text = ReplaceFirst(text, longest, "ʜ");
            }

            char[] letters = text.Replace("\u00A0", " ").ToCharArray();
            var gen = new Random();

            int toMiss = (int)Math.Floor(letters.Length * ((decimal)(100 - accuracy) / 100));
            int maxIndex = (int)Math.Floor(letters.Length / (decimal)(toMiss + 1));
            int index = 0;

            foreach (char letter in letters)
            {
                if (letter == 'ʜ')
                {
                    string args;
                    args = @"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                        "Input.dispatchKeyEvent",
                        args
                        );

                    args = @"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 13, ""unmodifiedText"": ""\r"", ""text"": ""\r""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                        "Input.dispatchKeyEvent",
                        args
                        );
                }
                else
                {
                    string args;
                    if (letter == 32)
                    {
                        args = @"{""type"": ""rawKeyDown"", ""windowsVirtualKeyCode"": 32, ""unmodifiedText"": "" "", ""text"": "" ""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                            );

                        args = @"{""type"": ""keyUp"", ""windowsVirtualKeyCode"": 32, ""unmodifiedText"": "" "", ""text"": "" ""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                            );
                    }
                    else
                    {
                        args = @"{""type"": ""char"", ""text"": """ + letter + @"""}";
                        _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Input.dispatchKeyEvent",
                            args
                            );
                    }
                }

                if (index == maxIndex)
                {
                    string args = @"{""type"": ""char"", ""text"": ""+""}";
                    _ = await webview2.CoreWebView2.CallDevToolsProtocolMethodAsync(
                        "Input.dispatchKeyEvent",
                        args
                    );
                    index = 0;
                }
                else
                {
                    index = index + 1;
                }

                if(!Globals.godMode)
                {
                    await Task.Delay((int)(Math.Sin(index) + typingDelay));
                }
            }

            if (Globals.autoGame)
            {
                await Task.Delay(gen.Next(4900, 6100));
                webview2.Reload();
            }

            isCheatRunning = false;
        }

        public static string ReplaceFirst(string input, string find, string replace)
        {
            var first = input.IndexOf(find);
            return input.Substring(0, first) + replace + input.Substring(first + find.Length);
        }
    }
}
