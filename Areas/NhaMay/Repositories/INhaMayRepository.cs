using App.Areas.NhaMay.Models;
using App.Database;

namespace App.Areas.NhaMay.Repositories;

public interface INhaMayRepository : IBaseRepository<NhaMayModel>
{
    public Task<bool> KiemTraTonTaiBangMaNhaMayAsync(string maNhaMay, Guid? id = null);
    public Task<NhaMayModel> LayMotBangMaNhaMayAsync(string maNhaMay);
}