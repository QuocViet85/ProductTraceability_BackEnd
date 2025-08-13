using App.Areas.NhaMay.Models;
using App.Database;

namespace App.Areas.NhaMay.Repositories;

public interface INhaMayRepository : IBaseRepository<NhaMayModel>
{
    public Task<bool> KiemTraTonTaiBangMaNhaMayAsync(string nm_MaNM, Guid? id = null);
    public Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM);
}