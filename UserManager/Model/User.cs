using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace UserManager.Model
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string? _department;
        public string? Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged("Department");
            }
        }

        private uint _salary;
        public uint Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                OnPropertyChanged("Salary");
            }
        }

        public User()
        {
            Salary = 0;
        }

        public void Deconstruct(out string name, out string? department, out uint salary) => (name, department, salary) = (Name, Department, Salary);

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
