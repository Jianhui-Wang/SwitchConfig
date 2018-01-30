using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Keysight.S8901A.Common;

using PortIcon = SwitchConfig.Port;
using PortData = Keysight.S8901A.Common.SwitchMatrix.Port;
using Connection = Keysight.S8901A.Common.SwitchMatrix.Connection;
using Path = Keysight.S8901A.Common.SwitchMatrix.Path;

namespace SwitchConfig
{
    /// <summary>
    /// Interaction logic for SwitchMap.xaml
    /// </summary>
    public partial class SwitchMap : Window
    {
        const int MAX_NUM_PORTS = 20;


        /*
         *       20*      2*   1*      10*     1*     2*
         *  ------------+----+----+----------+----+-------+
         *              |    |    |          |    |       |   1*
         *  ------------+----+----+----------+----+-------+
         *              |    |    |          |    |       |  10*
         *              |    |    |          |    |       |
         *     ListView |    |    |  Switch  |    |       |
         *              |    |    |  Matrix  |    |       |
         *              |    |    |  Button  |    |       |
         *              |    |    |          |    |       |
         *              |    |    |          |    |       |
         *              |    |    |          |    |       |
         *              |    |    |          |    |       |
         *              |    |    |          |    |       |
         *  ------------+----+----+----------+----+-------+
         *              |    |    |          |    |       |  1*
         *  ------------+----+----+----------+----+-------+
         * 
         */
        const int listViewColumn = 0;
        const int dutPortColumn = 2;
        const int switchColumn = 3;
        const int instPortColumn = 4;
        const int total_columns = 6;
        const int total_rows = 3;

        IEnumerable<PortData> dut_ports = null;
        IEnumerable<PortData> inst_ports = null;
        IEnumerable<Connection> conns = null;
        IEnumerable<Path> paths = null;

        Dictionary<int, PortIcon> dut_port_mapping = new Dictionary<int, PortIcon>();
        Dictionary<int, PortIcon> inst_port_mapping = new Dictionary<int, PortIcon>();
        List<Line> lines = new List<Line>();
        ListView lv = null;

        int dut_port_row_space = 0;
        int inst_port_row_space = 0;
        int DutPortId2RowIdx(int id) { return id * dut_port_row_space; }
        int InstPortId2RowIdx(int id) { return id * inst_port_row_space; }

        public SwitchMap()
        {
            InitializeComponent();
        }

        public SwitchMap(IEnumerable<PortData> dut_ports,
                         IEnumerable<PortData> inst_ports,
                         IEnumerable<Connection> conns,
                         IEnumerable<Path> paths)
        {
            this.dut_ports = dut_ports;
            this.inst_ports = inst_ports;
            this.conns = conns;
            this.paths = paths;
            dut_port_row_space = dut_ports.Count() > 0 ? dut_port_row_space = MAX_NUM_PORTS / (dut_ports.Count() + 1) : 0;
            inst_port_row_space = inst_ports.Count() > 0 ? inst_port_row_space = MAX_NUM_PORTS / (inst_ports.Count() + 1) : 0;
            InitializeComponent();
        }

        private void SwitchMap_Loaded(object sender, RoutedEventArgs e)
        {
            #region Define Row and Column
            RowDefinition rd1 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition rd2 = new RowDefinition { Height = new GridLength(10, GridUnitType.Star) };
            RowDefinition rd3 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition cd1 = new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) };
            ColumnDefinition cd2 = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) };
            ColumnDefinition cd3 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition cd4 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
            ColumnDefinition cd5 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition cd6 = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) };

            MyGrid.RowDefinitions.Add(rd1);
            MyGrid.RowDefinitions.Add(rd2);
            MyGrid.RowDefinitions.Add(rd3);
            MyGrid.ColumnDefinitions.Add(cd1);
            MyGrid.ColumnDefinitions.Add(cd2);
            MyGrid.ColumnDefinitions.Add(cd3);
            MyGrid.ColumnDefinitions.Add(cd4);
            MyGrid.ColumnDefinitions.Add(cd5);
            MyGrid.ColumnDefinitions.Add(cd6);

            #endregion

            #region Create ListView and SwitchBox Button
            ListView lv = new ListView();
            lv.SetValue(Grid.RowProperty, 1);
            lv.SetValue(Grid.ColumnProperty, listViewColumn);
            this.MyGrid.Children.Add(lv);

            Button swtch = new Button();
            swtch.SetValue(Grid.RowProperty, 1);
            swtch.SetValue(Grid.ColumnProperty, switchColumn);
            this.MyGrid.Children.Add(swtch);
            #endregion

            #region Create DUT Ports
            Grid dutPortGrid = new Grid();
            dutPortGrid.SetValue(Grid.RowProperty, 1);
            dutPortGrid.SetValue(Grid.ColumnProperty, dutPortColumn);
            RowDefinition[] dutRowDef = new RowDefinition[MAX_NUM_PORTS];

            for (int i=0; i<dutRowDef.Length; i++)
            {
                dutRowDef[i] = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                dutPortGrid.RowDefinitions.Add(dutRowDef[i]);
            }

            foreach (var p in dut_ports)
            {
                PortIcon dut_port = new PortIcon(p.desc);
                dut_port.SetValue(Grid.RowProperty, DutPortId2RowIdx(p.id));
                if (!dut_port_mapping.ContainsKey(p.id)) dut_port_mapping.Add(p.id, dut_port);
                dutPortGrid.Children.Add(dut_port);
            }

            this.MyGrid.Children.Add(dutPortGrid);
            #endregion

            #region Create Instrument Ports
            Grid instPortGrid = new Grid();
            instPortGrid.SetValue(Grid.RowProperty, 1);
            instPortGrid.SetValue(Grid.ColumnProperty, instPortColumn);
            RowDefinition[] instRowDef = new RowDefinition[MAX_NUM_PORTS];
            for (int i = 0; i < instRowDef.Length; i++)
            {
                instRowDef[i] = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                instPortGrid.RowDefinitions.Add(instRowDef[i]);
            }

            foreach (var p in inst_ports)
            {
                PortIcon inst_port = new PortIcon(p.desc);
                inst_port.SetValue(Grid.RowProperty, InstPortId2RowIdx(p.id));
                if (!inst_port_mapping.ContainsKey(p.id)) inst_port_mapping.Add(p.id, inst_port);
                instPortGrid.Children.Add(inst_port);
            }

            this.MyGrid.Children.Add(instPortGrid);
            #endregion

            #region Initialize the Path List

            GridView gv = new GridView();
            gv.AllowsColumnReorder = true;
            gv.ColumnHeaderToolTip = "Path Information";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.Header = "Path ID";
            gvc1.Width = 50;
            gvc1.DisplayMemberBinding = new Binding("id");
            gv.Columns.Add(gvc1);

            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.Header = "Description";
            gvc2.Width = 150;
            gvc2.DisplayMemberBinding = new Binding("desc");
            gv.Columns.Add(gvc2);

            lv.ItemsSource = paths;
            lv.View = gv;
            lv.SelectionChanged += Lv_SelectionChanged;
            #endregion

            // Force UI Update Layout so that we could draw lines
            this.UpdateLayout();
        }

        private void Lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var l in lines) this.MyGrid.Children.Remove(l);
            lines.Clear();

            lv = (ListView)sender;
            if (lv != null)
            {
                Path p = (Path)lv.SelectedItem;
                foreach (var c in p.conns)
                {
                    var dutPort = (from dp in dut_port_mapping where dp.Key == c.dut_port.id select dp.Value).Single();
                    var instPort = (from dp in inst_port_mapping where dp.Key == c.inst_port.id select dp.Value).Single();

                    Point pt1 = dutPort.TranslatePoint(new Point(dutPort.ActualWidth / 2, dutPort.ActualHeight / 2), this);
                    Point pt2 = instPort.TranslatePoint(new Point(instPort.ActualWidth / 2, instPort.ActualHeight / 2), this);

                    var line = new Line();
                    line.Stroke = Brushes.Green;
                    line.StrokeThickness = 3;
                    line.SetValue(Grid.RowProperty, 0);
                    line.SetValue(Grid.RowSpanProperty, total_rows);
                    line.SetValue(Grid.ColumnProperty, 0);
                    line.SetValue(Grid.ColumnSpanProperty, total_columns);
                    line.X1 = pt1.X;
                    line.Y1 = pt1.Y;
                    line.X2 = pt2.X;
                    line.Y2 = pt2.Y;
                    this.MyGrid.Children.Add(line);

                    lines.Add(line);
                }
            }
        }

        private void SwitchMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Lv_SelectionChanged(lv, null);
        }
    }
}