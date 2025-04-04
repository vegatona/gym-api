using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RoutineFridayPage : ContentPage
    {
        public RoutineFridayPage()
        {
            InitializeComponent();
            BindingContext = new RoutineViewModel();
        }
    }
}
