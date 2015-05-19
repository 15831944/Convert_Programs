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

namespace Convert_Programs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.fillCombobox();
        }

        private void fillCombobox()
        {
            System.Collections.Specialized.StringCollection pc = Properties.Settings.Default.machines;
            foreach (string m in pc)
            {
                try
                {
                    this.cbxMachines.Items.Add(m);
                }
                catch (ArgumentNullException ane)
                {
                    ErrMsg em = new ErrMsg(ane);
                    em.ShowDialog();
                }
                catch (SystemException se)
                {
                    ErrMsg em = new ErrMsg(se);
                    em.ShowDialog();
                }
            }
        }

        private void ConvertProgram(string fileName, string selection)
        {
            SettingsPropertyCollection pc = Properties.Settings.Default.Properties;
            FileInfo fi = new FileInfo(fileName);
            StringBuilder sb = new StringBuilder();

            using (StreamReader sr = fi.OpenText())
            {
                string text = string.Empty;
                try
                {
                    text = sr.ReadToEnd();
                }
                catch (OutOfMemoryException oome)
                {
                    ErrMsg em = new ErrMsg(oome);
                    em.ShowDialog();
                }
                catch (IOException ioe)
                {
                    ErrMsg em = new ErrMsg(ioe);
                    em.ShowDialog();
                }

                sb.Append(text);

                System.Collections.Specialized.StringCollection sc = Properties.Settings.Default.machines;
                foreach (string m in sc)
                {
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

        private void SaveFile(FileInfo file, StringBuilder sb)
        {
            using (StreamWriter sw = new StreamWriter(file.FullName, false))
            {
                try
                {
                    sw.Write(sb.ToString());
                }
                catch (ObjectDisposedException ode)
                {
                    ErrMsg em = new ErrMsg(ode);
                    em.ShowDialog();
                }
                catch (NotSupportedException nse)
                {
                    ErrMsg em = new ErrMsg(nse);
                    em.ShowDialog();
                }
                catch (IOException ioe)
                {
                    ErrMsg em = new ErrMsg(ioe);
                    em.ShowDialog();
                }
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            foreach (string item in this.lbBatch.Items)
            {
                this.ConvertProgram(item, this.cbxMachines.SelectedItem.ToString());
            }
        }

        private void lbBatch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = Properties.Settings.Default.ext;
            dlg.Filter = Properties.Settings.Default.filter;
            dlg.Multiselect = true;
            dlg.InitialDirectory = Properties.Settings.Default.initDir;
            if (Properties.Settings.Default.lastDir != null)
            {
                dlg.InitialDirectory = Properties.Settings.Default.lastDir;
            }

            Nullable<bool> res = dlg.ShowDialog();

            if (res == true)
            {
                foreach (string item in dlg.FileNames)
                {
                    try
                    {
                        this.lbBatch.Items.Add(item);
                    }
                    catch (ArgumentNullException ane)
                    {
                        ErrMsg em = new ErrMsg(ane);
                        em.ShowDialog();
                    }
                    catch (SystemException se)
                    {
                        ErrMsg em = new ErrMsg(se);
                        em.ShowDialog();
                    }
                    System.IO.FileInfo fi = new FileInfo(item);
                    Properties.Settings.Default.lastDir = fi.DirectoryName;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Save();
        }
    }
}
