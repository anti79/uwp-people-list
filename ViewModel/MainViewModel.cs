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
		public ObservableCollection<Person> People { get; set; }

		Person selectedPerson;
		Person editedPerson;

		readonly IView view;

		public event PropertyChangedEventHandler PropertyChanged;


		ICommand saveButtonCommand;
		ICommand startEditingCommand;
		ICommand startAddingCommand;
		ICommand cancelEditCommand;
		ICommand deleteCommand;

		public ICommand SaveButtonCommand { get { return GetSaveButtonCommand(); } }
		public ICommand StartEditingCommand { get { return GetStartEditingCommand(); } }
		public ICommand StartAddingCommand { get { return GetStartAddingCommand(); } }
		public ICommand CancelEditCommand { get { return GetCancelEditCommand(); } }
		public ICommand DeleteCommand { get { return GetDeleteCommand(); } }

		enum EditingStatus
		{
			None,
			AddingNew,
			EditingExisting
		}

		EditingStatus editingStatus;

		public Person SelectedPerson
		{
			get
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
		public bool FormVisibility
		{
			get
			{
				if (editingStatus!=EditingStatus.None) return true;
				return false;
			}
		}


		

		public MainViewModel(IView view)
		{
			People = new ObservableCollection<Person>(Data.People);
			this.view = view;
		}
		
		ICommand GetStartAddingCommand()
		{
			if(startAddingCommand is null)
			{
				startAddingCommand = new Command(() =>
				{
					editingStatus = EditingStatus.AddingNew;
					EditedPerson = new Person() { FirstName = "", LastName = "" };
					RaisePropertyChanged(nameof(FormVisibility));
				});
			}
			return startAddingCommand;
		}

		ICommand GetStartEditingCommand()
		{
			if(startEditingCommand is null)
			{
				startEditingCommand = new Command(() => {
					if (SelectedPerson != null)
					{
						editedPerson = selectedPerson;
						editingStatus = EditingStatus.EditingExisting;
						RaisePropertyChanged(nameof(EditedPerson));
						RaisePropertyChanged(nameof(FormVisibility));
					}

				});
			}
			return startEditingCommand;
		}

		ICommand GetDeleteCommand()
		{
			if(deleteCommand is null)
			{
				deleteCommand = new Command(async () =>
				{

					if (await view.DisplayConfirmationDialog("Delete the entry?"))
					{
						bool res = People.Remove(SelectedPerson);
						RaisePropertyChanged(nameof(People));
						UpdateModel();
					}
					else
					{
						cancelEditCommand.Execute(null);
					}

				});
			}
			return deleteCommand;
		}
		ICommand GetCancelEditCommand()
		{
			if(cancelEditCommand is null)
			{
				cancelEditCommand = new Command(() =>
				{
					editingStatus = EditingStatus.None;
					RaisePropertyChanged(nameof(FormVisibility));
				});
			}
			return cancelEditCommand;
		}

		ICommand GetSaveButtonCommand()
		{
			if(saveButtonCommand is null)
			{
				saveButtonCommand = new Command(() =>
				{
					if (editingStatus != EditingStatus.None)
					{
						if (Data.Validate(EditedPerson))
						{
							if (editingStatus == EditingStatus.EditingExisting)
							{
								People[SelectedIndex] = EditedPerson;
							}
							else if (editingStatus == EditingStatus.AddingNew)
							{
								People.Add(EditedPerson);
							}
							editingStatus = EditingStatus.None;
							RaisePropertyChanged(nameof(People));
							RaisePropertyChanged(nameof(FormVisibility));
							UpdateModel();
						}
						else
						{
							DisplayValidationError();
						}
					}
				});
			}
			return saveButtonCommand;
		}



		void DisplayValidationError()
		{
			view.DisplayInfoDialog("Invalid values. Name has to contain Latin latters only. Age must be between 0 and 116.");
			GetCancelEditCommand().Execute(null);
		}

		void UpdateModel()
		{
			Data.People = this.People.ToList();
		}
		
		void RaisePropertyChanged([CallerMemberName] string propertyName=null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
