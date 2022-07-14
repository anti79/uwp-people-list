using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UWPApp.Model
{
	static class Data
	{
		public static List<Person> People { get; set; }
		static Data()
		{
			People = new List<Person>() {
				new Person { FirstName = "Alice", LastName = "A.", Age = 15 },
				new Person { FirstName = "Bob", LastName = "B.", Age = 20 },
				new Person { FirstName = "Charlie", LastName = "C.", Age = 25 },


				};

		}
		public static bool Validate(Person person)
		{
			Regex nameRegex = new Regex("[A-Za-z]+");
			if (person.FirstName.Length < 1 || person.FirstName.Length > 15) return false;
			if (!nameRegex.IsMatch(person.FirstName) || !nameRegex.IsMatch(person.LastName)) return false;
			if (person.Age < 0 || person.Age > 116) return false;
			return true;
		}
	}
}
