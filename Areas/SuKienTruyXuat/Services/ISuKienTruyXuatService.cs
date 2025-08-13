using App.Areas.SuKienTruyXuat.Models;
using App.Services;

namespace App.Areas.SuKienTruyXuat.Services;

public interface ISuKienTruyXuatService : IBaseService<SuKienTruyXuatModel>
{
    public Task<(int totalItems, List<SuKienTruyXuatModel> listItems)> LayNhieuBangLoSanPhamAsync(Guid lsp_Id, int pageNumber, int limit, string search, bool descending);
    public Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK);
}