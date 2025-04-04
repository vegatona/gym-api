using Microsoft.Maui.Controls;
using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class RecoverPasswordPage : ContentPage
    {
        public RecoverPasswordPage()
        {
            InitializeComponent();
            BindingContext = new RecoverPasswordViewModel();
        }

        // Al salir de la página se limpian los datos ingresados.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is RecoverPasswordViewModel vm)
            {
                vm.ClearFields();
            }
        }
    }
}
