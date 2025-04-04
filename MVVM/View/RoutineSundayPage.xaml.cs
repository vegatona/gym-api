using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineSundayPage : ContentPage
    {
        public RoutineSundayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}
