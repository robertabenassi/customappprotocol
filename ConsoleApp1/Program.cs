using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace ConsoleApp1
{

    internal class Program
    {

        static string CustomProtocol = "myApp";
        static string ShellOpenPath = @"shell\open\command";

        static void RegisterMyProtocol(string applicationExecutable)  //full path to the executable
        {

            string applicationCommand = applicationExecutable + " " + "%1";

            //HKEY_CURRENT_USER\Software\Classes has to be accessed, for a user which is not an admin, and it
            RegistryKey keyCurrentUser = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);

            RegistryKey customKey = keyCurrentUser.OpenSubKey(CustomProtocol);
            if (customKey == null) //if the protocol is not registered yet...we register it
            {
                customKey = keyCurrentUser.CreateSubKey(CustomProtocol);
                customKey.SetValue(string.Empty, "URL: " + CustomProtocol + " Protocol");
                customKey.SetValue("URL Protocol", string.Empty);

                customKey = customKey.CreateSubKey(ShellOpenPath);
                customKey.SetValue(string.Empty, applicationCommand);
                //%1 represents the argument - this tells windows to open this program with an argument / parameter
            }
            else
            {
                // we verify that the customkey has the right shell\open\command
                customKey = customKey.OpenSubKey(ShellOpenPath, true); // true to make it writeable
                System.Console.WriteLine(customKey.GetValue(string.Empty));
                if (!customKey.GetValue(String.Empty).Equals(applicationCommand))
                {
                    customKey.SetValue(string.Empty, applicationCommand);
                }
            }

            customKey.Close();
        }

        [STAThread]
        static void Main(string[] args)
        {

            string appPath = Process.GetCurrentProcess().MainModule.FileName;

            RegisterMyProtocol(appPath);


            var clipboard = Clipboard.GetText();

            // Execution of something triggered by the content of the 
            MessageBox.Show(clipboard, "Browser - console app integration", MessageBoxButtons.OK);
        }
    }
}
