using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp09_caliburnApp.Models;

namespace wp09_caliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        // Caliburn version업으로 변경
        private string firstName = "Gildong";
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName); // 속성값이 변경된걸 시스템에 알려주는 역할
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FullName));
            }
        }

        private string lastName = "Hong";
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(() => FullName); // 변화통보
            }
        }

        public string FullName
        {
            get => $"{lastName} {firstName}";
        }

        // 콤보박스에 바인딩할 속성
        // 이런 곳에서는 var를 쓸수없음
        private BindableCollection<Person> managers = new BindableCollection<Person>();

        public BindableCollection<Person> Managers
        {
            get => managers;
            set => managers = value;
        }

        // 콤보박스에 선택된 값을 지정할 속성
        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = selectedManager.LastName;
                FirstName = selectedManager.FirstName;
                NotifyOfPropertyChange(nameof(selectedManager));
            }
        }

        public MainViewModel()
        {
            // DB를 사용하면 여기서 DB접속 -> 데이터를 SELECT까지 해와야
            // 지금은 임의로 작성
            Managers.Add(new Person { FirstName = "Jonh", LastName = "Carmack" });
            Managers.Add(new Person { FirstName = "Steve", LastName = "Jobs" });
            Managers.Add(new Person { FirstName = "Bill", LastName = "Gates" });
            Managers.Add(new Person { FirstName = "Elon", LastName = "Musk" });
        }

        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;   
            Managers.Clear();
        }

        // 메서드와 이름동일하게 앞에 Can을 붙임
        public bool CanClearName
        {
            get => !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }
    }
}
