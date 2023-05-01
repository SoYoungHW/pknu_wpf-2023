using ControlzEx.Standard;
using HospitalFinderApp.Logics;
using HospitalFinderApp.Models;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        bool isChecked = false;
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

            if (string.IsNullOrEmpty(TxtHospitalName.Text))
            {
                try
                {
                    LoadHospital(CboHospitalKind.Text);

                }
                catch (Exception ex)
                {
                    await Commons.ShowMessageAsync("오류", $"API 조회오류 발생 {ex.Message}");
                    return;
                }
            }

            else
            {
                SearchHospital(TxtHospitalName.Text);
            }
        }

        #region < 기관분류로 조회 >
        private async void LoadHospital(string HospitalKind)
        {
            // 부산광역시 병원 OpenAPI 조회(공공API)
            string ApiKey = "QDJZmhj73H87njKl9vUpiGSWDpFUdSePWFpoIgP08Wi0EWPo5bqHd3Fq%2B%2FS6rm%2FSZQu2BzPSVsp3iF0MEPFJbw%3D%3D";
            string encoding_HospitalKind = HttpUtility.UrlEncode(HospitalKind, Encoding.UTF8);
            string OpenApiUri = $@"https://apis.data.go.kr/6260000/MedicInstitService/MedicalInstitInfo?serviceKey={ApiKey}" +
                                $@"&pageNo=1&numOfRows=500&resultType=json&instit_kind={encoding_HospitalKind}";
            string result = string.Empty;
            
            //  객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

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

            var jsonResult = JObject.Parse(result);

            var resultCode = Convert.ToString(jsonResult["MedicalInstitInfo"]["header"]["resultCode"]); 
            // 제일위에 이름및에 헤더밑에 result코드

            if (resultCode == "00")
            {
                var data = jsonResult["MedicalInstitInfo"]["body"]["items"]["item"];
                var json_array = data as JArray;

                var hospitalInfo = new List<HospitalInfo>();
                foreach (var item in json_array)
                {
                    hospitalInfo.Add(new HospitalInfo
                    {
                        Instit_nm = Convert.ToString(item["instit_nm"]),
                        Instit_kind = Convert.ToString(item["instit_kind"]),
                        Medical_instit_kind = Convert.ToString(item["medical_instit_kind"]),
                        Zip_code = Convert.ToInt32(item["zip_code"]),
                        Street_nm_addr = Convert.ToString(item["street_nm_addr"]),
                        Organ_loc = Convert.ToString(item["organ_loc"]),
                        Tel = Convert.ToString(item["tel"]),
                        Monday = Convert.ToString(item["Monday"]),
                        Tuesday = Convert.ToString(item["Tuesday"]),
                        Wednesday = Convert.ToString(item["Wednesday"]),
                        Thursday = Convert.ToString(item["Thursday"]),
                        Friday = Convert.ToString(item["Friday"]),
                        Saturday = Convert.ToString(item["Saturday"]),
                        Sunday = Convert.ToString(item["Sunday"]),
                        Holiday = Convert.ToString(item["holiday"]),
                        Sunday_oper_week = Convert.ToString(item["sunday_oper_week"]),
                        Exam_part = Convert.ToString(item["exam_part"]),
                        Regist_dt = Convert.ToDateTime(item["regist_dt"]),
                        Update_dt = Convert.ToDateTime(item["update_dt"]),
                        Lng = Convert.ToDouble(item["lng"]),
                        Lat = Convert.ToDouble(item["lat"])
                    });
                }
                this.DataContext = hospitalInfo;
                isChecked = false;
                StsResult.Content = $"OpenAPI {hospitalInfo.Count}건 조회완료";
            }
        }
        #endregion

        #region < 기관분류 + 기관명으로 검색>
        private async void SearchHospital(string HospitalName) // 검색결과가 있으면
        {
            // 부산광역시 병원/약국 OpenAPI 조회(공공API)
            string ApiKey = "QDJZmhj73H87njKl9vUpiGSWDpFUdSePWFpoIgP08Wi0EWPo5bqHd3Fq%2B%2FS6rm%2FSZQu2BzPSVsp3iF0MEPFJbw%3D%3D";
            string encoding_HospitalName = HttpUtility.UrlEncode(HospitalName, Encoding.UTF8);
            string OpenApiUri = $@"https://apis.data.go.kr/6260000/MedicInstitService/MedicalInstitInfo?serviceKey={ApiKey}" +
                                $@"&pageNo=1&resultType=json&instit_nm={encoding_HospitalName}";
            string result = string.Empty;

            //  객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

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

            var jsonResult = JObject.Parse(result);

            var resultCode = Convert.ToString(jsonResult["MedicalInstitInfo"]["header"]["resultCode"]); // 제일위에 이름및에 헤더밑에 result코드

            if (resultCode == "00")
            {
                var data = jsonResult["MedicalInstitInfo"]["body"]["items"]["item"];
                var json_array = data as JArray;

                var hospitalInfo = new List<HospitalInfo>();
                foreach (var item in json_array)
                {
                    hospitalInfo.Add(new HospitalInfo
                    {
                        Instit_nm = Convert.ToString(item["instit_nm"]),
                        Instit_kind = Convert.ToString(item["instit_kind"]),
                        Medical_instit_kind = Convert.ToString(item["medical_instit_kind"]),
                        Zip_code = Convert.ToInt32(item["zip_code"]),
                        Street_nm_addr = Convert.ToString(item["street_nm_addr"]),
                        Organ_loc = Convert.ToString(item["organ_loc"]),
                        Tel = Convert.ToString(item["tel"]),
                        Monday = Convert.ToString(item["Monday"]),
                        Tuesday = Convert.ToString(item["Tuesday"]),
                        Wednesday = Convert.ToString(item["Wednesday"]),
                        Thursday = Convert.ToString(item["Thursday"]),
                        Friday = Convert.ToString(item["Friday"]),
                        Saturday = Convert.ToString(item["Saturday"]),
                        Sunday = Convert.ToString(item["Sunday"]),
                        Holiday = Convert.ToString(item["holiday"]),
                        Sunday_oper_week = Convert.ToString(item["sunday_oper_week"]),
                        Exam_part = Convert.ToString(item["exam_part"]),
                        Regist_dt = Convert.ToDateTime(item["regist_dt"]),
                        Update_dt = Convert.ToDateTime(item["update_dt"]),
                        Lng = Convert.ToDouble(item["lng"]),
                        Lat = Convert.ToDouble(item["lat"])
                    });
                }
                this.DataContext = hospitalInfo;
                isChecked = false;
                StsResult.Content = $"OpenAPI {hospitalInfo.Count}건 조회완료";
            }
        }
        #endregion

        #region < 구글맵 검색 >
        private async void BtnSearchMap_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "병원을 선택해주세요");
                return;
            }

            else if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("오류", "병원을 하나만 선택해주세요");
                return;
            }

            var selItem = GrdResult.SelectedItem as HospitalInfo;

            var mapWindow = new MapWindow(selItem.Lat, selItem.Lng);
            mapWindow.Owner = this;
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();
        }
        #endregion

        #region < 네이버 검색 >
        private async void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "병원을 선택해주세요");
                return;
            }

            else if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("오류", "병원을 하나만 선택해주세요");
                return;
            }

            var selItem = GrdResult.SelectedItem as HospitalInfo;

            var detailWindow = new DetailWindow(selItem.Instit_nm);
            detailWindow.Owner = this;
            detailWindow.WindowStartupLocation= WindowStartupLocation.CenterOwner;
            detailWindow.ShowDialog();
        }
        #endregion

        #region < 즐겨찾기 추가 >
        private async void BtnCheckAdd_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "병원을 선택해주세요(복수선택가능)");
                return;
            }

            if (isChecked == true)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 병원입니다.");
                return;
            }

            // DB 데이터 입력
            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }

                    var query = @" INSERT INTO [dbo].[hospital]
                                             ( [instit_nm]
                                             , [instit_kind]
                                             , [medical_instit_kind]
                                             , [zip_code]
                                             , [street_nm_addr]
                                             , [tel]
                                             , [organ_loc]
                                             , [Monday]
                                             , [Tuesday]
                                             , [Wednesday]
                                             , [Thursday]
                                             , [Friday]
                                             , [Saturday]
                                             , [Sunday]
                                             , [holiday]
                                             , [sunday_oper_week]
                                             , [exam_part]
                                             , [regist_dt]
                                             , [update_dt]
                                             , [lng]
                                             , [lat] )
                                        VALUES
                                             ( @instit_nm
                                             , @instit_kind
                                             , @medical_instit_kind
                                             , @zip_code
                                             , @street_nm_addr
                                             , @tel
                                             , @organ_loc
                                             , @Monday
                                             , @Tuesday 
                                             , @Wednesday 
                                             , @Thursday 
                                             , @Friday
                                             , @Saturday 
                                             , @Sunday
                                             , @holiday 
                                             , @sunday_oper_week
                                             , @exam_part
                                             , @regist_dt
                                             , @update_dt
                                             , @lng
                                             , @lat )";

                    var insRes = 0;
                    foreach (HospitalInfo item in GrdResult.SelectedItems) 
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        
                        cmd.Parameters.AddWithValue("@instit_nm", item.Instit_nm);
                        cmd.Parameters.AddWithValue("@instit_kind", item.Instit_kind);
                        cmd.Parameters.AddWithValue("@medical_instit_kind", item.Medical_instit_kind);
                        cmd.Parameters.AddWithValue("@zip_code", item.Zip_code);
                        cmd.Parameters.AddWithValue("@street_nm_addr", item.Street_nm_addr);
                        cmd.Parameters.AddWithValue("@tel", item.Tel);
                        cmd.Parameters.AddWithValue("@organ_loc", item.Organ_loc);
                        cmd.Parameters.AddWithValue("@Monday", item.Monday);
                        cmd.Parameters.AddWithValue("@Tuesday", item.Tuesday);
                        cmd.Parameters.AddWithValue("@Wednesday", item.Wednesday);
                        cmd.Parameters.AddWithValue("@Thursday", item.Thursday);
                        cmd.Parameters.AddWithValue("@Friday", item.Friday);
                        cmd.Parameters.AddWithValue("@Saturday", item.Saturday);
                        cmd.Parameters.AddWithValue("@Sunday", item.Sunday);
                        cmd.Parameters.AddWithValue("@holiday", item.Holiday);
                        cmd.Parameters.AddWithValue("@sunday_oper_week", item.Sunday_oper_week);
                        cmd.Parameters.AddWithValue("@exam_part", item.Exam_part);
                        cmd.Parameters.AddWithValue("@regist_dt", item.Regist_dt);
                        cmd.Parameters.AddWithValue("@update_dt", item.Update_dt);
                        cmd.Parameters.AddWithValue("@lng", item.Lng);
                        cmd.Parameters.AddWithValue("@lat", item.Lat);

                        insRes += cmd.ExecuteNonQuery();
                    }

                        if (GrdResult.SelectedItems.Count == insRes)
                        {
                            await Commons.ShowMessageAsync("저장", "DB저장 성공");
                            StsResult.Content = $"즐겨찾기 {insRes}건 저장 완료";
                        }
                        else
                        {
                            await Commons.ShowMessageAsync("실패", "DB저장 오류");
                            return;
                        }
                    }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB입력 오류 {ex.Message}");
                return;
            }
        }
        #endregion

        #region < 즐겨찾기 보기 >
                private async void BtnCheckView_Click(object sender, RoutedEventArgs e)
                {
                    this.DataContext = null;
                    CboHospitalKind.SelectedValue = string.Empty;
                    TxtHospitalName.Text = string.Empty;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(Commons.connString))
                        {
                            if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }

                            var query = @"SELECT instit_nm
                                           , instit_kind
                                           , medical_instit_kind
                                           , zip_code
                                           , street_nm_addr
                                           , tel
                                           , organ_loc
                                           , Monday
                                           , Tuesday
                                           , Wednesday
                                           , Thursday
                                           , Friday
                                           , Saturday
                                           , Sunday
                                           , holiday
                                           , sunday_oper_week
                                           , exam_part
                                           , regist_dt
                                           , update_dt
                                           , lng
                                           , lat
                                        FROM hospital
                                       ORDER BY instit_kind ASC";

                            var cmd = new SqlCommand(query, conn);
                            var adapter = new SqlDataAdapter(cmd);
                            var ds = new DataSet();
                            adapter.Fill(ds, "hospital");

                            List<HospitalInfo> hospitalInfos = new List<HospitalInfo>();
                            foreach (DataRow row in ds.Tables["hospital"].Rows)
                            {
                                hospitalInfos.Add(new HospitalInfo
                                {
                                    Instit_nm = Convert.ToString(row["instit_nm"]),
                                    Instit_kind = Convert.ToString(row["instit_kind"]),
                                    Medical_instit_kind = Convert.ToString(row["medical_instit_kind"]),
                                    Zip_code = Convert.ToInt32(row["zip_code"]),
                                    Street_nm_addr = Convert.ToString(row["street_nm_addr"]),
                                    Tel = Convert.ToString(row["tel"]),
                                    Organ_loc = Convert.ToString(row["organ_loc"]),
                                    Monday = Convert.ToString(row["Monday"]),
                                    Tuesday = Convert.ToString(row["Tuesday"]),
                                    Wednesday = Convert.ToString(row["Wednesday"]),
                                    Thursday = Convert.ToString(row["Thursday"]),
                                    Friday = Convert.ToString(row["Friday"]),
                                    Saturday = Convert.ToString(row["Saturday"]),
                                    Sunday = Convert.ToString(row["Sunday"]),
                                    Holiday = Convert.ToString(row["holiday"]),
                                    Sunday_oper_week = Convert.ToString(row["sunday_oper_week"]),
                                    Exam_part = Convert.ToString(row["exam_part"]),
                                    Regist_dt = Convert.ToDateTime(row["regist_dt"]),
                                    Update_dt = Convert.ToDateTime(row["update_dt"]),
                                    Lng = Convert.ToInt32(row["lng"]),
                                    Lat = Convert.ToInt32(row["lat"])
                                });
                            }
                            this.DataContext = hospitalInfos;
                            isChecked = true;
                            StsResult.Content = $"즐겨찾기 {hospitalInfos.Count}건 조회 완료";
                        }
                    }
                    catch (Exception ex)
                    {
                        await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
                        return;
                    }
                }
        #endregion

        #region < 즐겨찾기 삭제 >
        private async void BtnCheckDel_Click(object sender, RoutedEventArgs e)
        {
            if (isChecked == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if ( GrdResult.SelectedItems.Count == 0 )
            {
                await Commons.ShowMessageAsync("오류", "삭제할 병원을 선택해주세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }

                    var query = @"DELETE FROM hospital WHERE Tel = @tel";
                    var delRes = 0;

                    foreach (HospitalInfo item in GrdResult.SelectedItems) 
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@tel", item.Tel);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 성공");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}.");
                return;
            }

            BtnCheckView_Click(sender, e);
        }
        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CboHospitalKind.Focus(); // 콤보박스에 포커스 셋
        }

        private void CboHospitalKind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                BtnSearchHospital_Click(sender, e);
            }
            if(e.Key == Key.Tab)
            {
                TxtHospitalName.Focus();
            }
        }

        private void TxtHospitalName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchHospital_Click(sender, e);
            }
        }
    }
}
