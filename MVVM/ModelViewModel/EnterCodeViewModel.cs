using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Mockup.MVVM.ModelViewModel
{
    public class EnterCodeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Evento para solicitar al view que mueva el foco al Entry de índice especificado (1-based)
        public event Action<int> FocusNextRequested;

        private string _code1;
        public string Code1
        {
            get => _code1;
            set
            {
                string newValue = FilterDigit(value);
                if (_code1 != newValue)
                {
                    _code1 = newValue;
                    OnPropertyChanged(nameof(Code1));
                    if (!string.IsNullOrEmpty(newValue))
                        RequestFocusNext(2);
                    ValidateCode();
                }
            }
        }

        private string _code2;
        public string Code2
        {
            get => _code2;
            set
            {
                string newValue = FilterDigit(value);
                if (_code2 != newValue)
                {
                    _code2 = newValue;
                    OnPropertyChanged(nameof(Code2));
                    if (!string.IsNullOrEmpty(newValue))
                        RequestFocusNext(3);
                    ValidateCode();
                }
            }
        }

        private string _code3;
        public string Code3
        {
            get => _code3;
            set
            {
                string newValue = FilterDigit(value);
                if (_code3 != newValue)
                {
                    _code3 = newValue;
                    OnPropertyChanged(nameof(Code3));
                    if (!string.IsNullOrEmpty(newValue))
                        RequestFocusNext(4);
                    ValidateCode();
                }
            }
        }

        private string _code4;
        public string Code4
        {
            get => _code4;
            set
            {
                string newValue = FilterDigit(value);
                if (_code4 != newValue)
                {
                    _code4 = newValue;
                    OnPropertyChanged(nameof(Code4));
                    if (!string.IsNullOrEmpty(newValue))
                        RequestFocusNext(5);
                    ValidateCode();
                }
            }
        }

        private string _code5;
        public string Code5
        {
            get => _code5;
            set
            {
                string newValue = FilterDigit(value);
                if (_code5 != newValue)
                {
                    _code5 = newValue;
                    OnPropertyChanged(nameof(Code5));
                    if (!string.IsNullOrEmpty(newValue))
                        RequestFocusNext(6);
                    ValidateCode();
                }
            }
        }

        private string _code6;
        public string Code6
        {
            get => _code6;
            set
            {
                string newValue = FilterDigit(value);
                if (_code6 != newValue)
                {
                    _code6 = newValue;
                    OnPropertyChanged(nameof(Code6));
                    ValidateCode();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            private set { _hasError = value; OnPropertyChanged(nameof(HasError)); }
        }

        private bool _isConfirmEnabled;
        public bool IsConfirmEnabled
        {
            get => _isConfirmEnabled;
            private set { _isConfirmEnabled = value; OnPropertyChanged(nameof(IsConfirmEnabled)); }
        }

        // Command para confirmar el código
        public ICommand ConfirmCommand { get; }

        public EnterCodeViewModel()
        {
            ConfirmCommand = new Command(async () => await ConfirmCode(), () => IsConfirmEnabled);
        }

        // Filtra la entrada: permite solo dígitos y devuelve una cadena de un solo carácter.
        private string FilterDigit(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            char c = input[0];
            if (!char.IsDigit(c))
                return "";
            return c.ToString();
        }

        // Solicita al view que enfoque el Entry cuyo índice sea el indicado (1-based)
        private void RequestFocusNext(int nextIndex)
        {
            FocusNextRequested?.Invoke(nextIndex);
        }

        // Valida si se han ingresado 6 dígitos y actualiza la propiedad de habilitación.
        private void ValidateCode()
        {
            string code = $"{Code1}{Code2}{Code3}{Code4}{Code5}{Code6}";
            IsConfirmEnabled = code.Length == 6;
            if (!IsConfirmEnabled)
            {
                HasError = false;
                ErrorMessage = "";
            }
            else
            {
                HasError = false;
                ErrorMessage = "";
            }
            ((Command)ConfirmCommand).ChangeCanExecute();
        }

        // Lógica a ejecutar cuando se confirma el código.
        private async Task ConfirmCode()
        {
            string code = $"{Code1}{Code2}{Code3}{Code4}{Code5}{Code6}";
            if (code.Length != 6)
                return;

            // Navega directamente a la página para ingresar una nueva contraseña.
            await Shell.Current.GoToAsync("//NewPasswordPage");
        }

        // Método para limpiar todos los campos y errores, ideal para invocarlo al navegar fuera de la página.
        public void ClearFields()
        {
            Code1 = "";
            Code2 = "";
            Code3 = "";
            Code4 = "";
            Code5 = "";
            Code6 = "";
            ErrorMessage = "";
            HasError = false;
            IsConfirmEnabled = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
