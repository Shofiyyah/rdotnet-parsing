// Demo how to embed an R Graphics Window in a .NET Application
// Used R.NET to start and run R.dll. http://rdotnet.codeplex.com/
// Uses Dino  Esposito's Hook to embed the Graph Window
// http://msdn.microsoft.com/en-us/magazine/cc188920.aspx
// Dieter Menne, Menne Biomed Consulting Tübingen, dieter menne at menne-biomed de
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RDotNet
{
  public partial class RGraphForm : Form
  {
    private readonly RGraphAppHook cbt;
    private  REngine engine;

    public RGraphForm()
    {
      InitializeComponent();
      REngine.SetDllDirectory(GetRPath());
      REngine.CreateInstance("RDotNet");
      engine = REngine.GetInstanceFromID("RDotNet");
      cbt = new RGraphAppHook {GraphControl = RPanel1};
    }

    private void ShowRGraphClick(object sender, EventArgs e)
    {
      // Currently, we can use only one of the two choices, so disable other menu after first use.
      showInWindowToolStripMenuItem.Enabled = false;
      // Swap panels on each click
      cbt.Install();
      engine.EagerEvaluate("plot(rnorm(100))");
      cbt.Uninstall();
    }

    private void ShowInWindowToolStripMenuItemClick(object sender, EventArgs e)
    {
      graphitToolStripMenuItem.Enabled = false;
      // Once a graph has been shown, the exe (or the debugger) hangs for more than a minute
      engine.EagerEvaluate("plot(rnorm(100))");
    }

    private void BugInCloseToolStripMenuItemClick(object sender, EventArgs e)
    {
      if (engine.IsInvalid) return;
      double a = engine.EagerEvaluate("a=c(10,20)").AsNumeric().First();
      Console.WriteLine(a);
      engine.Close(); // There is something wrong with Close()
      // Fails ok now
      try {
        double b = engine.EagerEvaluate("b=c(33,34)").AsNumeric().First();
        MessageBox.Show(@"This box should not come up");
        Console.WriteLine(b);
      }
      catch
      {
        MessageBox.Show(@"Failed correctly after closing. But try to close application now, there will be an exception");
        Console.WriteLine(@"Failed correctly after closing");
      }
    }

    private void Form1FormClosed(object sender, FormClosedEventArgs e)
    {
      if (engine != null)
        engine.Close();
    }

    private static string GetRPath()
    {
      RegistryKey rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
      if (rCore == null)
      {
        throw new ApplicationException("Registry key for R-core not found.");
      }
      bool is64Bit = IntPtr.Size == 8;
      RegistryKey r = rCore.OpenSubKey(is64Bit ? "R64" : "R");
      if (r == null)
      {
        throw new ApplicationException("Registry key is not found.");
      }
      Version currentVersion = new Version((string) r.GetValue("Current Version"));
      string installPath = (string) r.GetValue("InstallPath");
      string bin = Path.Combine(installPath, "bin");
      // Up to 2.11.x, DLLs are installed in R_HOME\bin.
      // From 2.12.0, DLLs are installed in the one level deeper directory.
      return currentVersion < new Version(2, 12) ? bin : Path.Combine(bin, is64Bit ? "x64" : "i386");
    }

    private void DisposeUsingToolStripMenuItemClick(object sender, EventArgs e)
    {
      REngine.SetDllDirectory(GetRPath());
      REngine re = engine = REngine.CreateInstance("RDotNewNet");
      Console.WriteLine(engine.IsRunning);
      double a = engine.EagerEvaluate("a=c(10,20)").AsNumeric().First();
      Console.WriteLine(a);
      re.Close();

      try {
        re = REngine.CreateInstance("RDotNewNet");
        Console.WriteLine(engine.IsRunning);
        double b = engine.EagerEvaluate("a=c(30,20)").AsNumeric().First();
        Console.WriteLine(b);
        re.Close();
      }
      catch
      {
        MessageBox.Show(@"Exception after closing and reopening.");
      }
    }


  }
}