using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

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

    private static bool ShowMessage(Process process, bool IsUser)
    {
        string jsonString = process.ProcessName + process.Id + "IsUser=" + IsUser;

        #region ServiceMessage
        //Console.Out.WriteLine(jsonString);
        //bool result = false;
        //String title = "DEBUGGER HELPER INFO:";
        //int tlen = title.Length;
        //String msg = jsonString;
        //int mlen = msg.Length;
        //int resp = 7;
        //result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, 1, title, tlen, msg, mlen, 4, 0, out resp, true);
        ////MessageBox.Show(jsonString, "Debugger", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        #endregion

        #region NotepadMessage

        // Notepad text
        string textFile = @"C:\Users\vmadministrator\Desktop\Debugger.txt";
        String title = "DEBUGGER HELPER INFO:";
        string finalString = title + Environment.NewLine + "-------------------------------------------" + Environment.NewLine + "[ProcessName]:" + process.ProcessName + Environment.NewLine + "[ProcessId]:" + process.Id + Environment.NewLine + "[IsUserInteractiveMode]:" + IsUser + Environment.NewLine + "-------------------------------------------" + Environment.NewLine;
        string message = @"Current process is blocked for 5 min" + Environment.NewLine + "so you can attach a debugger for debugging" + Environment.NewLine + "-------------------------------------------" + Environment.NewLine + "Type Y or y and save file to ack:" + Environment.NewLine;
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(textFile))
        {
            file.WriteLine(finalString + message); 
        }
        Process.Start("notepad.exe", textFile);
        bool ack = false;
        int timeout = 300000;
        int step = 5000;
        int indexer = 0;
        while (!Debugger.IsAttached)
        {
            Thread.Sleep(step);
        }
        

        //while (indexer < timeout)
        //{
        //    Thread.Sleep(step);
        //    indexer = indexer + step;
        //}

        #endregion

        return true;
    }


    public static void Block()
    {
        try
        {
            FreeConsole();
            AllocConsole();
            ShowMessage(GetCurrentProcessInformation(), UserInteractiveModeOn());
            //BlockProcess();
            System.Diagnostics.Debugger.Break();
        }
        catch (Exception ex)
        {
            //MessageBox.Show(ex.Message);
        }
    }
}
