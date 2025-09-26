using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gprs
{
    public partial class Form1 : Form
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////键盘驱动
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////键盘钩子
        #region Windows API 声明

        // 钩子相关API
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // 鼠标事件结构体
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region 常量定义

        // 钩子类型
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WH_MOUSE = 7;

        // 鼠标消息
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        // 键盘消息
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        #endregion

        #region 变量和委托

        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        HookProc keyboardProc;
        HookProc mouseProc;
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 安装键盘和鼠标钩子
            InstallHooks();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 卸载钩子
            UninstallHooks();
        }

        #region 钩子管理

        private void InstallHooks()
        {
            // 安装键盘钩子
            keyboardProc = new HookProc(KeyboardHookCallback);
            _keyboardHookID = SetHook(WH_KEYBOARD_LL, keyboardProc);

            // 安装鼠标钩子
            mouseProc = new HookProc(MouseHookCallback);
            _mouseHookID = SetHook(WH_MOUSE_LL, mouseProc);
        }

        private void UninstallHooks()
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        private IntPtr SetHook(int hookType, HookProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(hookType, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        #endregion

    }
}
