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
		bool formVisibility = false;
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
		public enum EditingStatus
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
				RaisePropertyChanged(nameof(FormVisibility));
			}
		}
		public int SelectedIndex { get; set; }
		public bool FormVisibility
		{
			get
			{
				return (EditStatus != EditingStatus.None);
			}
			set
			{
				formVisibility = value;
			}
		}
		public EditingStatus EditStatus
		{
			get
			{
				return editingStatus;
			}
			set
			{
				editingStatus = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(FormVisibility));
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
					bool res = People.Remove(SelectedPerson);
					UpdateModel();
				}
				else
				{
					CancelEditCommand.Execute(null);
				}

			});

			StartEditingCommand = new Command(() => {
				if (SelectedPerson != null)
				{
					EditedPerson = selectedPerson;
					EditStatus = EditingStatus.EditingExisting;
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

			StartAddingCommand = new Command(() =>
			{
				EditStatus = EditingStatus.AddingNew;
				EditedPerson = new Person() { FirstName="",LastName="" };
			});

			CancelEditCommand = new Command(() =>
			{
				EditStatus = EditingStatus.None;
			});

			SaveButtonCommand = new Command(() =>
			{
				if (EditStatus!=EditingStatus.None)
				{
					if (Data.Validate(EditedPerson))
					{
						if (EditStatus==EditingStatus.EditingExisting)
						{
							People[SelectedIndex] = EditedPerson;
							//RaisePropertyChanged(nameof(People));
						}
						else if (EditStatus==EditingStatus.AddingNew)
						{
							People.Add(EditedPerson);
						}
						EditStatus = EditingStatus.None;
						UpdateModel();
					}
					else
					{
						DisplayValidationError();
					}
				}
			});
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
