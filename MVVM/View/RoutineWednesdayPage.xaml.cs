using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineWednesdayPage : ContentPage
    {
        public RoutineWednesdayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}
