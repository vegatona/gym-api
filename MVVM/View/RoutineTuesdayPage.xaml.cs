using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineTuesdayPage : ContentPage
    {
        public RoutineTuesdayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}