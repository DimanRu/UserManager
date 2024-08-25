using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using UserManager.Model;

namespace UserManager.ViewModel
{
    public class EditUserViewModel : INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();
        public bool HasErrors =>_validationErrors.Any();

        public RelayCommand SaveCommand { get; private set; }
        public User User { get; set; }

        public string Name
        {
            get => User.Name;
            set
            {
                User.Name = value;
                ClearErrors(nameof(Name));
                if (value == string.Empty)
                    AddError(nameof(Name), "Please enter a valid not empty string value");
            }
        }

        public EditUserViewModel(User user)
        {
            User = user;

            SaveCommand = new RelayCommand(obj =>
            {
                if (obj is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }, CanExecute);
        }
        private bool CanExecute()
        {
            return !HasErrors;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _validationErrors.GetValueOrDefault(propertyName, null);
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!_validationErrors.ContainsKey(propertyName))
            {
                _validationErrors.Add(propertyName, new List<string>());
            }

            _validationErrors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }

        public void ClearErrors(string propertyName)
        {
            if (_validationErrors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }
    }
}
