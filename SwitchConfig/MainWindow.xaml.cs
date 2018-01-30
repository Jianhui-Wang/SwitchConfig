using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using Keysight.S8901A.Common;


namespace SwitchConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        virtual internal protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(
                this,
                new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        ObservableCollection<SwitchMatrix.Port> inst_ports { get; set; }
        ObservableCollection<SwitchMatrix.Port> dut_ports { get; set; }
        ObservableCollection<SwitchMatrix.Connection> conns { get; set; }
        ObservableCollection<SwitchMatrix.Path> paths { get; set; }

        bool allow_addDutPort { get; set; }
        bool allow_removeDutPort { get; set; }
        bool allow_addInstPort { get; set; }
        bool allow_removeInstPort { get; set; }
        bool allow_addConn { get; set; }
        bool allow_removeConn { get; set; }
        bool allow_addPath { get; set; }
        bool allow_removePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            inst_ports = new ObservableCollection<SwitchMatrix.Port>();
            dut_ports = new ObservableCollection<SwitchMatrix.Port>();
            conns = new ObservableCollection<SwitchMatrix.Connection>();
            paths = new ObservableCollection<SwitchMatrix.Path>();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dutPortsList.ItemsSource = dut_ports;
            instPortsList.ItemsSource = inst_ports;
            connsList.ItemsSource = conns;
            pathList.ItemsSource = paths;
            addDutPort.DataContext = this;
            removeDutPort.DataContext = this;
            addInstPort.DataContext = this;
            removeInstPort.DataContext = this;
            addConn.DataContext = this;
            removeConn.DataContext = this;
            addPath.DataContext = this;
            removePath.DataContext = this;
        }

        private void addDutPort_Click(object sender, RoutedEventArgs e)
        {
            int idx = -1;
            for (int i = 0; i < dut_ports.Count<SwitchMatrix.Port>() + 1; i++)
            {
                var j = from dp in dut_ports where (dp.id == (i + 1)) select dp;
                if (j.Count() == 0)
                {
                    idx = i + 1;
                    break;
                }
            }
            dut_ports.Add(new SwitchMatrix.Port(idx, dutPort.Text));
            allow_removeDutPort = true;
            OnPropertyChanged("allow_removeDutPort");
        }

        private void removeDutPort_Click(object sender, RoutedEventArgs e)
        {
            if (dutPortsList.SelectedItem != null)
            {
                var p = from dp in dut_ports where (dp.id == ((SwitchMatrix.Port)(dutPortsList.SelectedItem)).id) select dp;
                SwitchMatrix.Port pt = p.FirstOrDefault();
                dut_ports.Remove(pt);
            }
            if (dut_ports.Count == 0)
            {
                allow_removeDutPort = false;
                OnPropertyChanged("allow_removeDutPort");
            }

        }

        private void addInstPort_Click(object sender, RoutedEventArgs e)
        {
            int idx = -1;
            for (int i = 0; i < inst_ports.Count<SwitchMatrix.Port>() + 1; i++)
            {
                var j = from dp in inst_ports where (dp.id == (i + 1)) select dp;
                if (j.Count() == 0)
                {
                    idx = i + 1;
                    break;
                }
            }
            inst_ports.Add(new SwitchMatrix.Port(idx, instPort.Text));
            allow_removeInstPort = true;
            OnPropertyChanged("allow_removeInstPort");
        }

        private void removeInstPort_Click(object sender, RoutedEventArgs e)
        {
            if (instPortsList.SelectedItem != null)
            {
                var p = from dp in inst_ports where (dp.id == ((SwitchMatrix.Port)(instPortsList.SelectedItem)).id) select dp;
                SwitchMatrix.Port pt = p.FirstOrDefault();
                inst_ports.Remove(pt);
            }
            if (inst_ports.Count == 0)
            {
                allow_removeInstPort = false;
                OnPropertyChanged("allow_removeInstPort");
            }

        }

        private void addConnection_Click(object sender, RoutedEventArgs e)
        {
            int idx = -1;
            for (int i = 0; i < conns.Count<SwitchMatrix.Connection>() + 1; i++)
            {
                var j = from dp in conns where (dp.id == (i + 1)) select dp;
                if (j.Count() == 0)
                {
                    idx = i + 1;
                    break;
                }
            }
            SwitchMatrix.Port dutPort = (SwitchMatrix.Port)(dutPortsList.SelectedItem);
            SwitchMatrix.Port instPort = (SwitchMatrix.Port)(instPortsList.SelectedItem);
            string s = instPort.desc + " <---> " + dutPort.desc;
            conns.Add(new SwitchMatrix.Connection(idx, instPort, dutPort, s));
            allow_removeConn = true;
            OnPropertyChanged("allow_removeConn");
        }

        private void removeConnection_Click(object sender, RoutedEventArgs e)
        {
            if (connsList.SelectedItem != null)
            {
                var p = from dp in conns where (dp.id == ((SwitchMatrix.Connection)(connsList.SelectedItem)).id) select dp;
                SwitchMatrix.Connection c = p.FirstOrDefault();
                conns.Remove(c);
            }
            if (conns.Count == 0)
            {
                allow_removeConn = false;
                OnPropertyChanged("allow_removeConn");
            }
        }

        private void dutPortsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dutPortsList.SelectedItem != null && instPortsList.SelectedItem != null)
            {
                allow_addConn = true;
            }
            else
            {
                allow_addConn = false;
            }
            OnPropertyChanged("allow_addConn");
        }

        private void instPortsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dutPortsList.SelectedItem != null && instPortsList.SelectedItem != null)
            {
                allow_addConn = true;
            }
            else
            {
                allow_addConn = false;
            }
            OnPropertyChanged("allow_addConn");
        }

        private void dutPortChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(dutPort.Text))
            {
                allow_addDutPort = true;
            }
            else
            {
                allow_addDutPort = false;
            }
            OnPropertyChanged("allow_addDutPort");
        }

        private void instPortChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(instPort.Text))
            {
                allow_addInstPort = true;
            }
            else
            {
                allow_addInstPort = false;
            }
            OnPropertyChanged("allow_addInstPort");
        }

        private void connsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (connsList.SelectedItems.Count > 1)
            {
                allow_addPath = true;
            }
            else
            {
                allow_addPath = false;
            }
            OnPropertyChanged("allow_addPath");
        }

        private void addPath_Click(object sender, RoutedEventArgs e)
        {
            int idx = -1;
            for (int i = 0; i < paths.Count<SwitchMatrix.Path>() + 1; i++)
            {
                var j = from dp in paths where (dp.id == (i + 1)) select dp;
                if (j.Count() == 0)
                {
                    idx = i + 1;
                    break;
                }
            }
            string s = "";
            for (int i=0; i< connsList.SelectedItems.Count; i++)
            {
                s += ((SwitchMatrix.Connection)(connsList.SelectedItems[i])).desc + "; ";
            }

            List<SwitchMatrix.Connection> a = new List<SwitchMatrix.Connection>();
            for (int i=0; i<connsList.SelectedItems.Count; i++)
            {
                a.Add((SwitchMatrix.Connection)connsList.SelectedItems[i]);
            }
            paths.Add(new SwitchMatrix.Path(idx, a, s));
            allow_removePath = true;
            OnPropertyChanged("allow_removePath");
        }

        private void removePath_Click(object sender, RoutedEventArgs e)
        {
            if (pathList.SelectedItem != null)
            {
                var p = from dp in paths where (dp.id == ((SwitchMatrix.Path)(pathList.SelectedItem)).id) select dp;
                SwitchMatrix.Path path = p.FirstOrDefault();
                paths.Remove(path);
            }
            if (paths.Count == 0)
            {
                allow_removePath = false;
                OnPropertyChanged("allow_removePath");
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\ProgramData\\Keysight\\Power Amplifier Solution\\Profiles\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                            SwitchMatrix sm = new SwitchMatrix(openFileDialog1.FileName);

                            inst_ports.Clear();
                            dut_ports.Clear();
                            conns.Clear();
                            paths.Clear();
                            foreach (SwitchMatrix.Port p in sm.inst_ports)
                            {
                                inst_ports.Add(p);
                            }
                            foreach (SwitchMatrix.Port p in sm.dut_ports)
                            {
                                dut_ports.Add(p);
                            }
                            foreach (SwitchMatrix.Connection p in sm.conns)
                            {
                                conns.Add(p);
                            }
                            foreach (SwitchMatrix.Path p in sm.paths)
                            {
                                paths.Add(p);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            Stream mystream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                string filePath = saveFileDialog1.FileName;
                SwitchMatrix sm = new SwitchMatrix(dut_ports.ToList(), inst_ports.ToList(), conns.ToList(), paths.ToList());
                sm.SaveToConfigFile(filePath);
            }
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
            SwitchMap switchMap = new SwitchMap(dut_ports, inst_ports, conns, paths);
            switchMap.ShowDialog();
        }

    }

}
