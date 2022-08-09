using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

public static class ProcessBlocker
{
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

    private static string BuildServiceMessage(BlockerInfo blockerInfo)
    {
        string message = string.Format(
            @"

X------------------------START-------------------------------------X

[PROCESS_NAME] : {0}

[PROCESS_ID]: {1}

[PROCESS_BLOCK_POINT]: {2}

[ASSEMBLY INFO:]

    [CALLING_ASSEMBLY_NAME]: {3}

    [CALLING_ASSEMBLY_FULLNAME]: {4}

    [ASSEMBLY_LOCATION]:{5}

    [IS_GAC]:{6}

[IS_USER_INTERACTIVE_MODE]: {7}

X-------------------------END--------------------------------------X

            ", blockerInfo.Process.ProcessName, blockerInfo.Process.Id, blockerInfo.StackTrace, blockerInfo.Assembly.GetName().Name, blockerInfo.Assembly.FullName, blockerInfo.Assembly.Location, blockerInfo.Assembly.GlobalAssemblyCache, blockerInfo.IsUser );

        return message;
    }

    private static string BuildNotePadMessage(BlockerInfo blockerInfo)
    {
        string message = string.Format(
            @"
            
DEBUGGER HELPER INFO:

X------------------------START-------------------------------------X

[PROCESS_NAME] : {0}

[PROCESS_ID]: {1}

[PROCESS_BLOCK_POINT]: {2}

[ASSEMBLY INFO:]

    [CALLING_ASSEMBLY_NAME]: {3}

    [CALLING_ASSEMBLY_FULLNAME]: {4}

    [ASSEMBLY_LOCATION]:{5}

    [IS_GAC]:{6}

[IS_USER_INTERACTIVE_MODE]: {7}

X-------------------------END--------------------------------------X

            ", blockerInfo.Process.ProcessName, blockerInfo.Process.Id, blockerInfo.StackTrace, blockerInfo.Assembly.GetName().Name, blockerInfo.Assembly.FullName, blockerInfo.Assembly.Location, blockerInfo.Assembly.GlobalAssemblyCache, blockerInfo.IsUser);

        return message;
    }

    private static bool NotePadMessage(string message, string title, string path)
    {
        Guid fileName = Guid.NewGuid();

        string textFileName = "Debugger" + "_" + fileName + ".log";

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

        if (blockerInfo.IsUser)
        {
            #region ServiceMessage

            string message = BuildServiceMessage(blockerInfo);

            ServiceMessage(message, blockerInfo.Title);

            #endregion
        }
        else
        {
            #region NotepadMessage

            string message = BuildNotePadMessage(blockerInfo);

            NotePadMessage(message, blockerInfo.Title, blockerInfo.LogPath);

            #endregion
        }

        while(!Debugger.IsAttached)
        {
            Thread.Sleep(5000);
        }

        return true;
    }

    public static void Block(string logPath)
    {
        // Getting the stack trace. The stack trace holds all the code upto this point, removing the code thats in this dll here, means removing the first 6 lines.

        string[] stackTrace = Environment.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        // Getting the calling assembly.

        Assembly callingAssembly = Assembly.GetCallingAssembly();

        if (String.IsNullOrEmpty(logPath) || !Directory.Exists(logPath))
        {
            throw new Exception(String.Format("[Block]: LogPath = [{0}] is not valid or doesnt exist.",logPath));
        }

        try
        {
            Process process = GetCurrentProcessInformation();

            bool isUser = UserInteractiveModeOn();

            BlockerInfo blockerInfo = new BlockerInfo()
            {
                Process = process,

                IsUser = isUser,

                Title = "DEBUGGER HELPER INFO:",

                LogPath = logPath,

                StackTrace = stackTrace[7], // passing the 7 line, that is the process block point source line.

                Assembly = callingAssembly
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

        public string StackTrace { get; set; }

        public Assembly Assembly { get; set; }
    }
}
