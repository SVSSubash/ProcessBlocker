using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public static class ProcessBlocker
{
    [DllImport("kernel32.dll")]
    private static extern Int32 AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern Int32 FreeConsole();

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSSendMessage(
          IntPtr hServer,
          [MarshalAs(UnmanagedType.I4)] int SessionId,
          String pTitle,
          [MarshalAs(UnmanagedType.U4)] int TitleLength,
          String pMessage,
          [MarshalAs(UnmanagedType.U4)] int MessageLength,
          [MarshalAs(UnmanagedType.U4)] int Style,
          [MarshalAs(UnmanagedType.U4)] int Timeout,
          [MarshalAs(UnmanagedType.U4)] out int pResponse,
          bool bWait);

    public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

    public static int WTS_CURRENT_SESSION = 1;


    private static bool BlockProcess()
    {
        Console.ReadKey();

        return true;
    }

    private static Process GetCurrentProcessInformation()
    {
        Process currentProcess = Process.GetCurrentProcess();

        return currentProcess;
    }

    private static bool UserInteractiveModeOn()
    {
        return Environment.UserInteractive;
    }

    private static bool ServiceMessage(string message, string title)
    {
        Console.Out.WriteLine(message);

        while (true)
        {
            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            if (result == DialogResult.No)
            {
                Thread.Sleep(5000);
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private static string BuildMessage(BlockerInfo blockerInfo)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Environment.NewLine + "-------------------------------------------- " + Environment.NewLine + "[ProcessName]:" + blockerInfo.Process.ProcessName + Environment.NewLine + "[ProcessId]:" + blockerInfo.Process.Id + Environment.NewLine + "[IsUserInteractiveMode]:" + blockerInfo.IsUser + Environment.NewLine + "------------------------------------------ - " + Environment.NewLine);

        sb.Append(@"Current process is blocked for 5 min" + Environment.NewLine + "so you can attach a debugger for debugging" + Environment.NewLine + "-------------------------------------------" + Environment.NewLine + "Type Y or y and save file to ack:" + Environment.NewLine);

        return sb.ToString();
    }

    private static bool NotePadMessage(string message, string title, string path)
    {
        string textFileName = "Debugger.log";

        path = path + @"\" + textFileName;

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
        {
            file.WriteLine(message);
        }

        Process.Start("notepad.exe", path);

        return true;
    }

    private static bool ShowMessage(BlockerInfo blockerInfo)
    {

        string message = BuildMessage(blockerInfo);

        if (blockerInfo.IsUser)
        {
            #region ServiceMessage

            ServiceMessage(message, blockerInfo.Title);

            #endregion
        }
        else
        {
            #region NotepadMessage

            NotePadMessage(message, blockerInfo.Title, blockerInfo.LogPath);

            #endregion
        }

        return true;
    }


    public static void Block(string logPath)
    {
        if (String.IsNullOrEmpty(logPath) || !Directory.Exists(logPath))
        {
            throw new Exception(String.Format("[Block]: LogPath = [{0}] is not valid or doesnt exist.",logPath));
        }

        try
        {
            FreeConsole();

            AllocConsole();

            Process process = GetCurrentProcessInformation();

            bool isUser = UserInteractiveModeOn();

            BlockerInfo blockerInfo = new BlockerInfo()
            {
                Process = process,

                IsUser = isUser,

                Title = "DEBUGGER HELPER INFO:",

                LogPath = logPath
            };

            ShowMessage(blockerInfo);

            System.Diagnostics.Debugger.Break();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public class BlockerInfo
    {
        public Process Process { get; set; }

        public bool IsUser { get; set; }

        public string Title { get; set; }

        public string LogPath { get; set; }
    }
}
