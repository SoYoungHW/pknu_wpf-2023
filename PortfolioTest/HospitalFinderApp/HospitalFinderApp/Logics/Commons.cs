using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HospitalFinderApp.Logics
{
    public class Commons
    {
        // 오류창 띄우는 메서드
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }

        // SQL 연결
        public static readonly string connString = "Data Source=localhost;" +
                                                   "Initial Catalog=Hospital;" +
                                                   "Persist Security Info=True;" +
                                                   "User ID=sa;" +
                                                   "Password=12345";
    }
}
