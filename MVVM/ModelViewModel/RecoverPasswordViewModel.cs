using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;  // Para usar Preferences
using System.Text.RegularExpressions;

namespace Mockup.MVVM.ModelViewModel
{
    public class RecoverPasswordViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _errorMessage;
        private bool _isErrorVisible;
        private bool _isContinueEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Username
        {
            get => _username;
            set
            {
                string cleanedValue = Regex.Replace(value ?? "", "[^a-zA-Z0-9._,@!*+-]", "");
                if (cleanedValue.Length > 12)
                    cleanedValue = cleanedValue.Substring(0, 12);

                _username = cleanedValue;
                OnPropertyChanged(nameof(Username));
                ValidateUsername();
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

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set
            {
                _isErrorVisible = value;
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public bool IsContinueEnabled
        {
            get => _isContinueEnabled;
            set
            {
                _isContinueEnabled = value;
                OnPropertyChanged(nameof(IsContinueEnabled));
            }
        }

        public ICommand ContinueCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public RecoverPasswordViewModel()
        {
            ContinueCommand = new Command(async () => await ExecuteContinue());
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private void ValidateUsername()
        {
            IsContinueEnabled = !string.IsNullOrWhiteSpace(Username) && Username.Length >= 3;
        }

        private async Task ExecuteContinue()
        {
            if (string.IsNullOrWhiteSpace(Username))
                return;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("userName", Username);
                    var response = await client.GetAsync("https://6m5r2ddv-5210.usw3.devtunnels.ms/api/Users/Consultar-Usuario");

                    if (response.IsSuccessStatusCode)
                    {
                        // Guardar el nombre de usuario en Preferences
                        Preferences.Set("username", Username);

                        ErrorMessage = "";
                        IsErrorVisible = false;
                        await Shell.Current.GoToAsync("//EnterCodePage");
                    }
                    else
                    {
                        ErrorMessage = "El usuario no existe.";
                        IsErrorVisible = true;
                    }
                }
            }
            catch
            {
                ErrorMessage = "Error de conexión. Intenta de nuevo.";
                IsErrorVisible = true;
            }
        }

        public void ClearFields()
        {
            Username = string.Empty;
            ErrorMessage = string.Empty;
            IsErrorVisible = false;
            IsContinueEnabled = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

