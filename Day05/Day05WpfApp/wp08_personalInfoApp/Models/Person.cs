using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp.Models
{
    internal class Person
    {
        // 외부에서 접근불가
        private string firstName;
        private string lastName;
        private string email;
        private DateTime date;

        // 프로퍼티
        public string FirstName { get; set; } // 오토프로퍼티
        public string LastName { get; set; }
        
        public string Email 
        { 
            get => email;        
            set 
            {
                if (Commons.isValidEmail(value) != true) // 이메일 형식 불일치
                {
                    throw new Exception("유효하지 않은 이메일형식");
                }
                else
                {
                    email = value;
                }
            } 
        }
        
        public DateTime Date 
        { 
            get => date; 
            set 
            {
                var result = Commons.getAge(value);
                if (result > 120 || result <= 0) 
                {
                    throw new Exception("유효하지 않은 생년월일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAdult
        {
            get => Commons.getAge(date) > 18; // 19살 이상이면 
            // 람다
        }

        public bool IsBirthday
        {
            get
            {
                return DateTime.Now.Month == date.Month 
                    && DateTime.Now.Day == date.Day; // 오늘하고 월일같으면 생일
                // 일반
            }
        }

        public string Zodiac
        {
            get => Commons.getZodiac(date); // 12지로 띠를 받아옴
        }

        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }
    }
}
