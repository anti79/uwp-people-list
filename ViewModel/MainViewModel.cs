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


		public ICommand SaveButtonCommand { get; }
		public ICommand StartEditingCommand { get; }
		public ICommand StartAddingCommand { get; }
		public ICommand CancelEditCommand { get; }
		public ICommand DeleteCommand { get; set; }

		public MainViewModel(IView view)
		{
			People = new ObservableCollection<Person>(Data.People);
			this.view = view;

			DeleteCommand = new Command(async () =>
			{
				
				if (await view.DisplayConfirmationDialog("Delete the entry?"))
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
			CancelEditCommand.Execute(null);
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
