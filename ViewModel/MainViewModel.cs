using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UWPApp.Model;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace UWPApp.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			People = new ObservableCollection<Person>(Data.People);
			DeleteCommand = new Command(() =>
			{
				bool res = People.Remove(SelectedPerson);
				RaisePropertyChanged(nameof(People));
				RaisePropertyChanged(nameof(SelectedPerson));
				CancelEditCommand.Execute(null);
				
				UpdateModel();


			});

			StartEditingCommand = new Command(() => {

				editedPerson = selectedPerson;
				RaisePropertyChanged(nameof(EditedPerson));
				IsEditing = true;
				RaisePropertyChanged(nameof(FormVisibility));
				RaisePropertyChanged(nameof(SaveButtonCommand));

			});
			StartAddingCommand = new Command(() =>
			{
				IsEditing = false;
				IsAdding = true;
				EditedPerson = new Person() { FirstName="",LastName="" };
				RaisePropertyChanged(nameof(SaveButtonCommand));
				RaisePropertyChanged(nameof(FormVisibility));
			});
			

			saveEditedCommand = new Command(() =>
			{
				if(!Data.Validate(EditedPerson))
				{
					DisplayValidationError();
					return;
				}
				People[SelectedIndex] = EditedPerson;
				RaisePropertyChanged(nameof(SelectedPerson));
				RaisePropertyChanged(nameof(People));
				IsEditing = false;
				RaisePropertyChanged(nameof(FormVisibility));
				UpdateModel();
				

			});
			saveNewCommand = new Command(() =>
			{
				if (!Data.Validate(EditedPerson))
				{
					DisplayValidationError();
					return;
				}
				People.Add(EditedPerson);
				RaisePropertyChanged(nameof(People));
				IsAdding = false;
				RaisePropertyChanged(nameof(FormVisibility));
				UpdateModel();
			});

			CancelEditCommand = new Command(() =>
			{
				IsAdding = false;
				IsEditing = false;
				RaisePropertyChanged(nameof(FormVisibility));
			});
		}
		public ObservableCollection<Person> People { get; set; }

		Person selectedPerson;
		Person editedPerson;

		ICommand saveEditedCommand;
		ICommand saveNewCommand;

		async void DisplayValidationError()
		{
			MessageDialog messageDialog = new MessageDialog("Invalid values. Name has to contain Latin latters only. Age must be between 0 and 116.");
			await messageDialog.ShowAsync();
			CancelEditCommand.Execute(null);
		}

		public Person SelectedPerson { get
			{
				return selectedPerson;
			}
			set
			{
				selectedPerson = value;
				RaisePropertyChanged();
			}
		}
		public Person EditedPerson
		{
			get
			{
				return editedPerson;
			}
			set
			{
				editedPerson = value;
				RaisePropertyChanged();
			}
		}
		public int SelectedIndex { get; set; }
		public bool IsEditing
		{
			get; set;
		}
		public bool IsAdding
		{
			get; set;
		}
		public Visibility FormVisibility {
		get {
				if (IsAdding || IsEditing) return Visibility.Visible;
				return Visibility.Collapsed;
			} 
		}


		public ICommand SaveButtonCommand {
			get {
				if (IsEditing) return saveEditedCommand;
				if(IsAdding) return saveNewCommand;
				return null;
				
			}
			}
		public ICommand StartEditingCommand {get;set;}
		public ICommand StartAddingCommand {get;set;}
		public ICommand CancelEditCommand { get; set; }
		public void UpdateSelection()
		{
			RaisePropertyChanged(nameof(SelectedPerson));
		}
		private void UpdateModel()
		{
			Data.People = this.People.ToList();
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand DeleteCommand { get; set; }
		private void RaisePropertyChanged([CallerMemberName] string propertyName=null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
