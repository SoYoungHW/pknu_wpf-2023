﻿using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wp11_MovieFinder.Logics;
using wp11_MovieFinder.Mondels;

namespace wp11_MovieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false; // false -> openAPI로 검색해온 결과, true -> 즐겨찾기 보기
        public MainWindow()
        {
            InitializeComponent();
        }

        //private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        //{
        //    await Commons.ShowMessageAsync("네이버 영화", "네이버 영화 사이트로 접속합니다.");
        //}

        // 검색버튼, API 사용 영화 검색
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
        private async void SearchMovie(string movieName)
        {
            string TMDB_apiKey = "TMDB키입력";
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
                res = await req.GetResponseAsync(); // 요청한 결과를 (비동기) 응답객체에 할당
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
                    Vote_Average = Convert.ToDouble(val["vote_average"]),
                    Adult = Convert.ToBoolean(val["adult"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Overview = Convert.ToString(val["overview"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Poster_Path = Convert.ToString(val["poster_path"])
                };
                movieItems.Add(MovieItem);
            }

            this.DataContext = movieItems;
            isFavorite = false; // 즐겨찾기가 아니라고 알림
            StsResult.Content = $"OpenAPI {movieItems.Count}건 조회완료";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus(); // 텍스트박스에 포커스 셋
        }

        // 그리드에서 셀을 선택하면
        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                string posterPath = string.Empty;

                if (GrdResult.SelectedItem is MovieItem) // openAPI로 검색된 영화포스터
                {
                    var movie = GrdResult.SelectedItem as MovieItem;
                    posterPath = movie.Poster_Path;
                }
                else if (GrdResult.SelectedItem is FavoriteMovieItem) // 즐겨찾기 DB에서 가져온 영화포스터
                {
                    var movie = GrdResult.SelectedItem as FavoriteMovieItem;
                    posterPath = movie.Poster_Path;
                }

                // var movie = GrdResult.SelectedItem as MovieItem;
                Debug.WriteLine(posterPath);
                if (string.IsNullOrEmpty(posterPath)) // 포스터 이미지가 없으면 노픽쳐 이미지 
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else // 포스터 이미지 경로가 있으면
                {
                    var base_url = "https://image.tmdb.org/t/p/w600_and_h900_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{posterPath}", UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", $"이미지로드 오류발생");
            }

        }

        // 영화 예고편 유튜브에서 보기
        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택해주세요.");
                return;
            }
            else if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택해주세요.");
                return;
            }

            string movieName = string.Empty;
            if (GrdResult.SelectedItems is MovieItem)
            {
                var movie = GrdResult.SelectedItem as MovieItem;
                movieName = movie.Title;
            }
            else if (GrdResult.SelectedItem is FavoriteMovieItem)
            {
                var movie = GrdResult.SelectedItem as FavoriteMovieItem;
                movieName = movie.Title;
            }
            // movieName = (GrdResult.SelectedItem as MovieItem).Title;

            // await Commons.ShowMessageAsync("유튜브", $"예고편 볼 영화 {movieName}");
            var trailerWindow = new TrailerWindow(movieName);
            trailerWindow.Owner = this; // TrailerWindow의 부모는 MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 부모창의 정중앙에 위치
            // trailerWindow.Show(); // 모달리스로 창을 열면 부모창을 손댈수 있기 때문에
            trailerWindow.ShowDialog(); // 모달창
        }

        // 검색 결과중에서 좋아하는 영화 저장, 즐겨찾기 추가
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택해주세요.(복수선택가능)");
                return;
            }

            if (isFavorite == true)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다.");
                return;
            }

            /* List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();
            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                var favoriteMovie = new FavoriteMovieItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Original_Language = item.Original_Language,
                    Original_Title = item.Original_Title,
                    Adult = item.Adult,
                    Overview = item.Overview,
                    Release_Date = item.Release_Date,
                    Vote_Average = item.Vote_Average,
                    Popularity = item.Popularity,
                    Poster_Path = item.Poster_Path,
                    Reg_Date = DateTime.Now // 지금 저장하는 일시
                };

                list.Add(favoriteMovie);
            } */

            #region < MySQL 테스트 >
            /* 
            try
            {
                // MySQL DB 데이터 입력(테스트용)
                using (MySqlConnection conn = new MySqlConnection(Commons.MyconnString))
                {
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }

                    var query = @"INSERT INTO FavoriteMovieItem
                                            ( Id
                                            , Title
                                            , Original_Title
                                            , Release_Date
                                            , Original_Language
                                            , Adult
                                            , Popularity
                                            , Vote_Average
                                            , Overview
                                            , Poster_Path
                                            , Reg_Date )
                                       VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_Date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Overview
                                            , @Poster_Path
                                            , @Reg_Date )";

                    var insRes = 0;
                    foreach (FavoriteMovieItem item in list)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Reg_Date", item.Reg_Date);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    //if (list.Count == insRes)
                    //{
                    //    await Commons.ShowMessageAsync("저장", "MySQL DB저장성공");
                    //}
                    //else
                    //{
                    //    await Commons.ShowMessageAsync("실패", "DB저장오류 : 관리자에게 문의해주세요");
                    //}
                    // var result = cmd.ExecuteScalar();
                    // await Commons.ShowMessageAsync("데이터 갯수", result.ToString());
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            } */
            #endregion

            // SQL Sever
            try
            {
                // DB 데이터 입력
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }

                    var query = @"INSERT INTO [dbo].[FavoriteMovieItem]
                                            ( [Id]
                                            , [Title]
                                            , [Original_Title]
                                            , [Release_Date]
                                            , [Original_Language]
                                            , [Adult]
                                            , [Popularity]
                                            , [Vote_Average]
                                            , [Overview]
                                            , [Poster_Path]
                                            , [Reg_Date] )
                                       VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_Date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Overview
                                            , @Poster_Path
                                            , @Reg_Date )";

                    var insRes = 0;
                    foreach (MovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn); // OpenAPI로 조회된 결과라서 MovieItem

                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Reg_Date", DateTime.Now);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                        StsResult.Content = $"즐겨찾기 {insRes}건 저장완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("실패", "DB저장오류 : 관리자에게 문의해주세요");
                    }
                    // var result = cmd.ExecuteScalar();
                    // await Commons.ShowMessageAsync("데이터 갯수", result.ToString());
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
        }

        // 즐겨찾기 보기 
        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = string.Empty;

            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }

                    var query = @"SELECT Id
                                       , Title
                                       , Original_Title
                                       , Release_Date
                                       , Original_Language
                                       , Adult
                                       , Popularity
                                       , Vote_Average
                                       , Overview
                                       , Poster_Path
                                       , Reg_Date
                                    FROM FavoriteMovieItem
                                    ORDER BY Id ASC";

                    var cmd = new SqlCommand(query, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteMovieItem");
                    
                    foreach (DataRow dr in dSet.Tables["FavoriteMovieItem"].Rows)
                    {
                        list.Add(new FavoriteMovieItem
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Title = Convert.ToString(dr["Title"]),
                            Release_Date = Convert.ToString(dr["Release_Date"]),
                            Original_Title = Convert.ToString(dr["Original_Title"]),
                            Original_Language = Convert.ToString(dr["Original_Language"]),
                            Adult = Convert.ToBoolean(dr["Adult"]),
                            Popularity = Convert.ToDouble(dr["Popularity"]),
                            Vote_Average = Convert.ToDouble(dr["Vote_Average"]),
                            Overview = Convert.ToString(dr["Overview"]),
                            Poster_Path = Convert.ToString(dr["Poster_Path"]),
                            Reg_Date = Convert.ToDateTime(dr["Reg_Date"])
                        });
                    }
                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
            }
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false ) // 검색해온거
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0 ) 
            {
                await Commons.ShowMessageAsync("오류", "삭제할 영화를 선택해주세요.");
                return;
            }

            try // 삭제
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }

                    var query = @"DELETE FROM FavoriteMovieItem
                                  WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FavoriteMovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }
                    
                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 성공");
                        StsResult.Content = $"즐겨찾기 {delRes}건 삭제완료"; // 화면에 안나옴(바로 다시 조회하니까)
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 일부성공");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e); // 즐겨찾기 보기 이벤트핸들러를 한번 실행
        }
    }
}

