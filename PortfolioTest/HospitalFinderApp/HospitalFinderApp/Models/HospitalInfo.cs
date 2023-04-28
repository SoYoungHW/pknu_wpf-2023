using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalFinderApp.Models
{
    public class HospitalInfo
    {
        //"instit_nm": "동아대학교병원", 
        //    "instit_kind": "상급종합병원",
        //    "medical_instit_kind": "권역응급의료센터",
        //    "zip_code": "49201",
        //    "street_nm_addr": "부산광역시 서구 대신공원로 26 (동대신동3가)",
        //    "tel": "051-240-2400",
        //    "organ_loc": "버스정류장: 동아대 구덕캠퍼스",
        //    "Monday": "08:00~17:00",
        //    "Tuesday": "08:00~17:00",
        //    "Wednesday": "08:00~17:00",
        //    "Thursday": "08:00~17:00",
        //    "Friday": "08:00~17:00",
        //    "Saturday": "~",
        //    "Sunday": "~",
        //    "holiday": "~",
        //    "sunday_oper_week": "",
        //    "exam_part": "가정의학과,구강안면외과,내과,마취통증의학과,방사선종양학과,병리과,비뇨의학과,산부인과,성형외과,소아청소년과,신경과,신경외과,안과,영상의학과,외과,응급의학과,이비인후과,작업환경의학과,재활의학과,정신건강의학과,정형외과,진단검사의학과,피부과,핵의학과,흉부외과",
        //    "regist_dt": "2008-12-05",
        //    "update_dt": "2021-02-11",
        //    "lng": "129.0180276",
        //    "lat": "35.11954225"
        // 총 21개

        public string Instit_nm { get; set; } // 기관명
        public string Instit_kind { get; set; } // 기관분류
        public string Medical_instit_kind { get; set; } // 의료기관분류
        public int Zip_code { get; set; } // 우편번호
        public string Street_nm_addr { get; set; } // 주소
        public string Organ_loc { get; set; } // 근처 버스정류장
        public string Tel { get; set; } // 전화번호
        public string Monday { get; set; } // 월요일 시간(나중에 평일기준될지도)
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; } // 토요일 시간
        public string Sunday { get; set; }
        public string Holiday { get; set; }
        public bool Sunday_oper_week { get; set; } // 일요일 오픈여부
        public string Exam_part { get; set; } // 진료과목
        public DateTime Regist_dt { get; set; } // 등록일
        public DateTime Update_dt { get; set; } // 수정일
        public double Lng { get; set; } // 경도
        public double Lat { get; set; } // 위도




    }
}
