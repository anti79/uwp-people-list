using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPApp
{
	public interface IView
	{
		Task<bool> DisplayConfirmationDialog(string text);
		void DisplayInfoDialog(string text);
	}
}
