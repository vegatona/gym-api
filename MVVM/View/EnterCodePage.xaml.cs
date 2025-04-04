using Mockup.MVVM.ModelViewModel;

namespace Mockup;

public partial class EnterCodePage : ContentPage
{
    public EnterCodePage()
    {
        InitializeComponent();
        var viewModel = new EnterCodeViewModel();
        BindingContext = viewModel;
        viewModel.FocusNextRequested += OnFocusNextRequested;
        CodeEntry1.Focus();
    }

    // Establece el foco en el Entry correspondiente según el índice solicitado (1-based)
    private void OnFocusNextRequested(int index)
    {
        switch (index)
        {
            case 2:
                CodeEntry2.Focus();
                break;
            case 3:
                CodeEntry3.Focus();
                break;
            case 4:
                CodeEntry4.Focus();
                break;
            case 5:
                CodeEntry5.Focus();
                break;
            case 6:
                CodeEntry6.Focus();
                break;
        }
    }

    // Al salir de la página se limpian los datos ingresados
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is EnterCodeViewModel vm)
        {
            vm.ClearFields();
        }
    }
}

