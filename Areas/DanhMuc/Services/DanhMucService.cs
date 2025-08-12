using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.DanhMuc.Models;
using App.Areas.DanhMuc.Repositories;

namespace App.Areas.DanhMuc.Services;

public class DanhMucService : IDanhMucService
{
    private readonly IDanhMucRepository _danhMucRepo;

    public DanhMucService(IDanhMucRepository danhMucRepo)
    {
        _danhMucRepo = danhMucRepo;
    }

    public async Task<(int totalItems, List<DanhMucModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _danhMucRepo.LayTongSoAsync();

        List<DanhMucModel> listDanhMucs = await _danhMucRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listDanhMucs);
    }

    public async Task<DanhMucModel> LayMotBangIdAsync(Guid id)
    {
        var danhMuc = await _danhMucRepo.LayMotBangIdAsync(id);

        if (danhMuc == null)
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }

        return danhMuc;
    }
    public async Task ThemAsync(DanhMucModel danhMuc, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (await _danhMucRepo.KiemTraTonTaiBangTenAsync(danhMuc.DM_Ten))
        {
            throw new Exception("Không thể tạo danh mục sản phẩm vì đã có danh mục sản phẩm cùng tên");
        }

        if (danhMuc.DM_LaDMCha == null)
        {
            danhMuc.DM_LaDMCha = false;
        }

        if ((bool) danhMuc.DM_LaDMCha)
        {
            danhMuc.DM_DMCha_Id = null;
        }
        else if (danhMuc.DM_DMCha_Id != null)
        {
            var danhMucCha = await _danhMucRepo.LayMotBangIdAsync((Guid)danhMuc.DM_DMCha_Id);

            if (danhMucCha == null)
            {
                throw new Exception("Danh mục cha không tồn tại");
            }

            if (!(bool)danhMucCha.DM_LaDMCha)
            {
                throw new Exception("Danh mục không phải danh mục cha");
            }

            danhMuc.DM_LaDMCha = false;
            danhMuc.DM_DMCha_Id = danhMuc.DM_DMCha_Id;
        }
        else
        {
            throw new Exception("Chưa chọn là danh mục cha hoặc có danh mục cha nên không thể tạo danh mục");
        }

        danhMuc.DM_NguoiTaoId = Guid.Parse(userIdNow);

        int result = await _danhMucRepo.ThemAsync(danhMuc);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo danh mục sản phẩm thất bại");
        }
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var danhMuc = await _danhMucRepo.LayMotBangIdAsync(id);

        if (danhMuc == null)
        {
            throw new Exception("Danh mục sản phẩm không tồn tại");
        }

        int result = await _danhMucRepo.XoaAsync(danhMuc);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Xóa danh mục sản phẩm thất bại");
        }
    }

    public async Task SuaAsync(Guid id, DanhMucModel danhMucUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var danhMuc = await _danhMucRepo.LayMotBangIdAsync(id);

        if (danhMuc == null)
        {
            throw new Exception("Danh mục sản phẩm không tồn tại");
        }

        if (await _danhMucRepo.KiemTraTonTaiBangTenAsync(danhMucUpdate.DM_Ten, id))
        {
            throw new Exception("Không cập nhật danh mục sản phẩm vì đã có danh mục sản phẩm khác cùng tên");
        }

        danhMuc.DM_Ten = danhMucUpdate.DM_Ten;
        danhMuc.DM_MoTa = danhMucUpdate.DM_MoTa;
        danhMuc.DM_NguoiSuaId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        danhMuc.DM_NgaySua = DateTime.Now;

        int result = await _danhMucRepo.SuaAsync(danhMuc);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật danh mục sản phẩm thất bại");
        }
    }

    public async Task<DanhMucModel> LayMotBangTenAsync(string name)
    {
        var danhMuc = await _danhMucRepo.LayMotBangTenAsync(name);

        if (danhMuc == null)
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }

        return danhMuc;
    }

    public Task<(int totalItems, List<DanhMucModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}