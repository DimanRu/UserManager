using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Data;
using UserManager.Data;
using UserManager.Model;
using UserManager.View;
using System.Text.Json;
using System.Configuration;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace UserManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand EditUserCommand { get; private set; }
        public RelayCommand RemoveUserCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand ImportCommand { get; private set; }
        public ApplicationContext Db { get; set; }
        public ObservableCollection<User> Users { get; }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindowViewModel(ApplicationContext db)
        {
            Db = db;
            Db.Users.Load();
            Users = Db.Users.Local.ToObservableCollection();

            EditUserCommand = new RelayCommand(obj =>
            {
                var user = obj is User ? (User)obj : new User() { Name = "Name of user"};
                var (Name, Department, Salary) = user;
                var editWindow = new EditUserView() { DataContext = new EditUserViewModel(user) };
                if (editWindow.ShowDialog() ?? false)
                {
                    Db.Users.Update(user);
                    Db.SaveChanges();
                }
                else
                {
                    user.Name = Name;
                    user.Department = Department;
                    user.Salary = Salary;
                }
            });

            RemoveUserCommand = new RelayCommand(obj =>
            {
                if (obj is User user)
                {
                    Db.Users.Remove(user);
                    Db.SaveChanges();
                }
            });

            ExportCommand = new RelayCommand(obj =>
            {
                var saveFD = new SaveFileDialog();
                if (saveFD.ShowDialog() ?? false)
                {
                    if (File.Exists(saveFD.FileName))
                        File.Delete(saveFD.FileName);
                    using (FileStream fs = new FileStream(saveFD.FileName, FileMode.OpenOrCreate))
                    {
                        JsonSerializer.SerializeAsync<IEnumerable<User>>(fs, Users, new JsonSerializerOptions() { WriteIndented = true });
                    }
                }
            });

            ImportCommand = new RelayCommand(obj =>
            {
                var openFD = new OpenFileDialog();
                if (openFD.ShowDialog() ?? false)
                {
                    using (FileStream fs = new FileStream(openFD.FileName, FileMode.Open))
                    {
                        var _users = JsonSerializer.Deserialize<List<User>>(fs, new JsonSerializerOptions() { WriteIndented = true });
                        foreach (var user in _users)
                        {
                            if (Users.FirstOrDefault(u => u.Id == user.Id) is User u)
                            {
                                u.Name = user.Name;
                                u.Department = user.Department;
                                u.Salary = user.Salary;
                                Db.Users.Update(u);
                            }
                            else
                            {
                                Db.Users.Add(user);
                            }
                         }
                    }
                }
            });
        }
    }
}
