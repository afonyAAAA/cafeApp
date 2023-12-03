using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Dialogs;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Media.Imaging;
using System.Reactive.Linq;
using Npgsql.Replication;

namespace caffeApp.ViewModels.Admin
{
    public class RegistrationViewModel : ViewModelBase
    {
        public override string? UrlPathSegment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IScreen HostScreen { get; set; }
        public override ViewModelActivator Activator { get; set; }
      
        private string _firstName;
        private string _secondName;
        private string _surname;
        private string _login;
        private string _password;
        private string _selectedImageUserPath;
        private string _selectedImageAgreementPath;
        private Bitmap _userImageBitmap;
        private Bitmap _aggrementImageBitmap;

        public Bitmap UserImageBitmap
        {
            get => _userImageBitmap;
            set => this.RaiseAndSetIfChanged(ref _userImageBitmap, value);
        }

        public Bitmap AggrementImageBitmap {
            get => _aggrementImageBitmap;
            set => this.RaiseAndSetIfChanged(ref _aggrementImageBitmap, value);
        }

        public string FirstName {
            get => _firstName; 
            set => this.RaiseAndSetIfChanged(ref _firstName, value); 
        }

        public string SecondName { 
            get => _secondName; 
            set => this.RaiseAndSetIfChanged(ref _secondName, value);
        }

        public string Surname { 
            get => _surname; 
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public string Login { 
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        public string Password {
            get => _password; 
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string SelectedImageUserPath {
            get => _selectedImageUserPath;
            set => this.RaiseAndSetIfChanged(ref _selectedImageUserPath, value);
        }

        public string SelectedImageAgreementPath
        {
            get => _selectedImageAgreementPath;
            set => this.RaiseAndSetIfChanged(ref _selectedImageAgreementPath, value);
        }

        

        public ReactiveCommand<Unit, Unit> OpenDialogFileUserImage { get; }

        public ReactiveCommand<Unit, Unit> OpenDialogFileAgreementImage{ get; }
       

        [Obsolete]
        public RegistrationViewModel(IScreen screen)
        {
            Activator = new ViewModelActivator();

            HostScreen = screen;

            OpenDialogFileUserImage = ReactiveCommand.Create(() =>
            {
                SelectedImageUserPath = openFilePicker();
            });

            OpenDialogFileAgreementImage = ReactiveCommand.Create(() =>
            {
                SelectedImageAgreementPath = openFilePicker();
            });

            this.WhenAnyValue(x => x.SelectedImageUserPath).Subscribe(tuple =>
            {
                if(tuple != null)
                {
                    UserImageBitmap = new Bitmap(tuple);
                }
            });

            this.WhenAnyValue(x => x.SelectedImageAgreementPath).Subscribe(tuple =>
            {
                if(tuple != null)
                {
                    AggrementImageBitmap = new Bitmap(tuple);
                }
            });

            this.WhenActivated(disposables => {

                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                    })
                    .DisposeWith(disposables);
            });
        }

        [Obsolete]
        private async Task openFilePicker()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filters = GetImageFileFilters().ToList()
            };

            var result = dialog.ShowAsync(GetWindow());
            string path = null;

            await result.ContinueWith(task =>
            {
                if (task.Result != null && task.Result.Any())
                {
                    // Ваш код обработки выбранного файла
                     path = task.Result.First();
                }
            });

            Task.FromResult(path);
        }

        [Obsolete]
        private FileDialogFilter[] GetImageFileFilters()
        {
            // Фильтры для изображений
            return new[]
            {
                new FileDialogFilter
                {
                    Name = "Изображения",
                    Extensions = new List<string> { "jpg", "jpeg", "png", "gif", "bmp" }
                },
                new FileDialogFilter
                {
                    Name = "Все файлы",
                    Extensions = new List<string> { "*" }
                }
            };
        }

        private Window GetWindow()
        {
            // Метод, который возвращает текущее окно
            // Вам, возможно, нужно адаптировать его в зависимости от вашей архитектуры
            // Если у вас есть доступ к текущему окну, используйте его
            // В данном случае просто возвращается null, а значит, окно будет выбираться автоматически
            return new Window();
        }

    }
}
