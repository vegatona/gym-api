using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Text;
using System.Text.Json;

namespace Mockup.MVVM.ModelViewModel
{
    class LoginViewModel : INotifyPropertyChanged
    {
        private string _userNumber;
        private string _password;
        private string _errorMessage;
        private string _passwordErrorMessage;
        private bool _isErrorVisible;
        private bool _isPasswordErrorVisible;
        private bool _isLoginEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public string UserNumber
        {
            get => _userNumber;
            set
            {
                _userNumber = SanitizeUsernumber(value);
                OnPropertyChanged(nameof(UserNumber));
                ValidateForm();
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ValidateForm();
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set
            {
                _passwordErrorMessage = value;
                OnPropertyChanged(nameof(PasswordErrorMessage));
            }
        }
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set
            {
                _isErrorVisible = value;
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }
        public bool IsPasswordErrorVisible
        {
            get => _isPasswordErrorVisible;
            set
            {
                _isPasswordErrorVisible = value;
                OnPropertyChanged(nameof(IsPasswordErrorVisible));
            }
        }
        public bool IsLoginEnabled
        {
            get => _isLoginEnabled;
            set
            {
                _isLoginEnabled = value;
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }

        // Comandos para iniciar sesión y navegar a otras páginas.
        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }
        public ICommand GoToRecoverPasswordCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(ExecuteLogin);
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
            GoToRecoverPasswordCommand = new Command(async () => await Shell.Current.GoToAsync("//RecoverPasswordPage"));
        }

        // Filtra el valor ingresado en el usuario permitiendo solo letras y números, limitado a 12 caracteres.
        private string SanitizeUsernumber(string input)
        {
            string sanitized = Regex.Replace(input ?? "", "[^a-zA-Z0-9]", "");
            return sanitized.Length > 12 ? sanitized.Substring(0, 12) : sanitized;
        }

        private void ValidateForm()
        {
            bool isValid = true;

            // Validación del usuario.
            if (string.IsNullOrWhiteSpace(UserNumber))
            {
                ErrorMessage = "";
                IsErrorVisible = false;
                isValid = false;
            }
            else if (UserNumber.Length < 3)
            {
                ErrorMessage = "El usuario debe tener entre 3 y 12 caracteres.";
                IsErrorVisible = true;
                isValid = false;
            }
            else
            {
                ErrorMessage = "";
                IsErrorVisible = false;
            }

            // Validación de la contraseña.
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordErrorMessage = "";
                IsPasswordErrorVisible = false;
                isValid = false;
            }
            else if (!IsValidPassword(Password))
            {
                PasswordErrorMessage = "La contraseña debe tener al menos 8 caracteres.\nUna mayúscula.\nUna minúscula.";
                IsPasswordErrorVisible = true;
                isValid = false;
            }
            else
            {
                PasswordErrorMessage = "";
                IsPasswordErrorVisible = false;
            }

            // Habilitar el botón de login solo si ambos campos tienen datos y son válidos.
            IsLoginEnabled = isValid &&
                             !string.IsNullOrWhiteSpace(UserNumber) &&
                             !string.IsNullOrWhiteSpace(Password);
        }

        // Valida la contraseña mediante una expresión regular.
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            // En este ejemplo se requiere mínimo 8 caracteres, una mayúscula, una minúscula y un dígito.
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
            return regex.IsMatch(password);
        }

        // Ejecuta el login realizando una petición HTTP POST a la API de autenticación.
        private async void ExecuteLogin()
        {
            if (IsLoginEnabled)
            {
                var credentials = new
                {
                    nombre_Usuario = UserNumber,
                    contraseña = Password
                };

                try
                {
                    // Configuramos el HttpClientHandler para ignorar errores de certificado.
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };

                    using (var client = new HttpClient(handler))
                    {
                        // Usar la dirección especial para el emulador Android.
                        client.BaseAddress = new Uri("https://6m5r2ddv-5210.usw3.devtunnels.ms/");

                        var json = System.Text.Json.JsonSerializer.Serialize(credentials);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("api/auth/login", content);

                        // mostra el error
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Debug", $"Código: {response.StatusCode}\nRespuesta: {responseContent}", "OK");

                        if (response.IsSuccessStatusCode)
                        {
                            // Navegar a la siguiente página en caso de éxito.
                            await Shell.Current.GoToAsync($"//UserDashboardPage?username=Ruben González&UserNumber={UserNumber}");
                        }
                        else
                        {
                            // Mostrar un mensaje de error si las credenciales no son válidas.
                            await Shell.Current.DisplayAlert("Error", "Las credenciales no son válidas.", "OK");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones: muestra el error producido.
                    await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
                }
            }
        }

        // Método para limpiar todos los campos y mensajes (por ejemplo, al navegar fuera de la página).
        public void ClearFields()
        {
            UserNumber = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            PasswordErrorMessage = string.Empty;
            IsErrorVisible = false;
            IsPasswordErrorVisible = false;
            IsLoginEnabled = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
