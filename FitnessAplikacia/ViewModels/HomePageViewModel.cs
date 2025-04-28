using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using Microsoft.Maui.Controls;
using FitnessAplikacia.Views;
using System.ComponentModel;
using Newtonsoft.Json;

using System.Net.Http;

namespace FitnessAplikacia.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public ObservableCollection<string> DayNames { get; } = new ObservableCollection<string>();
        private readonly NotesService _notesService;
        private readonly AuthService _authService;
        private ObservableCollection<NoteModel> _notesForSelectedDay;
        public ObservableCollection<DayModel> Days { get; set; }
        public ObservableCollection<NoteModel> NotesForSelectedDay
        {
            get => _notesForSelectedDay;
            set => SetProperty(ref _notesForSelectedDay, value);
        }
        public ObservableCollection<NoteModel> Notes { get; private set; } = new ObservableCollection<NoteModel>();

        private int _currentMonth;
        private int _currentYear;
        private DayModel _selectedDay;
        private string _newNote;

        private string _userName;
        private string _email;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public int CurrentMonth
        {
            get => _currentMonth;
            set
            {
                if (SetProperty(ref _currentMonth, value))
                {
                    OnPropertyChanged(nameof(CurrentMonthYear));
                }
            }
        }

        public int CurrentYear
        {
            get => _currentYear;
            set
            {
                if (SetProperty(ref _currentYear, value))
                {
                    OnPropertyChanged(nameof(CurrentMonthYear));
                }
            }
        }

        public string CurrentMonthYear => $"{new DateTime(CurrentYear, CurrentMonth, 1):MMMM yyyy}";

        public DayModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                SetProperty(ref _selectedDay, value);
                if (_selectedDay != null)
                {
                    LoadNotesForSelectedDay(_selectedDay.Date).ConfigureAwait(false);  // Pass selected date to LoadNotesForSelectedDay
                }
            }
        }

        public string NewNote
        {
            get => _newNote;
            set => SetProperty(ref _newNote, value);
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand AddNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SelectDayCommand { get; }

        public HomePageViewModel()
        {
            DayNames = new ObservableCollection<string>();
            FirstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            Days = new ObservableCollection<DayModel>();
            NotesForSelectedDay = new ObservableCollection<NoteModel>();
            AddNoteCommand = new Command(async () => await AddNoteAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
            DeleteNoteCommand = new Command<int>(async (id) => await DeleteNoteAsync(id));
            CurrentMonth = DateTime.Today.Month;
            CurrentYear = DateTime.Today.Year;
            PreviousMonthCommand = new Command(GoToPreviousMonth);
            NextMonthCommand = new Command(GoToNextMonth);
            LoadMonthDays(CurrentYear, CurrentMonth);
            SelectedDay = Days.FirstOrDefault();
            SelectDayCommand = new Command<DayModel>(OnDaySelected);

            if (SelectedDay != null)
            {
                LoadNotesForSelectedDay(SelectedDay.Date).ConfigureAwait(false);  // Pass selected date to LoadNotesForSelectedDay
            }
        }



        public DateTime FirstDayOfMonth { get; set; }
        public HomePageViewModel(NotesService notesService, AuthService authService)
            : this()
        {
            _notesService = notesService;
            _authService = authService;
            LoadUserInfoAsync().ConfigureAwait(false); // Load user info when the view model is initialized
        }

        private void LoadMonthDays(int year, int month)
        {
            Days.Clear();
            var firstDayOfMonth = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                Days.Add(new DayModel { Date = new DateTime(year, month, day) });
            }

            UpdateDayNames(firstDayOfMonth);
        }

        private void UpdateDayNames(DateTime firstDayOfMonth)
        {
            string[] allDayNames = { "Po", "Ut", "St", "Št", "Pi", "So", "Ne" };
            int firstDayIndex = (int)firstDayOfMonth.DayOfWeek - 1;
            if (firstDayIndex < 0) firstDayIndex = 6; // Adjust if it's Sunday

            var shiftedDays = allDayNames.Skip(firstDayIndex).Concat(allDayNames.Take(firstDayIndex)).ToList();

            DayNames.Clear(); // Clear the existing collection
            foreach (var day in shiftedDays)
            {
                DayNames.Add(day); // Add the updated day names
            }
        }



        private void GoToPreviousMonth()
        {
            if (CurrentMonth == 1)
            {
                CurrentMonth = 12;
                CurrentYear--;
            }
            else
            {
                CurrentMonth--;
            }
            FirstDayOfMonth = new DateTime(CurrentYear, CurrentMonth, 1);
            LoadMonthDays(CurrentYear, CurrentMonth);
            SelectedDay = Days.FirstOrDefault();
            if (SelectedDay != null)
            {
                LoadNotesForSelectedDay(SelectedDay.Date).ConfigureAwait(false);  // Pass selected date to LoadNotesForSelectedDay
            }
        }

        private void GoToNextMonth()
        {
            if (CurrentMonth == 12)
            {
                CurrentMonth = 1;
                CurrentYear++;
            }
            else
            {
                CurrentMonth++;
            }
            FirstDayOfMonth = new DateTime(CurrentYear, CurrentMonth, 1);
            LoadMonthDays(CurrentYear, CurrentMonth);
            SelectedDay = Days.FirstOrDefault();
            if (SelectedDay != null)
            {
                LoadNotesForSelectedDay(SelectedDay.Date).ConfigureAwait(false);  // Pass selected date to LoadNotesForSelectedDay
            }
        }

        public async Task LoadNotesForSelectedDay(DateTime selectedDate)
        {

            var notesForDay = await _notesService.GetNotesForDateAsync(selectedDate);
            Console.WriteLine(notesForDay);
            NotesForSelectedDay.Clear();
            if (notesForDay != null)
            {
                foreach (var note in notesForDay)
                {
                    Console.WriteLine($"Note: {JsonConvert.SerializeObject(note)}");
                    Console.WriteLine($"ID: {note.Id}, UserId: {note.UserId}, Date: {note.Date}, Content: {note.Content}");
                    NotesForSelectedDay.Add(note);
                }
            }
            else
            {
                Console.WriteLine("No notes found for the selected date.");
            }
        }



        private async Task AddNoteAsync()
        {
            if (string.IsNullOrWhiteSpace(NewNote) || SelectedDay == null) return;

            var email = await _authService.GetUserEmailAsync();
            var note = new NoteModel
            {
                UserId = email,
                Date = SelectedDay.Date,
                Content = NewNote
            };

            var addedNote = await _notesService?.AddNoteAsync(note);
            if (addedNote != null && !string.IsNullOrWhiteSpace(addedNote.Content))
            {
                NotesForSelectedDay.Add(addedNote);
                Notes.Add(addedNote);
            }

            NewNote = string.Empty;
        }

        private async Task DeleteNoteAsync(int id)
        {
            if (id == 0)
            {
                Console.WriteLine("Invalid note Id.");
                return;
            }

            Console.WriteLine($"Deleting note with Id: {id}");

            // Call the service to delete the note
            var success = await _notesService.DeleteNoteAsync(id);

            if (success)
            {
                // Remove from the local collection if delete was successful
                var noteToRemove = NotesForSelectedDay.FirstOrDefault(n => n.Id == id);
                if (noteToRemove != null)
                {
                    NotesForSelectedDay.Remove(noteToRemove);
                    Notes.Remove(noteToRemove);
                }
            }
            else
            {
                Console.WriteLine($"Attempting to delete note with ID: {id}");
                Console.WriteLine($"Failed to delete note with Id: {id}");
            }
        }

        public async Task LogoutAsync()
        {
            await _authService?.LogoutAsync();
            Application.Current.MainPage = new LoginPage(_authService);
        }

        public async Task LoadUserInfoAsync()
        {
            var userInfo = await _authService.GetCurrentUserAsync();
            if (!string.IsNullOrEmpty(userInfo))
            {
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(userInfo);
                UserName = user.UserName;
                Email = user.Email;
            }
        }

        private void OnDaySelected(DayModel selectedDay)
        {
            if (selectedDay != null)
            {
                SelectedDay = selectedDay;
            }
        }
    }

}