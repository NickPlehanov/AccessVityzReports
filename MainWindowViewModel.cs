using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using accessVityzReports.Models;
using accessVityzReports.Help;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using accessVityzReports.Hepl;
using accessVityzReports.Data;
using Microsoft.EntityFrameworkCore;


namespace accessVityzReports {
    class MainWindowViewModel : BaseViewModel {
        ReportbaseContext reportBaseContext;
        public MainWindowViewModel() {
            reportBaseContext = new ReportbaseContext();
            GetReports.Execute(null);
            ShowUserReports.Execute(null);
        }

        /// <summary>
        /// Коллекция для хранения списка логинОв
        /// </summary>
        private ObservableCollection<UsersReports> _UserReportsList;
        public ObservableCollection<UsersReports> UserReportsList {
            get => _UserReportsList;
            set {
                _UserReportsList = value;
                OnPropertyChanged(nameof(UsersReports));
            }
        }
        /// <summary>
        /// Выбранный логин
        /// </summary>
        /// 
        private UsersReports _SelectedUserReports;
        public UsersReports SelectedUserReports {
            get => _SelectedUserReports;
            set {
                _SelectedUserReports = value;
                OnPropertyChanged(nameof(SelectedUserReports));
            }
        }
        /// <summary>
        ///  логин
        /// </summary>
        private string _UserReportsName;
        public string UserReportsName {
            get => _UserReportsName;
            set {
                _UserReportsName = value;
                OnPropertyChanged(nameof(UserReportsName));
            }
        }
        /// <summary>
        /// Выбранный отчет 
        /// </summary>
        private Reports _SelectedReports;
        public Reports SelectedReports {
            get => _SelectedReports;
            set {
                _SelectedReports = value;
                OnPropertyChanged(nameof(SelectedReports));
            }
        }
        /// <summary>
        /// Коллекция для хранения списка ОТЧЕТОВ
        /// </summary>
        private ObservableCollection<Reports> _ReportsList;
        public ObservableCollection<Reports> ReportsList {
            get => _ReportsList;
            set {
                _ReportsList = value;
                OnPropertyChanged(nameof(ReportsList));
            }
        }
        /// <summary>
        /// Добавление
        /// </summary>
        private RelayCommand _AddUserReports;
        public RelayCommand AddUserReports => _AddUserReports ??= new RelayCommand(async obj => {
            if(string.IsNullOrEmpty(UserReportsName) && string.IsNullOrWhiteSpace(UserReportsName) && SelectedReports == null) { return; }
            
            var prov = reportBaseContext.UsersReports.FirstOrDefault(x => x.UsrLogin == UserReportsName && x.RptId == SelectedReports.RptId);
            if (prov == null) {

                UsersReports usr = new() {
                    Id = Guid.NewGuid(),
                    UsrLogin = UserReportsName,
                    RptId = SelectedReports.RptId

                };
                reportBaseContext.Entry<UsersReports>(usr).State = EntityState.Added;
                int result = await reportBaseContext.SaveChangesAsync();
                if (result.Equals(1)) {
                    UserReportsName = "";
                    SelectedReports = null;
                    
                }
            }
            
            ShowUserReports.Execute(null);
        } );

        /// <summary>
        /// Удаление доступа
        /// </summary>
        private RelayCommand _DelUserReports;
        public RelayCommand DelUserReports {
            get => _DelUserReports ??= new RelayCommand(async obj => {
                
                MessageBoxResult msg = MessageBox.Show("Удалить доступ?", "Удалить доступ?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (msg == MessageBoxResult.Yes) { 
                    if (obj == null)
                    return;
                    UsersReports ur = obj as UsersReports;
                    if (ur == null)
                    return;

                    
                    reportBaseContext.Entry<UsersReports>(ur).State = EntityState.Deleted;
                int result = await reportBaseContext.SaveChangesAsync();
                    if (result.Equals(1)) {
                        UserReportsList.Remove(ur);
                        UserReportsName = "";
                        SelectedUserReports = null;
                        //}
                    }                    
            }
                ShowUserReports.Execute(null);
            });
        }

        private RelayCommand _ShowUserReports;
        public RelayCommand ShowUserReports => _ShowUserReports ??= new RelayCommand(async obj => {
            //UserReportsList = new ObservableCollection<UsersReports>(await new ReportbaseContext().UsersReports.Include(x => x.Rpt).AsNoTracking().ToListAsync().ConfigureAwait(false));
            UserReportsList = new ObservableCollection<UsersReports>(reportBaseContext.UsersReports.Include(x => x.Rpt).AsNoTracking().ToList());
            OnPropertyChanged(nameof(UserReportsList));

        });

        private RelayCommand _GetReports;
        public RelayCommand GetReports {
            get => _GetReports ??= new RelayCommand(async obj => {
                ReportsList = new ObservableCollection<Reports>(reportBaseContext.Reports.AsNoTracking().ToList());
            });
        }
    }
}

