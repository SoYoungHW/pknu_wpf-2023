using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using wp11_MovieFinder.Logics;
using wp11_MovieFinder.Mondels;

namespace wp11_MovieFinder
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

        private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            await Commons.ShowMessageAsync("네이버 영화", "네이버 영화 사이트로 접속합니다.");
        }

        // 검색버튼, 네이버 API 사용 영화 검색
        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력해주세요."); // 검색인증
                return;
            }
            //else if (TxtMovieName.Text.Length < 2)
            //{
            //    await Commons.ShowMessageAsync("검색", "검색어를 2자 이상 입력해주세요.");
            //    return;
            //}

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }


        // 텍스트 박스에서 키를 입력할때 엔터를 누르면 검색되도록 시작
        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchMovie_Click(sender, e);
            }
        }

        // 실제 검색 메서드
        private void SearchMovie(string movieName)
        {
            string TMDB_apiKey = "b3a43f11f114b88a79e3706ab896be97";
            string enconding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string OpenApiUri = $@"https://api.themoviedb.org/3/search/movie?api_key={TMDB_apiKey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={enconding_movieName}";
            string result = string.Empty;

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // TMDB API 요청
            try
            {
                req = WebRequest.Create(OpenApiUri); // URL을 넣어서 객체를 생성
                res = req.GetResponse(); // 요청한 결과를 응답객체에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json 결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string -> json

            var totalResult = Convert.ToInt32(jsonResult["total_results"]);
            // await Commons.ShowMessageAsync("검색결과", totalResult.ToString());
            
            // items를 데이터 그리드에 표시
            var items = jsonResult["results"];
            var json_array = items as JArray;

            var movieItems = new List<MovieItem>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var MovieItem = new MovieItem()
                {
                    Id = Convert.ToInt32(val["id"]),
                    Title = Convert.ToString(val["title"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])
                    // Adult = Convert.ToBoolean(val["adult"])
                };
                movieItems.Add(MovieItem);
            }

            this.DataContext = movieItems;
        }


    }
}
