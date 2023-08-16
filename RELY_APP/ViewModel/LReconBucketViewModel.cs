using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReconBucketViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "BatchNumber")]
        [Required(ErrorMessage = "BatchNumber is required")]
        public int BatchNumber { get; set; }

        [Display(Name = "Ordinal")]
        public Nullable<int> Ordinal { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "ProductCode")]
        [Required(ErrorMessage = "ProductCode is required")]
        [MaxLength(255, ErrorMessage = " ProductCode can be maximum 255 characters")]
        public string ProductCode { get; set; }

        [Display(Name = "SysCatId")]
        [Required(ErrorMessage = "SysCatId is required")]
        public int SysCatId { get; set; }

        [Display(Name = "A001")]
        [MaxLength(255, ErrorMessage = " A001 can be maximum 255 characters")]
        public string A001 { get; set; }

        [Display(Name = "A002")]
        [MaxLength(255, ErrorMessage = " A002 can be maximum 255 characters")]
        public string A002 { get; set; }

        [Display(Name = "A003")]
        [MaxLength(255, ErrorMessage = " A003 can be maximum 255 characters")]
        public string A003 { get; set; }

        [Display(Name = "A004")]
        [MaxLength(255, ErrorMessage = " A004 can be maximum 255 characters")]
        public string A004 { get; set; }

        [Display(Name = "A005")]
        [MaxLength(255, ErrorMessage = " A005 can be maximum 255 characters")]
        public string A005 { get; set; }

        [Display(Name = "A006")]
        [MaxLength(255, ErrorMessage = " A006 can be maximum 255 characters")]
        public string A006 { get; set; }

        [Display(Name = "A007")]
        [MaxLength(255, ErrorMessage = " A007 can be maximum 255 characters")]
        public string A007 { get; set; }

        [Display(Name = "A008")]
        [MaxLength(255, ErrorMessage = " A008 can be maximum 255 characters")]
        public string A008 { get; set; }

        [Display(Name = "A009")]
        [MaxLength(255, ErrorMessage = " A009 can be maximum 255 characters")]
        public string A009 { get; set; }

        [Display(Name = "A010")]
        [MaxLength(255, ErrorMessage = " A010 can be maximum 255 characters")]
        public string A010 { get; set; }

        [Display(Name = "A011")]
        [MaxLength(255, ErrorMessage = " A011 can be maximum 255 characters")]
        public string A011 { get; set; }

        [Display(Name = "A012")]
        [MaxLength(255, ErrorMessage = " A012 can be maximum 255 characters")]
        public string A012 { get; set; }

        [Display(Name = "A013")]
        [MaxLength(255, ErrorMessage = " A013 can be maximum 255 characters")]
        public string A013 { get; set; }

        [Display(Name = "A014")]
        [MaxLength(255, ErrorMessage = " A014 can be maximum 255 characters")]
        public string A014 { get; set; }

        [Display(Name = "A015")]
        [MaxLength(255, ErrorMessage = " A015 can be maximum 255 characters")]
        public string A015 { get; set; }

        [Display(Name = "A016")]
        [MaxLength(255, ErrorMessage = " A016 can be maximum 255 characters")]
        public string A016 { get; set; }

        [Display(Name = "A017")]
        [MaxLength(255, ErrorMessage = " A017 can be maximum 255 characters")]
        public string A017 { get; set; }

        [Display(Name = "A018")]
        [MaxLength(255, ErrorMessage = " A018 can be maximum 255 characters")]
        public string A018 { get; set; }

        [Display(Name = "A019")]
        [MaxLength(255, ErrorMessage = " A019 can be maximum 255 characters")]
        public string A019 { get; set; }

        [Display(Name = "A020")]
        [MaxLength(255, ErrorMessage = " A020 can be maximum 255 characters")]
        public string A020 { get; set; }

        [Display(Name = "A021")]
        [MaxLength(255, ErrorMessage = " A021 can be maximum 255 characters")]
        public string A021 { get; set; }

        [Display(Name = "A022")]
        [MaxLength(255, ErrorMessage = " A022 can be maximum 255 characters")]
        public string A022 { get; set; }

        [Display(Name = "A023")]
        [MaxLength(255, ErrorMessage = " A023 can be maximum 255 characters")]
        public string A023 { get; set; }

        [Display(Name = "A024")]
        [MaxLength(255, ErrorMessage = " A024 can be maximum 255 characters")]
        public string A024 { get; set; }

        [Display(Name = "A025")]
        [MaxLength(255, ErrorMessage = " A025 can be maximum 255 characters")]
        public string A025 { get; set; }

        [Display(Name = "A026")]
        [MaxLength(255, ErrorMessage = " A026 can be maximum 255 characters")]
        public string A026 { get; set; }

        [Display(Name = "A027")]
        [MaxLength(255, ErrorMessage = " A027 can be maximum 255 characters")]
        public string A027 { get; set; }

        [Display(Name = "A028")]
        [MaxLength(255, ErrorMessage = " A028 can be maximum 255 characters")]
        public string A028 { get; set; }

        [Display(Name = "A029")]
        [MaxLength(255, ErrorMessage = " A029 can be maximum 255 characters")]
        public string A029 { get; set; }

        [Display(Name = "A030")]
        [MaxLength(255, ErrorMessage = " A030 can be maximum 255 characters")]
        public string A030 { get; set; }

        [Display(Name = "A031")]
        [MaxLength(255, ErrorMessage = " A031 can be maximum 255 characters")]
        public string A031 { get; set; }

        [Display(Name = "A032")]
        [MaxLength(255, ErrorMessage = " A032 can be maximum 255 characters")]
        public string A032 { get; set; }

        [Display(Name = "A033")]
        [MaxLength(255, ErrorMessage = " A033 can be maximum 255 characters")]
        public string A033 { get; set; }

        [Display(Name = "A034")]
        [MaxLength(255, ErrorMessage = " A034 can be maximum 255 characters")]
        public string A034 { get; set; }

        [Display(Name = "A035")]
        [MaxLength(255, ErrorMessage = " A035 can be maximum 255 characters")]
        public string A035 { get; set; }

        [Display(Name = "A036")]
        [MaxLength(255, ErrorMessage = " A036 can be maximum 255 characters")]
        public string A036 { get; set; }

        [Display(Name = "A037")]
        [MaxLength(255, ErrorMessage = " A037 can be maximum 255 characters")]
        public string A037 { get; set; }

        [Display(Name = "A038")]
        [MaxLength(255, ErrorMessage = " A038 can be maximum 255 characters")]
        public string A038 { get; set; }

        [Display(Name = "A039")]
        [MaxLength(255, ErrorMessage = " A039 can be maximum 255 characters")]
        public string A039 { get; set; }

        [Display(Name = "A040")]
        [MaxLength(255, ErrorMessage = " A040 can be maximum 255 characters")]
        public string A040 { get; set; }

        [Display(Name = "A041")]
        [MaxLength(255, ErrorMessage = " A041 can be maximum 255 characters")]
        public string A041 { get; set; }

        [Display(Name = "A042")]
        [MaxLength(255, ErrorMessage = " A042 can be maximum 255 characters")]
        public string A042 { get; set; }

        [Display(Name = "A043")]
        [MaxLength(255, ErrorMessage = " A043 can be maximum 255 characters")]
        public string A043 { get; set; }

        [Display(Name = "A044")]
        [MaxLength(255, ErrorMessage = " A044 can be maximum 255 characters")]
        public string A044 { get; set; }

        [Display(Name = "A045")]
        [MaxLength(255, ErrorMessage = " A045 can be maximum 255 characters")]
        public string A045 { get; set; }

        [Display(Name = "A046")]
        [MaxLength(255, ErrorMessage = " A046 can be maximum 255 characters")]
        public string A046 { get; set; }

        [Display(Name = "A047")]
        [MaxLength(255, ErrorMessage = " A047 can be maximum 255 characters")]
        public string A047 { get; set; }

        [Display(Name = "A048")]
        [MaxLength(255, ErrorMessage = " A048 can be maximum 255 characters")]
        public string A048 { get; set; }

        [Display(Name = "A049")]
        [MaxLength(255, ErrorMessage = " A049 can be maximum 255 characters")]
        public string A049 { get; set; }

        [Display(Name = "A050")]
        [MaxLength(255, ErrorMessage = " A050 can be maximum 255 characters")]
        public string A050 { get; set; }

        [Display(Name = "A051")]
        [MaxLength(255, ErrorMessage = " A051 can be maximum 255 characters")]
        public string A051 { get; set; }

        [Display(Name = "A052")]
        [MaxLength(255, ErrorMessage = " A052 can be maximum 255 characters")]
        public string A052 { get; set; }

        [Display(Name = "A053")]
        [MaxLength(255, ErrorMessage = " A053 can be maximum 255 characters")]
        public string A053 { get; set; }

        [Display(Name = "A054")]
        [MaxLength(255, ErrorMessage = " A054 can be maximum 255 characters")]
        public string A054 { get; set; }

        [Display(Name = "A055")]
        [MaxLength(255, ErrorMessage = " A055 can be maximum 255 characters")]
        public string A055 { get; set; }

        [Display(Name = "A056")]
        [MaxLength(255, ErrorMessage = " A056 can be maximum 255 characters")]
        public string A056 { get; set; }

        [Display(Name = "A057")]
        [MaxLength(255, ErrorMessage = " A057 can be maximum 255 characters")]
        public string A057 { get; set; }

        [Display(Name = "A058")]
        [MaxLength(255, ErrorMessage = " A058 can be maximum 255 characters")]
        public string A058 { get; set; }

        [Display(Name = "A059")]
        [MaxLength(255, ErrorMessage = " A059 can be maximum 255 characters")]
        public string A059 { get; set; }

        [Display(Name = "A060")]
        [MaxLength(255, ErrorMessage = " A060 can be maximum 255 characters")]
        public string A060 { get; set; }

        [Display(Name = "A061")]
        [MaxLength(255, ErrorMessage = " A061 can be maximum 255 characters")]
        public string A061 { get; set; }

        [Display(Name = "A062")]
        [MaxLength(255, ErrorMessage = " A062 can be maximum 255 characters")]
        public string A062 { get; set; }

        [Display(Name = "A063")]
        [MaxLength(255, ErrorMessage = " A063 can be maximum 255 characters")]
        public string A063 { get; set; }

        [Display(Name = "A064")]
        [MaxLength(255, ErrorMessage = " A064 can be maximum 255 characters")]
        public string A064 { get; set; }

        [Display(Name = "A065")]
        [MaxLength(255, ErrorMessage = " A065 can be maximum 255 characters")]
        public string A065 { get; set; }

        [Display(Name = "A066")]
        [MaxLength(255, ErrorMessage = " A066 can be maximum 255 characters")]
        public string A066 { get; set; }

        [Display(Name = "A067")]
        [MaxLength(255, ErrorMessage = " A067 can be maximum 255 characters")]
        public string A067 { get; set; }

        [Display(Name = "A068")]
        [MaxLength(255, ErrorMessage = " A068 can be maximum 255 characters")]
        public string A068 { get; set; }

        [Display(Name = "A069")]
        [MaxLength(255, ErrorMessage = " A069 can be maximum 255 characters")]
        public string A069 { get; set; }

        [Display(Name = "A070")]
        [MaxLength(255, ErrorMessage = " A070 can be maximum 255 characters")]
        public string A070 { get; set; }

        [Display(Name = "A071")]
        [MaxLength(255, ErrorMessage = " A071 can be maximum 255 characters")]
        public string A071 { get; set; }

        [Display(Name = "A072")]
        [MaxLength(255, ErrorMessage = " A072 can be maximum 255 characters")]
        public string A072 { get; set; }

        [Display(Name = "A073")]
        [MaxLength(255, ErrorMessage = " A073 can be maximum 255 characters")]
        public string A073 { get; set; }

        [Display(Name = "A074")]
        [MaxLength(255, ErrorMessage = " A074 can be maximum 255 characters")]
        public string A074 { get; set; }

        [Display(Name = "A075")]
        [MaxLength(255, ErrorMessage = " A075 can be maximum 255 characters")]
        public string A075 { get; set; }

        [Display(Name = "A076")]
        [MaxLength(255, ErrorMessage = " A076 can be maximum 255 characters")]
        public string A076 { get; set; }

        [Display(Name = "A077")]
        [MaxLength(255, ErrorMessage = " A077 can be maximum 255 characters")]
        public string A077 { get; set; }

        [Display(Name = "A078")]
        [MaxLength(255, ErrorMessage = " A078 can be maximum 255 characters")]
        public string A078 { get; set; }

        [Display(Name = "A079")]
        [MaxLength(255, ErrorMessage = " A079 can be maximum 255 characters")]
        public string A079 { get; set; }

        [Display(Name = "A080")]
        [MaxLength(255, ErrorMessage = " A080 can be maximum 255 characters")]
        public string A080 { get; set; }

        [Display(Name = "A081")]
        [MaxLength(255, ErrorMessage = " A081 can be maximum 255 characters")]
        public string A081 { get; set; }

        [Display(Name = "A082")]
        [MaxLength(255, ErrorMessage = " A082 can be maximum 255 characters")]
        public string A082 { get; set; }

        [Display(Name = "A083")]
        [MaxLength(255, ErrorMessage = " A083 can be maximum 255 characters")]
        public string A083 { get; set; }

        [Display(Name = "A084")]
        [MaxLength(255, ErrorMessage = " A084 can be maximum 255 characters")]
        public string A084 { get; set; }

        [Display(Name = "A085")]
        [MaxLength(255, ErrorMessage = " A085 can be maximum 255 characters")]
        public string A085 { get; set; }

        [Display(Name = "A086")]
        [MaxLength(255, ErrorMessage = " A086 can be maximum 255 characters")]
        public string A086 { get; set; }

        [Display(Name = "A087")]
        [MaxLength(255, ErrorMessage = " A087 can be maximum 255 characters")]
        public string A087 { get; set; }

        [Display(Name = "A088")]
        [MaxLength(255, ErrorMessage = " A088 can be maximum 255 characters")]
        public string A088 { get; set; }

        [Display(Name = "A089")]
        [MaxLength(255, ErrorMessage = " A089 can be maximum 255 characters")]
        public string A089 { get; set; }

        [Display(Name = "A090")]
        [MaxLength(255, ErrorMessage = " A090 can be maximum 255 characters")]
        public string A090 { get; set; }

        [Display(Name = "A091")]
        [MaxLength(255, ErrorMessage = " A091 can be maximum 255 characters")]
        public string A091 { get; set; }

        [Display(Name = "A092")]
        [MaxLength(255, ErrorMessage = " A092 can be maximum 255 characters")]
        public string A092 { get; set; }

        [Display(Name = "A093")]
        [MaxLength(255, ErrorMessage = " A093 can be maximum 255 characters")]
        public string A093 { get; set; }

        [Display(Name = "A094")]
        [MaxLength(255, ErrorMessage = " A094 can be maximum 255 characters")]
        public string A094 { get; set; }

        [Display(Name = "A095")]
        [MaxLength(255, ErrorMessage = " A095 can be maximum 255 characters")]
        public string A095 { get; set; }

        [Display(Name = "A096")]
        [MaxLength(255, ErrorMessage = " A096 can be maximum 255 characters")]
        public string A096 { get; set; }

        [Display(Name = "A097")]
        [MaxLength(255, ErrorMessage = " A097 can be maximum 255 characters")]
        public string A097 { get; set; }

        [Display(Name = "A098")]
        [MaxLength(255, ErrorMessage = " A098 can be maximum 255 characters")]
        public string A098 { get; set; }

        [Display(Name = "A099")]
        [MaxLength(255, ErrorMessage = " A099 can be maximum 255 characters")]
        public string A099 { get; set; }

        [Display(Name = "A100")]
        [MaxLength(255, ErrorMessage = " A100 can be maximum 255 characters")]
        public string A100 { get; set; }
    }
}