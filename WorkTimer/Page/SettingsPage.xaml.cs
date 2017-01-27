namespace WorkTimer.Page
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            Slider.Maximum = 90;
            Slider.Minimum = 1;
            Slider.Value = 1; 
        }
    }
}
