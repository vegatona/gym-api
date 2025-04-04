using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineMondayPage : ContentPage
    {
        public RoutineMondayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}