using Mockup.MVVM.ModelViewModel;

namespace Mockup;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }
    // Al navegar fuera de esta página, se limpian los datos ingresados.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is RegisterViewModel vm)
        {
            vm.ClearFields();
        }
    }
}
