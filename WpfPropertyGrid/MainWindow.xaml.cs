namespace WpfPropertyGrid
{
    using System.Windows;
    using Vms;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SomeSettingVm(new Repository(), new SomeSetting());
        }
    }
}
