using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineSaturdayPage : ContentPage
    {
        public RoutineSaturdayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}
