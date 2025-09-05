using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tblTheoDoiDoanhNghiep")]
public class TheoDoiDoanhNghiepModel
{
    [Key]
    public Guid TDDN_Id { set; get; }

    public Guid TDDN_NguoiTheoDoi_Id { set; get; }

    public Guid TDDN_DN_Id { set; get; }
}