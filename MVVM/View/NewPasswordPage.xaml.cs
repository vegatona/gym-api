using Mockup.MVVM.ModelViewModel;

namespace Mockup;

public partial class NewPasswordPage : ContentPage
{
    public NewPasswordPage()
    {
        InitializeComponent();
        // El BindingContext se asigna en el XAML o desde otro lugar
    }

    // Al salir de la página se limpian los datos ingresados
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is NewPasswordViewModel vm)
        {
            vm.ClearFields();
        }
    }
}
