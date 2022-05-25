using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PushersVC.Effects
{
    public class VolumeUpper
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public void Mute()
        {
            SendMessageW(default, WM_APPCOMMAND, default,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public void VolDown()
        {
            SendMessageW(default, WM_APPCOMMAND, default,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        public void VolUp()
        {
            SendMessageW(default, WM_APPCOMMAND, default,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }
    }
}