using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;


namespace Twitch_Reigns
{
    class GameHandler
    {
        int MonitorWidth = 1920;
        int MonitorHeight = 1080;

        public enum GameState {ListenChat,ChooseAction,GameOver}
        public static int CurrentState;
        //Static
        public static IrcClient Bot;
        public static SpriteBatch SpriteBatch;
        public static SpriteFont Font;

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public float ColorTimer { get; set; }
        public float ReadTimer { get; set; }
        public float MouseTimer { get; set; }
        public float ClickTimer { get; set; }
        public bool ClickContinue { get; set; }

        public Random Random;

        float ReconnectTimer;

        int left;
        int right;
        public GameHandler()
        {
            Random = new Random();
            CurrentState = Convert.ToInt32(GameState.ListenChat);
            ColorTimer = 4;
            MouseTimer = 8;
            ClickTimer = 1.5f;
           
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out System.Drawing.Point lpPoint);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        public static Point GetCursorPosition()
        {
            
            Point lpPoint = new Point();
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            System.Drawing.Point pt = GetCursorPosition();
            uint X = (uint)pt.X;
            uint Y = (uint)pt.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public void ListenChat()
        {
            if (CurrentState == Convert.ToInt32(GameState.ListenChat))
            {
                MouseTimer -= 1f / 60f;
                if(MouseTimer >= 4)
                {
                    SetCursorPos(MonitorWidth - MonitorWidth / 4, MonitorHeight / 2);
                }
                else if(MouseTimer <= 4)
                {
                    SetCursorPos(MonitorWidth / 4, MonitorHeight / 2);
                    if(MouseTimer <= 0)
                    {
                        CurrentState = Convert.ToInt32(GameState.ChooseAction);
                        MouseTimer = 8;
                    }
                }
            }
            left = 0;
            right = 0;
            foreach (string action in IrcClient.Actions)
            {
                if (action.Contains("!left"))
                {
                    left++;
                }
                if (action.Contains("!right"))
                {
                    right++;
                }
            }

        }

        public void ChooseAction()
        {
            if (CurrentState == Convert.ToInt32(GameState.ChooseAction))
            {
                MouseTimer = 8;
                if (IrcClient.Actions.Count == 0)
                {
                    CurrentState = Convert.ToInt32(GameState.ListenChat);
                }
                

                //if the left and right have the same amount????????????????????????????????????

                if (left > right)
                {
                    SetCursorPos(MonitorWidth / 4, MonitorHeight / 2);
                    DoMouseClick();
                }
                else if (left < right)
                {
                    SetCursorPos(MonitorWidth - MonitorWidth / 4, MonitorHeight / 2);
                    DoMouseClick();
                }
                else if(left == right && IrcClient.Actions.Count != 0)
                {
                    int choose = Random.Next(0, 2);
                    if(choose == 1)
                    {
                        SetCursorPos(MonitorWidth / 4, MonitorHeight / 2);
                        DoMouseClick();
                    }
                    else
                    {
                        SetCursorPos(MonitorWidth - MonitorWidth / 4, MonitorHeight / 2);
                        DoMouseClick();
                    }
                }
                Bot.RemoveActions();
                if(IrcClient.Actions.Count == 0)
                {
                    CurrentState = Convert.ToInt32(GameState.ListenChat);
                }
            }
        }

        public void GameOver()
        {
            if (CurrentState == Convert.ToInt32(GameState.GameOver))
            {
                
                if(ClickContinue)
                {
                    ClickTimer -= 1f / 60f;
                    if (ClickTimer <= 0)
                    {
                        DoMouseClick();
                        ClickTimer = 1.5f;

                    }
                    SetCursorPos(800, 800);
                    if (GetPixelColor(MonitorWidth / 4, (MonitorHeight / 2) - 200) != Color.FromArgb(255, 18, 10, 1))
                    {
                        CurrentState = Convert.ToInt32(GameState.ListenChat);
                        ClickContinue = false;
                    }
                }
                else
                {
                    SetCursorPos(MonitorWidth / 4, MonitorHeight / 2);
                    DoMouseClick();
                    SetCursorPos(MonitorWidth / 4, MonitorHeight / 2);
                    DoMouseClick();
                    ClickContinue = true;
                }
                
                
               
            }
        }

        public void CheckStateGameOver()
        {
            ColorTimer -= 1f / 60f;

            if (ColorTimer <= 0.3f)
            {
                Point pt = GetCursorPosition();
                if (GetPixelColor(MonitorWidth / 4, (MonitorHeight / 2) - 200) == Color.FromArgb(255, 18, 10, 1))
                {
                    CurrentState = Convert.ToInt32(GameState.GameOver);
                }
            }
            if (ColorTimer <= 0)
            {
                ColorTimer = 4;
            }
        }
        public void Update()
        {
            ReconnectTimer += 1f / 60f;
            if(ReconnectTimer >= 3)
            {
                ReconnectTimer = 0;
                Bot.ReConnect();
            }

           /* CheckStateGameOver();
            ListenChat();
            ChooseAction();
            GameOver();*/
        }

        public void Draw()
        {
            //Color a = GetPixelColor(MonitorWidth / 4, (MonitorHeight / 2) - 200);
            

            SpriteBatch.DrawString(Font, "!left", new Microsoft.Xna.Framework.Vector2(100, MonitorHeight / 2), Microsoft.Xna.Framework.Color.White,
                0, new Microsoft.Xna.Framework.Vector2(0, 0), 1, SpriteEffects.None, 0);

            SpriteBatch.DrawString(Font, left.ToString(), new Microsoft.Xna.Framework.Vector2(180, MonitorHeight / 8), Microsoft.Xna.Framework.Color.White,
               0, new Microsoft.Xna.Framework.Vector2(0, 0), 1, SpriteEffects.None, 0);

            SpriteBatch.DrawString(Font, "!right", new Microsoft.Xna.Framework.Vector2(MonitorWidth - 450, MonitorHeight / 2), Microsoft.Xna.Framework.Color.White,
                0, new Microsoft.Xna.Framework.Vector2(0, 0), 1, SpriteEffects.None, 0);

            SpriteBatch.DrawString(Font, right.ToString(), new Microsoft.Xna.Framework.Vector2(MonitorWidth - 320, MonitorHeight / 8), Microsoft.Xna.Framework.Color.White,
                0, new Microsoft.Xna.Framework.Vector2(0, 0), 1, SpriteEffects.None, 0);
        }

        public Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
    }

}