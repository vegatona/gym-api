using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace Mockup.MVVM.ModelViewModel
{
    public partial class NewPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string passwordError;

        [ObservableProperty]
        private string confirmPasswordError;

        [ObservableProperty]
        private bool isPasswordErrorVisible;

        [ObservableProperty]
        private bool isConfirmPasswordErrorVisible;

        [ObservableProperty]
        private bool isChangePasswordEnabled;

        public IRelayCommand ChangePasswordCommand { get; }

        public NewPasswordViewModel()
        {
            ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
        }

        partial void OnNewPasswordChanged(string value) => ValidatePasswords();
        partial void OnConfirmPasswordChanged(string value) => ValidatePasswords();

        private void ValidatePasswords()
        {
            bool isValid = IsValidPassword(NewPassword);

            if (string.IsNullOrEmpty(NewPassword))
            {
                PasswordError = string.Empty;
                IsPasswordErrorVisible = false;
            }
            else if (!isValid)
            {
                PasswordError = "La contraseña debe tener al menos 8 caracteres, una mayúscula y una minúscula.";
                IsPasswordErrorVisible = true;
            }
            else
            {
                PasswordError = string.Empty;
                IsPasswordErrorVisible = false;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordErrorVisible = false;
            }
            else if (NewPassword != ConfirmPassword)
            {
                ConfirmPasswordError = "Las contraseñas no coinciden.";
                IsConfirmPasswordErrorVisible = true;
            }
            else
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordErrorVisible = false;
            }

            IsChangePasswordEnabled = isValid &&
                                      !string.IsNullOrEmpty(ConfirmPassword) &&
                                      (NewPassword == ConfirmPassword);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
            return regex.IsMatch(password);
        }

        private async Task ChangePasswordAsync()
        {
            try
            {
                var username = Preferences.Get("username", string.Empty);
                if (string.IsNullOrEmpty(username))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo recuperar el usuario.", "OK");
                    return;
                }

                var content = new StringContent(
                    JsonSerializer.Serialize(new { contraseña = NewPassword }),
                    Encoding.UTF8,
                    "application/json"
                );

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("userName", username);

                var response = await client.PutAsync("https://6m5r2ddv-5210.usw3.devtunnels.ms/api/Users/Modificar-Contraseña", content);

                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Tu contraseña ha sido cambiada exitosamente.", "Iniciar sesión");

                    Preferences.Remove("username");
                    Preferences.Remove("password");

                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cambiar la contraseña. Intenta nuevamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cambiar contraseña: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error inesperado.", "OK");
            }
        }

        public void ClearFields()
        {
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            IsPasswordErrorVisible = false;
            IsConfirmPasswordErrorVisible = false;
            IsChangePasswordEnabled = false;
        }
    }
}
