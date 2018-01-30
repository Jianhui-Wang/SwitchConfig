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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SwitchConfig
{
    /// <summary>
    /// Interaction logic for Port.xaml
    /// </summary>
    public partial class Port : UserControl, INotifyPropertyChanged
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

        public string help { get; set; }

        public Port()
        {
            InitializeComponent();
            button.DataContext = this;
        }

        public Port(string port_info) : this()
        {
            this.help = port_info;
        }

    }
}
