using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Mockup.MVVM.Models; // Modelo ClienteDto

namespace Mockup.MVVM.ModelViewModel
{
    [QueryProperty(nameof(Username), "username")]
    [QueryProperty(nameof(UserNumber), "UserNumber")]
    public class UserDashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Propiedades recibidas por Query
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _userNumber;
        public string UserNumber
        {
            get => _userNumber;
            set
            {
                if (_userNumber != value)
                {
                    _userNumber = value;
                    OnPropertyChanged(nameof(UserNumber));
                    Task.Run(() => LoadUserDataAsync(_userNumber));
                }
            }
        }

        // Propiedades a mostrar en la UI
        private string _nombreCompleto;
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(nameof(NombreCompleto)); }
        }

        private string _nip;
        public string NIP
        {
            get => _nip;
            set { _nip = value; OnPropertyChanged(nameof(NIP)); }
        }

        private string _foto;
        public string Foto
        {
            get => _foto;
            set { _foto = value; OnPropertyChanged(nameof(Foto)); }
        }

        private string _celular;
        public string Celular
        {
            get => _celular;
            set { _celular = value; OnPropertyChanged(nameof(Celular)); }
        }

        private string _claveSeguridad;
        public string ClaveSeguridad
        {
            get => _claveSeguridad;
            set { _claveSeguridad = value; OnPropertyChanged(nameof(ClaveSeguridad)); }
        }

        // Información adicional
        private string _membership;
        public string Membership
        {
            get => _membership;
            set { _membership = value; OnPropertyChanged(nameof(Membership)); }
        }

        private string _expirationDate ;
        // Si la API no devuelve fecha, quedará null
        public string ExpirationDate
        {
            get => _expirationDate;
            set { _expirationDate = value; OnPropertyChanged(nameof(ExpirationDate)); }
        }

        public string _observacion;
        public string Observations
        {
            get => _observacion;
            set { _observacion = value; OnPropertyChanged(nameof(Observations)); }
        }

        // Propiedad para el nuevo código (opcional, si se usa)
        private string _newCode;
        public string NewCode
        {
            get => _newCode;
            set { _newCode = value; OnPropertyChanged(nameof(NewCode)); }
        }

        // Propiedades para rutinas y actividades
        public ObservableCollection<string> RoutineOptions { get; } = new ObservableCollection<string>
        {
            "Pecho", "Bíceps", "Tríceps", "Espalda", "Pierna"
        };

        private string _selectedRoutine;
        public string SelectedRoutine
        {
            get => _selectedRoutine;
            set
            {
                _selectedRoutine = value;
                OnPropertyChanged(nameof(SelectedRoutine));
                UpdateDetailedRoutines();
                IsRoutineVisible = !string.IsNullOrEmpty(value);
            }
        }

        private ObservableCollection<string> _detailedRoutines = new ObservableCollection<string>();
        public ObservableCollection<string> DetailedRoutines
        {
            get => _detailedRoutines;
            set { _detailedRoutines = value; OnPropertyChanged(nameof(DetailedRoutines)); }
        }

        private string _selectedDetailedRoutine;
        public string SelectedDetailedRoutine
        {
            get => _selectedDetailedRoutine;
            set
            {
                _selectedDetailedRoutine = value;
                OnPropertyChanged(nameof(SelectedDetailedRoutine));
                if (!string.IsNullOrEmpty(value))
                    ShowRoutineDetailModal();
            }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                ShowActivityInfo();
            }
        }

        public DateTime MaximumDate { get; set; }

        private bool _isRoutineVisible;
        public bool IsRoutineVisible
        {
            get => _isRoutineVisible;
            set { _isRoutineVisible = value; OnPropertyChanged(nameof(IsRoutineVisible)); }
        }

        private bool _areDayButtonsVisible;
        public bool AreDayButtonsVisible
        {
            get => _areDayButtonsVisible;
            set { _areDayButtonsVisible = value; OnPropertyChanged(nameof(AreDayButtonsVisible)); }
        }

        private bool _isActivityDatePickerVisible;
        public bool IsActivityDatePickerVisible
        {
            get => _isActivityDatePickerVisible;
            set { _isActivityDatePickerVisible = value; OnPropertyChanged(nameof(IsActivityDatePickerVisible)); }
        }

        // Comandos
        public ICommand LogoutCommand { get; }
        public ICommand RegenerateCodeCommand { get; }
        public ICommand ToggleDayButtonsCommand { get; }
        public ICommand ToggleDatePickerCommand { get; }
        public ICommand SelectDayCommand { get; }

        public UserDashboardViewModel()
        {
            try
            {
                TimeZoneInfo sonoraTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Hermosillo");
                DateTime sonoraNow = TimeZoneInfo.ConvertTime(DateTime.Now, sonoraTimeZone);
                MaximumDate = sonoraNow.Date;
            }
            catch
            {
                MaximumDate = DateTime.Today;
            }

            LogoutCommand = new Command(async () => await Logout());
            RegenerateCodeCommand = new Command(async () => await RegenerateCodeAsync());
            ToggleDayButtonsCommand = new Command(() => AreDayButtonsVisible = !AreDayButtonsVisible);
            ToggleDatePickerCommand = new Command(() => IsActivityDatePickerVisible = !IsActivityDatePickerVisible);
            SelectDayCommand = new Command<string>(async (routinePage) => await SelectDay(routinePage));
        }

        // Llama a la API para cargar los datos del usuario a partir del NIP
        private async Task LoadUserDataAsync(string nip)
        {
            if (string.IsNullOrEmpty(nip))
                return;

            try
            {
                using (var client = new HttpClient())
                {
                    // Ajusta la URL de tu API
                    string url = $"https://6m5r2ddv-5210.usw3.devtunnels.ms/api/Clients/ConsultarClientePorNIP?NIP={nip}";
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<ClienteDto>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (data != null)
                        {
                            NombreCompleto = data.nombre_Completo;
                            NIP = data.nip;
                            Foto = string.IsNullOrWhiteSpace(data.foto) ? "user.png" : data.foto;
                            Celular = data.celular;
                            ClaveSeguridad = data.clave_Seguridad;
                            if (!string.IsNullOrEmpty(data.membresia))
                                Membership = data.membresia;
                            // Si fecha_Fin está vacía se asigna null
                            ExpirationDate = string.IsNullOrWhiteSpace(data.fecha_Fin) ? null : data.fecha_Fin;
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "No se pudo obtener datos del usuario.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
            }
        }

        // Llama a la API para generar y actualizar la nueva clave de seguridad
        private async Task RegenerateCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(NIP))
            {
                await Shell.Current.DisplayAlert("Error", "No se tiene un NIP válido para regenerar el código.", "OK");
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    // Ajusta la URL de la API para generar el código
                    string url = "https://6m5r2ddv-5210.usw3.devtunnels.ms/api/Clients/Generar-Nueva-Clave-De-Seguridad";
                    var body = new { NIP = this.NIP };
                    var jsonBody = JsonSerializer.Serialize(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        // Suponiendo que la API devuelve el nuevo código en la propiedad clave_Seguridad
                        var data = JsonSerializer.Deserialize<ClienteDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (data != null && !string.IsNullOrEmpty(data.clave_Seguridad))
                        {
                            ClaveSeguridad = data.clave_Seguridad;
                            await Shell.Current.DisplayAlert("Éxito", $"Nuevo código generado: {ClaveSeguridad}", "OK");
                        }
                        else
                        {
                            // Si no se devuelve, se recarga la información
                            await LoadUserDataAsync(NIP);
                            await Shell.Current.DisplayAlert("Éxito", "Se generó una nueva clave de seguridad.", "OK");
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", $"No se pudo generar el nuevo código: {response.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Cerrar sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private async Task SelectDay(string routinePage)
        {
            if (string.IsNullOrEmpty(routinePage))
                return;

            Page page = null;
            switch (routinePage)
            {
                case "RoutineMondayPage":
                    page = new RoutineMondayPage();
                    break;
                case "RoutineTuesdayPage":
                    page = new RoutineTuesdayPage();
                    break;
                case "RoutineWednesdayPage":
                    page = new RoutineWednesdayPage();
                    break;
                case "RoutineThursdayPage":
                    page = new RoutineThursdayPage();
                    break;
                case "RoutineFridayPage":
                    page = new RoutineFridayPage();
                    break;
                case "RoutineSaturdayPage":
                    page = new RoutineSaturdayPage();
                    break;
                case "RoutineSundayPage":
                    page = new RoutineSundayPage();
                    break;
            }
            if (page != null)
            {
                await Shell.Current.Navigation.PushAsync(page);
            }
        }

        private void UpdateDetailedRoutines()
        {
            if (SelectedRoutine == "Pecho")
            {
                DetailedRoutines = new ObservableCollection<string>
                {
                    "Press inclinado con barra (4x8-10 reps)",
                    "Press inclinado con mancuernas (3x10-12 reps)",
                    "Aperturas en banco inclinado (3x12-15 reps)",
                    "Flexiones en banco inclinado (3x15-20 reps, hasta el fallo)",
                    "Press de banca plano con barra (4x8 reps)",
                    "Fondos en paralelas (dips) (4x12 reps)",
                    "Crossover en poleas (aperturas) (3x12-15 reps)",
                    "Press declinado con mancuernas o barra (3x10-12 reps)"
                };
            }
            else if (SelectedRoutine == "Tríceps")
            {
                DetailedRoutines = new ObservableCollection<string>
                {
                    "Fondos en banco (bench dips) (3x10-12 reps)",
                    "Extensión de tríceps por encima de la cabeza con mancuerna (3x10-12 reps)",
                    "Jalones de tríceps en polea (agarre recto) (3x10-12 reps)",
                    "Flexiones cerradas (tipo diamante) (3x8-12 reps)",
                    "Press francés con barra EZ (4x10-12 reps)",
                    "Extensiones de tríceps en polea (agarre cuerda) (4x12-15 reps)",
                    "Press cerrado con barra (4x8 reps)"
                };
            }
            else if (SelectedRoutine == "Bíceps")
            {
                DetailedRoutines = new ObservableCollection<string>
                {
                    "Curl con barra EZ (peso pesado) (4x6-8 reps)",
                    "Curl inclinado con mancuernas (3x10 reps)",
                    "Curl martillo con cuerda (polea) (3x12-15 reps)",
                    "Curl concentrado (3x12 reps por brazo)",
                    "Curl con barra recta (ligero) (3x12-15 reps)",
                    "Curl en predicador (banco Scott) (3x10-12 reps)",
                    "Curl en polea baja (agarre supino) (3x12-15 reps)"
                };
            }
            else if (SelectedRoutine == "Espalda")
            {
                DetailedRoutines = new ObservableCollection<string>
                {
                    "Dominadas (agarre amplio) (4x8-10 reps)",
                    "Jalón al pecho en polea alta (agarre estrecho) (3x12 reps)",
                    "Remo en máquina (agarre neutro) (3x10-12 reps)",
                    "Remo con mancuerna (un brazo) (3x10 reps por lado)",
                    "Pullovers con mancuerna (3x15 reps)",
                    "Remo con barra T (agarre cerrado) (4x10 reps)",
                    "Encogimientos con mancuernas (trapecio) (3x12 reps)",
                    "Remo en polea baja (agarre recto) (4x12 reps)"
                };
            }
            else if (SelectedRoutine == "Pierna")
            {
                DetailedRoutines = new ObservableCollection<string>
                {
                    "Peso muerto convencional o sumo (4x6 reps)",
                    "Prensa de piernas (peso pesado) (4x8-10 reps)",
                    "Zancadas con barra o mancuernas (4x12 reps por pierna)",
                    "Elevaciones de talones con peso (4x20 reps)",
                    "Hip thrust con barra (peso moderado) (4x12-15 reps)",
                    "Curl de pierna acostado (máquina) (3x15 reps)",
                    "Elevaciones de pantorrillas (con peso adicional) (4x20 reps)",
                    "Sentadilla búlgara con mancuernas (3x10-12 reps por pierna)"
                };
            }
        }

        private async void ShowRoutineDetailModal()
        {
            await Shell.Current.DisplayAlert("Rutina seleccionada",
                $"Has seleccionado: {SelectedDetailedRoutine}",
                "OK");
        }

        private async void ShowActivityInfo()
        {
            string formattedDate = SelectedDate.ToString("D", CultureInfo.CurrentCulture);
            await Shell.Current.DisplayAlert("Actividad:",
                $"Fecha de entrada:\n {formattedDate}",
                "OK");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
