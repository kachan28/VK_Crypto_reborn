using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using VKCrypto_reborn_win_.Views;

namespace VKCrypto_reborn_win_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public byte newstrnum = 0, oldstrnum = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToMessages_Clicked(object sender, RoutedEventArgs e)
        {
            newstrnum = 1;            
            if (newstrnum != oldstrnum)
            {
                Pages.Content = new Messages();
            }
            oldstrnum = newstrnum;
        }

        private void GoToGroups_Clicked(object sender, RoutedEventArgs e)
        {
            newstrnum = 2;
            if (newstrnum != oldstrnum)
            {
                Pages.Content = new Groups();
            }
            oldstrnum = newstrnum;
        }

        private void Pages_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.5);
            ta.DecelerationRatio = 0.7;
            if (newstrnum > oldstrnum)
            {
                ta.To = new Thickness(0, 0, 0, 0);
                ta.From = new Thickness(0, 700, 0, 0);
            }
            else
            {
                ta.To = new Thickness(0, 0, 0, 0);
                ta.From = new Thickness(0, 0, 0, 700);
            }
            (e.Content as UserControl).BeginAnimation(MarginProperty, ta);
        }
    }
}
