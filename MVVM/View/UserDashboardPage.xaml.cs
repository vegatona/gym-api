using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class UserDashboardPage : ContentPage
    {
        public UserDashboardPage()
        {
            InitializeComponent();
            BindingContext = new UserDashboardViewModel();
        }
    }
}