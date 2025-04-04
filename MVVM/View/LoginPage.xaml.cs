using Mockup.MVVM.ModelViewModel;

namespace Mockup
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Al navegar fuera de esta página, se limpian los datos ingresados.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is LoginViewModel vm)
            {
                vm.ClearFields();
            }
        }
    }
}
