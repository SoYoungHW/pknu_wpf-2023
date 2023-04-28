using ControlzEx.Standard;
using HospitalFinderApp.Logics;
using HospitalFinderApp.Models;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HospitalFinderApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void BtnSearchHospital_Click(object sender, RoutedEventArgs e)
        {
            // 의료기관 종류 선택

            if (string.IsNullOrEmpty(CboHospitalKind.Text))
            {
                await Commons.ShowMessageAsync("오류", "찾고싶은 의료기관을 선택하세요");
                return;
            } // 선택안하면
            

            try
            {
                SearchHospital(CboHospitalKind.Text);
                
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"API 조회오류 발생 {ex.Message}");
                return;
            }
        }

        private async void SearchHospital(string HospitalKind)
        {
            // 부산광역시 병원/약국 OpenAPI 조회(공공API)
            string ApiKey = "QDJZmhj73H87njKl9vUpiGSWDpFUdSePWFpoIgP08Wi0EWPo5bqHd3Fq%2B%2FS6rm%2FSZQu2BzPSVsp3iF0MEPFJbw%3D%3D";
            string encoding_HospitalKind = HttpUtility.UrlEncode(HospitalKind, Encoding.UTF8);
            string OpenApiUri = $@"https://apis.data.go.kr/6260000/MedicInstitService/MedicalInstitInfo?serviceKey={ApiKey}" +
                                $@"&pageNo=1&numOfRows=10&resultType=json&instit_kind={encoding_HospitalKind}";

            // https://apis.data.go.kr/6260000/MedicInstitService/MedicalInstitInfo?serviceKey=QDJZmhj73H87njKl9vUpiGSWDpFUdSePWFpoIgP08Wi0EWPo5bqHd3Fq%2B%2FS6rm%2FSZQu2BzPSVsp3iF0MEPFJbw%3D%3D&pageNo=1&numOfRows=10&resultType=json&instit_kind=%ED%95%9C%EC%9D%98%EC%9B%90
            string result = string.Empty;

            //  객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 밑에 밑에 밑에
            try
            {
                req = WebRequest.Create(OpenApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 (비동기) 응답객체에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json 결과 텍스트에 저장

                Debug.WriteLine(result);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result를 json으로 변경

            //var jsonResult = JObject.Parse(result);

            //var resultCode = Convert.ToString(jsonResult["MedicalInstitInfo"]["header"]["resultCode"]); // 제일위에 이름및에 헤더밑에 result코드

            //if (resultCode == "00")
            //{
            //    var data = jsonResult["MedicalInstitInfo"]["body"]["items"]["item"];
            //    var json_array = data as JArray;

            //    var hospitalInfo = new List<HospitalInfo>();
            //    foreach (var item in json_array)
            //    {
            //        hospitalInfo.Add(new HospitalInfo
            //        {
            //            Instit_nm = Convert.ToString(item["instit_nm"]),
            //            Instit_kind = Convert.ToString(item["instit_kind"]),
            //            Medical_instit_kind = Convert.ToString(item["medical_instit_kind"]),
            //            Zip_code = Convert.ToInt32(item["zip_code"]),
            //            Street_nm_addr = Convert.ToString(item["street_nm_addr"]),
            //            Organ_loc = Convert.ToString(item["organ_loc"]),
            //            Tel = Convert.ToString(item["tel"]),
            //            Monday = Convert.ToString(item["Monday"]),
            //            Tuesday = Convert.ToString(item["Tuesday"]),
            //            Wednesday = Convert.ToString(item["Wednesday"]),
            //            Thursday = Convert.ToString(item["Thursday"]),
            //            Friday = Convert.ToString(item["Friday"]),
            //            Saturday = Convert.ToString(item["Saturday"]),
            //            Sunday = Convert.ToString(item["Sunday"]),
            //            Holiday = Convert.ToString(item["holiday"]),
            //            Sunday_oper_week = Convert.ToBoolean(item["sunday_oper_week"]),
            //            Exam_part = Convert.ToString(item["exam_part"]),
            //            Regist_dt = Convert.ToDateTime(item["regist_dt"]),
            //            Update_dt = Convert.ToDateTime(item["update_dt"]),
            //            Lng = Convert.ToDouble(item["lng"]),
            //            Lat = Convert.ToDouble(item["lat"])
            //        });
            //    }
            //    this.DataContext = hospitalInfo;
            //}
        }
    }
}
