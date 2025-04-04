using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Mockup.MVVM.Models;

namespace Mockup.MVVM.ModelViewModel
{
    public class RoutineViewModel : BindableObject
    {
        public ObservableCollection<string> Muscles { get; } = new ObservableCollection<string>
        {
            "Pecho", "Biceps", "Triceps", "Espalda", "Pierna"
        };

        private bool _isPickerVisible;
        public bool IsPickerVisible
        {
            get => _isPickerVisible;
            set
            {
                _isPickerVisible = value;
                OnPropertyChanged();
            }
        }

        private string _selectedMuscle;
        public string SelectedMuscle
        {
            get => _selectedMuscle;
            set
            {
                if (_selectedMuscle != value)
                {
                    _selectedMuscle = value;
                    OnPropertyChanged();
                    UpdateExercises();
                }
            }
        }

        private string _selectedMuscleText;
        public string SelectedMuscleText
        {
            get => _selectedMuscleText;
            set
            {
                _selectedMuscleText = value;
                OnPropertyChanged();
            }
        }

        private bool _areExercisesVisible;
        public bool AreExercisesVisible
        {
            get => _areExercisesVisible;
            set
            {
                _areExercisesVisible = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Excersice> Exercises { get; } = new ObservableCollection<Excersice>();

        public ICommand ShowPickerCommand => new Command(() => IsPickerVisible = true);

        private void UpdateExercises()
        {
            Exercises.Clear();
            SelectedMuscleText = $"Has seleccionado: {SelectedMuscle}";
            IsPickerVisible = false;

            if (string.IsNullOrEmpty(SelectedMuscle))
                return;

            switch (SelectedMuscle)
            {
                case "Pecho":
                    Exercises.Add(new Excersice { Name = "Press inclinado con mancuernas o barra (4x8-10 reps)", Image = "press_inclinado.png" });
                    Exercises.Add(new Excersice { Name = "Press de banca plano con barra (4x8 reps)", Image = "press_plano.png" });
                    Exercises.Add(new Excersice { Name = "Press declinado con mancuernas o barra (3x10-12 reps)", Image = "press_declinado.png" });
                    Exercises.Add(new Excersice { Name = "Aperturas en banco inclinado (3x12-15 reps)", Image = "apertura.png" });
                    break;

                case "Biceps":
                    Exercises.Add(new Excersice { Name = "Curl con barra recta (ligero) (3x12-15 reps)", Image = "curl_barra_recta.png" });
                    Exercises.Add(new Excersice { Name = "Curl concentrado (3x12 reps por brazo)", Image = "curl_concen.png" });
                    Exercises.Add(new Excersice { Name = "Curl en predicador (banco Scott) (3x10-12 reps)", Image = "curl_barra_z.png" });
                    Exercises.Add(new Excersice { Name = "Curl martillo con mancuerna (3x12-15 reps)", Image = "curl_martillo.png" });
                    break;

                case "Triceps":
                    Exercises.Add(new Excersice { Name = "Fondos en banco (bench dips) (3x10-12 reps)", Image = "fondos_banco.png" });
                    Exercises.Add(new Excersice { Name = "Press francés con barra EZ (4x10-12 reps)", Image = "press_frances.png" });
                    Exercises.Add(new Excersice { Name = "Jalones de tríceps en polea (agarre recto) (3x10-12 reps)", Image = "extencion_triceps.png" });
                    Exercises.Add(new Excersice { Name = "Extensión de tríceps por encima de la cabeza con mancuerna (3x10-12 reps)", Image = "extension_triceps_encima_cabeza_con_mancuerna.png" });
                    break;

                case "Espalda":
                    Exercises.Add(new Excersice { Name = "Jalón al pecho en polea alta (agarre estrecho) (3x12 reps)", Image = "jalon_pecho_polea_alta_agarre_estrecho.png" });
                    Exercises.Add(new Excersice { Name = "Pullover (3x15 reps)", Image = "pull_over.png" });
                    Exercises.Add(new Excersice { Name = "Remo con barra T (agarre cerrado) (4x10 reps)", Image = "remo_barra_t_agarre_cerrado.png" });
                    Exercises.Add(new Excersice { Name = "Remo en máquina (agarre neutro) (3x10-12 reps)", Image = "remo_maquina_agarre_neutro.png" });
                    break;

                case "Pierna":
                    Exercises.Add(new Excersice { Name = "Hip thrust con barra (peso moderado) (4x12-15 reps)", Image = "hip_thrust_barra.png" });
                    Exercises.Add(new Excersice { Name = "Peso muerto convencional o sumo (4x6 reps)", Image = "peso_muerto.png" });
                    Exercises.Add(new Excersice { Name = "Prensa de piernas (peso pesado) (4x8-10 reps)", Image = "prensa.png" });
                    Exercises.Add(new Excersice { Name = "Sentadilla búlgara con mancuernas (3x10-12 reps por pierna)", Image = "sentadillas_bulgaras.png" });
                    break;
            }

            AreExercisesVisible = Exercises.Count > 0;
        }
    }
}
