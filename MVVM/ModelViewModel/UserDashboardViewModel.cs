using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Mockup.MVVM.ModelViewModel
{
    [QueryProperty(nameof(Username), "username")]
    [QueryProperty(nameof(UserNumber), "UserNumber")]

    public class UserDashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                }
            }
        }

        // Propiedades para la información adicional
        public string BloodType { get; set; } = "O+";
        public string Membership { get; set; } = "Premium";
        public string ExpirationDate { get; set; } = "31/12/2025";
        public string Observations { get; set; } = "Sin observaciones.";

        // Propiedades para las opciones de rutina
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
                UpdateDetailedRoutines();       // Actualiza las rutinas detalladas según la selección
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
                    ShowRoutineDetailModal();  // Muestra un modal al seleccionar una rutina detallada
            }
        }

        // Propiedades para la actividad (fecha)
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                // Mostrar información de la fecha seleccionada
                ShowActivityInfo();
            }
        }

        // Propiedad para el máximo de la fecha (según la zona horaria de Sonora)
        public DateTime MaximumDate { get; set; }

        // Propiedades para controlar la visibilidad de controles
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

        // Comandos para las acciones de la vista
        public ICommand LogoutCommand { get; }
        public ICommand RegenerateCodeCommand { get; }
        public ICommand ToggleDayButtonsCommand { get; }
        public ICommand ToggleDatePickerCommand { get; }
        public ICommand SelectDayCommand { get; }

        public UserDashboardViewModel()
        {
            // Inicializa el máximo de fecha usando la zona horaria de Sonora
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

            // Inicializa los comandos
            LogoutCommand = new Command(async () => await Logout());
            RegenerateCodeCommand = new Command(async () => await RegenerateCode());
            ToggleDayButtonsCommand = new Command(() => AreDayButtonsVisible = !AreDayButtonsVisible);
            ToggleDatePickerCommand = new Command(() => IsActivityDatePickerVisible = !IsActivityDatePickerVisible);
            SelectDayCommand = new Command<string>(async (routinePage) => await SelectDay(routinePage));
        }

        // Método que genera un código de 6 dígitos y lo muestra
        private async Task RegenerateCode()
        {
            Random random = new Random();
            string newCode = random.Next(100000, 999999).ToString();
            await Shell.Current.DisplayAlert("Nuevo código", $"El nuevo código de acceso es: {newCode}", "OK");
        }

        // Método para cerrar sesión con confirmación
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Cerrar sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        // Método para navegar a la página de rutina según el día seleccionado
        private async Task SelectDay(string routinePage)
        {
            if (string.IsNullOrEmpty(routinePage))
                return;

            Page page = null;
            // Se selecciona la página según el parámetro recibido
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

        // Actualiza las rutinas detalladas según el músculo seleccionado
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

        // Muestra un modal con el detalle de la rutina seleccionada
        private async void ShowRoutineDetailModal()
        {
            await Shell.Current.DisplayAlert("Rutina seleccionada",
                $"Has seleccionado: {SelectedDetailedRoutine}",
                "OK");
        }

        // Muestra información de la actividad al seleccionar una fecha
        private async void ShowActivityInfo()
        {
            string branch = "\nPuerto de Mazatlán 4602, México, 85190 Cd. Obregón, Son.";
            string formattedDate = SelectedDate.ToString("D");
            await Shell.Current.DisplayAlert("Actividad:",
                $"Fecha de entrada:\n {formattedDate}\nSucursal: {branch}",
                "OK");
        }

        // Método auxiliar para notificar cambios en las propiedades
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}