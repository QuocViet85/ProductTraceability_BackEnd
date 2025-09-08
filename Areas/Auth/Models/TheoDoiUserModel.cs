
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tblTheoDoiUser")]
public class TheoDoiUserModel
{
    [Key]
    public Guid TDU_Id { set; get; }
    public Guid TDU_UserTheoDoi_Id { set; get; }
    public Guid TDU_UserDuocTheoDoi_Id { set; get; }
}