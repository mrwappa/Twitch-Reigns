using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Twitch_Reigns
{
    class IrcClient
    {
        public static List<string> Actions = new List<string>();

        TcpClient tcpClient;
        IPAddress ipAddress;
        StreamReader inputStream;
        StreamWriter outputsStream;
        int adressPort;

        string auth_password;
        string userName;
        string myChannel;

        public IrcClient(string ip, int port, string username, string password)
        {
            ipAddress = IPAddress.Parse(ip);
            adressPort = port;
            auth_password = password;
            userName = username;

            tcpClient = new TcpClient();

            if (!tcpClient.Connected)
            {
                Connect();
            }
        }

        public async void Connect()
        {
            try
            {
                await tcpClient.ConnectAsync(ipAddress, adressPort);
                if (tcpClient.Connected)
                {
                    inputStream = new StreamReader(tcpClient.GetStream());
                    outputsStream = new StreamWriter(tcpClient.GetStream());
                    outputsStream.WriteLine("PASS " + auth_password);
                    outputsStream.WriteLine("NICK " + userName);
                    outputsStream.WriteLine("USER " + userName + " 8 * :" + userName);
                    outputsStream.WriteLine("CAP REQ :twitch.tv/membership");
                    outputsStream.WriteLine("CAP REQ :twtich.tv/commands");
                    outputsStream.Flush();
                    JoinRoom("mrwappa");
                    SendChatMessage("Connected!");
                    SendChatMessage("/slow 5");
                    ReadChat();
                }
            }
            catch
            {
                return;
            }

        }
        string msg = null;
        public async void ReadChat()
        {
            /*byte[] buffer = new byte[1024];

            int n = 0;*/

            try
            {
                msg = await inputStream.ReadLineAsync();
            }
            catch
            {
                return;
            }
            if (msg.Contains("!help"))
            {
                SendChatMessage("Type !left or !right to vote on an action");
            }
            if (GameHandler.CurrentState == Convert.ToInt32(GameHandler.GameState.ListenChat))
            {
                if (msg.Contains("!left") || msg.Contains("!right"))
                {
                    Actions.Add(msg);
                }
            }
            
            ReadChat();
        }

        public void JoinRoom(string channel)
        {
            myChannel = channel;
            outputsStream.WriteLine("JOIN #" + channel);
            outputsStream.Flush();
        }

        public void SendIrcMessage(string message)
        {
            outputsStream.WriteLine(message);
            outputsStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + myChannel + " :" + message);
        }

        public void RemoveActions()
        {
            Actions.RemoveAll(RemoveString);
        }
        private static bool RemoveString(String s)
        {
            return true;
        }
    }
}
