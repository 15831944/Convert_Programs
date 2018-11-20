using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Configuration;

namespace Convert_Programs {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    const int THISFUNCTION = 9;

    public MainWindow() {
      InitializeComponent();
      this.Top = Properties.Settings.Default.Top;
      this.Left = Properties.Settings.Default.Left;
      this.fillCombobox();
    }

    private void fillCombobox() {
      System.Collections.Specialized.StringCollection pc = Properties.Settings.Default.machines;
      foreach (string m in pc) {
        try {
          this.cbxMachines.Items.Add(m);
        } catch (ArgumentNullException ane) {
					ENGINEERINGDataSet.ProcessError(ane);
        } catch (SystemException se) {
					ENGINEERINGDataSet.ProcessError(se);
        }
      }
    }

    private void ConvertProgram(string fileName, string selection) {
      SettingsPropertyCollection pc = Properties.Settings.Default.Properties;
      FileInfo fi = new FileInfo(fileName);
      StringBuilder sb = new StringBuilder();

      using (StreamReader sr = fi.OpenText()) {
        string text = string.Empty;
        try {
          text = sr.ReadToEnd();
        } catch (OutOfMemoryException oome) {
					ENGINEERINGDataSet.ProcessError(oome);
        } catch (IOException ioe) {
					ENGINEERINGDataSet.ProcessError(ioe);
        }

        sb.Append(text);

        System.Collections.Specialized.StringCollection sc = Properties.Settings.Default.machines;
        foreach (string m in sc) {
          SettingsProperty sp = pc[m];
          SettingsProperty target = pc[selection];
          System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(string[]));

          string[] sps = (string[])xs.Deserialize(new System.Xml.XmlTextReader(sp.DefaultValue.ToString(), System.Xml.XmlNodeType.Element, null));
          string[] tgts = (string[])xs.Deserialize(new System.Xml.XmlTextReader(target.DefaultValue.ToString(), System.Xml.XmlNodeType.Element, null));

          sb.Replace(sps[0], tgts[0]);
          sb.Replace(sps[1], tgts[1]);
        }
      }
      this.SaveFile(fi, sb);
    }

    private void SaveFile(FileInfo file, StringBuilder sb) {
      using (StreamWriter sw = new StreamWriter(file.FullName, false)) {
        try {
          sw.Write(sb.ToString());
        } catch (ObjectDisposedException ode) {
					ENGINEERINGDataSet.ProcessError(ode);
        } catch (NotSupportedException nse) {
					ENGINEERINGDataSet.ProcessError(nse);
        } catch (IOException ioe) {
					ENGINEERINGDataSet.ProcessError(ioe);
        }
      }
    }

    private void AddItem(string item) {
      try {
        if (!this.lbBatch.Items.Contains(item) && item.ToUpper().EndsWith(@".LAX"))
          this.lbBatch.Items.Add(item);
      } catch (ArgumentNullException ane) {
				ENGINEERINGDataSet.ProcessError(ane);
      } catch (SystemException se) {
				ENGINEERINGDataSet.ProcessError(se);
      }
    }

		private void btnGo_Click(object sender, RoutedEventArgs e) {
			if (cbxMachines.SelectedItem == null) {
				MessageBox.Show(@"You need to choose what machine we're converting to.", @"?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			using (ENGINEERINGDataSetTableAdapters.GEN_USERSTableAdapter ta = new ENGINEERINGDataSetTableAdapters.GEN_USERSTableAdapter()) {
				int uid = (int)ta.GetUIDByUsername(Environment.UserName);
				ENGINEERINGDataSet.IncrementOdometer(THISFUNCTION, uid);
			}
      foreach (string item in this.lbBatch.Items) {
        this.ConvertProgram(item, this.cbxMachines.SelectedItem.ToString());
      }
      this.lblStatus.Content = string.Format("Converted programs to {0}.", this.cbxMachines.SelectedItem.ToString());
    }

    private void lbBatch_MouseDown(object sender, MouseButtonEventArgs e) {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

      dlg.DefaultExt = Properties.Settings.Default.ext;
      dlg.Filter = Properties.Settings.Default.filter;
      dlg.Multiselect = true;
      dlg.InitialDirectory = Properties.Settings.Default.initDir;
      if (Properties.Settings.Default.lastDir != null) {
        dlg.InitialDirectory = Properties.Settings.Default.lastDir;
      }

      Nullable<bool> res = dlg.ShowDialog();

      if (res == true) {
        foreach (string item in dlg.FileNames) {
          this.AddItem(item);
          System.IO.FileInfo fi = new FileInfo(item);
          Properties.Settings.Default.lastDir = fi.DirectoryName;
        }
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      Properties.Settings.Default.Top = this.Top;
      Properties.Settings.Default.Left = this.Left;
      Properties.Settings.Default.Save();
    }

    private void mainGrid_Drop(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
        string[] f = e.Data.GetData(DataFormats.FileDrop, true) as string[];
        foreach (string s in f) {
          this.AddItem(s);
        }
      }
    }

    private void lbBatch_KeyDown(object sender, KeyEventArgs e) {
      switch (e.Key) {
        case Key.Delete:
          deleteSelected();
          break;
        default:
          break;
      }
    }

    private void deleteSelected() {
      while (lbBatch.SelectedItems.Count > 0)
        this.lbBatch.Items.Remove(this.lbBatch.SelectedItem);
    }

    private void btnClear_Click(object sender, RoutedEventArgs e) {
      while (this.lbBatch.Items.Count > 0)
        this.lbBatch.Items.RemoveAt(0);
    }
  }
}
