using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalFinderApp.Models
{
    public class HospitalInfo
    {
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
        public string Sunday_oper_week { get; set; } // 일요일 오픈여부
        public string Exam_part { get; set; } // 진료과목
        public DateTime Regist_dt { get; set; } // 등록일
        public DateTime Update_dt { get; set; } // 수정일
        public double Lng { get; set; } // 경도
        public double Lat { get; set; } // 위도
    }
}
