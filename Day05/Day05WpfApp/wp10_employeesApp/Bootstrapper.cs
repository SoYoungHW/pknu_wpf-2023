using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
// using wp09_caliburnApp.ViewModels;
using wp10_employeesApp.ViewModels;

namespace wp10_employeesApp
{
    // Caliburn으로 MVVM 실행할때 주요설정 진행
    internal class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() 
        {
            Initialize(); // Caliburn MVVM 초기화
        }

        // 시작한후에 초기화 진행
        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            // base.OnStartup(sender, e); // 부모클래스 실행은 주석처리
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
