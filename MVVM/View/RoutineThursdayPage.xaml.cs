using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineThursdayPage : ContentPage
    {
        public RoutineThursdayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}

