using System;
using System.Windows.Forms;

namespace FixedAssetInventory
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                using (var splash = new SplashForm())
                {
                    splash.Show();
                    Application.DoEvents();
                    
                    var mainForm = new MainForm();
                    splash.Close();
                    
                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}\n\n{ex.StackTrace}", 
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}