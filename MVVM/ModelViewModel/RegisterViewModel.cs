using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Mockup.MVVM.ModelViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        // Propiedades para el registro.
        [ObservableProperty] private string username;
        [ObservableProperty] private string password;
        [ObservableProperty] private string confirmPassword;


        // Llamar a la validación cuando cambien los campos
        partial void OnUsernameChanged(string value) => Validate();
        partial void OnPasswordChanged(string value) => Validate();
        partial void OnConfirmPasswordChanged(string value) => Validate();

        [ObservableProperty] private string usernameError;
        [ObservableProperty] private string passwordError;
        [ObservableProperty] private string confirmPasswordError;

        [ObservableProperty] private bool isUsernameErrorVisible;
        [ObservableProperty] private bool isPasswordErrorVisible;
        [ObservableProperty] private bool isConfirmPasswordErrorVisible;

        [ObservableProperty] private bool isRegisterEnabled;

        public IRelayCommand RegisterCommand { get; }
        public IRelayCommand GoToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
        }

        // Validación de campos.
        private void Validate()
        {
            bool isValid = true;

            // Validar Username
            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = string.Empty;
                IsUsernameErrorVisible = false;
                isValid = false;
            }
            else if (Username.Length < 3 || Username.Length > 12)
            {
                UsernameError = "El usuario debe tener entre 3 y 12 caracteres.";
                IsUsernameErrorVisible = true;
                isValid = false;
            }
            else
            {
                UsernameError = string.Empty;
                IsUsernameErrorVisible = false;
            }

            // Validar Password
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = string.Empty;
                IsPasswordErrorVisible = false;
                isValid = false;
            }
            else if (!IsValidPassword(Password))
            {
                PasswordError = "La contraseña debe tener al menos 8 caracteres.\nUna mayúscula.\nUna minúscula.\nUn número";
                IsPasswordErrorVisible = true;
                isValid = false;
            }
            else
            {
                PasswordError = string.Empty;
                IsPasswordErrorVisible = false;
            }

            // Validar ConfirmPassword
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordErrorVisible = false;
                isValid = false;
            }
            else if (Password != ConfirmPassword)
            {
                ConfirmPasswordError = "Las contraseñas no coinciden.";
                IsConfirmPasswordErrorVisible = true;
                isValid = false;
            }
            else
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordErrorVisible = false;
            }

            IsRegisterEnabled = isValid;
        }

        // Método que valida la contraseña.
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
            return regex.IsMatch(password);
        }

        // Comando que se ejecuta al pulsar el botón de registro.
        private async Task RegisterAsync()
        {
            // Validar campos antes de continuar
            Validate();

            // Si alguna validación falla, evitar el registro
            if (!IsRegisterEnabled)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor, corrige los errores en el formulario.", "OK");
                return;
            }

            // Asignar null al NIP
            string nip = null;

            var userData = new
            {
                nombre_Usuario = Username,
                contraseña = Password,
                rol = "Cliente",
                nip = nip  // El NIP queda como null
            };

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://6m5r2ddv-5210.usw3.devtunnels.ms/");
                    var json = JsonSerializer.Serialize(userData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/Users/Registrar-Usuario-Cliente", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Mensaje de éxito
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Tu registro ha sido exitoso.", "Iniciar sesión");
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar: {responseContent}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                await Application.Current.MainPage.DisplayAlert("Error", $"Error en la solicitud: {ex.Message}", "OK");
            }
        }

        // Comando para navegar a la página de Login.
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // Método para limpiar los campos al salir de la página.
        public void ClearFields()
        {
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            UsernameError = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            IsUsernameErrorVisible = false;
            IsPasswordErrorVisible = false;
            IsConfirmPasswordErrorVisible = false;
            IsRegisterEnabled = false;
        }
    }
}
