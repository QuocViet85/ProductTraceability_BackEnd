using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.NhaMay.Models;
using App.Areas.NhaMay.Repositories;
using App.Areas.DoanhNghiep.Repositories;
using Microsoft.AspNetCore.Authorization;
using App.Areas.NhaMay.Authorization;
using App.Helper;

namespace App.Areas.NhaMay.Services;

public class NhaMayService : INhaMayService
{
    private readonly INhaMayRepository _nhaMayRepo;
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;
    private readonly IAuthorizationService _authorizationService;

    public NhaMayService(INhaMayRepository nhaMayRepo, IDoanhNghiepRepository doanhNghiepRepo, IAuthorizationService authorizationService)
    {
        _nhaMayRepo = nhaMayRepo;
        _doanhNghiepRepo = doanhNghiepRepo;
        _authorizationService = authorizationService;
    }

    public async Task<(int totalItems, List<NhaMayModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _nhaMayRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<NhaMayModel> listNhaMays = await _nhaMayRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listNhaMays);
    }

    public async Task<(int totalItems, List<NhaMayModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int tongSo = await _nhaMayRepo.LayTongSoCuaNguoiDungAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<NhaMayModel> listNhaMays = await _nhaMayRepo.LayNhieuCuaNguoiDungAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        return (tongSo, listNhaMays);
    }

    public async Task<NhaMayModel> LayMotBangIdAsync(Guid id)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);
        if (nhaMay == null)
        {
            throw new Exception("Không tìm thấy nhà máy");
        }

        return nhaMay;
    }

    public async Task ThemAsync(NhaMayModel nhaMayModel, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (nhaMayModel.NM_DN_Id == null)
        {
            throw new Exception("Không thể tạo nhà máy không thuộc sở hữu doanh nghiệp nào");
        }

        bool laChuDoanhNghiep = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync((Guid) nhaMayModel.NM_DN_Id, Guid.Parse(userIdNow));

        if (!laChuDoanhNghiep)
        {
            throw new Exception("Không sở hữu doanh nghiệp nên không thể tạo nhà máy");
        }

        if (nhaMayModel.NM_MaNM != null)
        {
            bool daCoMaNhaMay = await _nhaMayRepo.KiemTraTonTaiBangMaNhaMayAsync(nhaMayModel.NM_MaNM);

            if (daCoMaNhaMay)
            {
                throw new Exception("Mã nhà máy đã tồn tại nên không tạo nhà máy");
            }
        }
        else
        {
            nhaMayModel.NM_MaNM = CreateCode.GenerateCodeFromTicks();
        }

        nhaMayModel.NM_NguoiTao_Id = Guid.Parse(userIdNow);

        int result = await _nhaMayRepo.ThemAsync(nhaMayModel);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo nhà máy thất bại");
        }
    }

    public async Task SuaAsync(Guid id, NhaMayModel nhaMayUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Nhà máy không tồn tại");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new SuaNhaMayRequirement());

        if (quyenSua.Succeeded)
        {
            if (nhaMayUpdate.NM_MaNM != null)
            {
                bool daCoMaNhaMay = await _nhaMayRepo.KiemTraTonTaiBangMaNhaMayAsync(nhaMayUpdate.NM_MaNM);

                if (daCoMaNhaMay)
                {
                    throw new Exception("Mã nhà máy đã tồn tại nên không cập nhật nhà máy");
                }

                nhaMay.NM_MaNM = nhaMayUpdate.NM_MaNM;
            }
            nhaMay.NM_Ten = nhaMayUpdate.NM_Ten;
            nhaMay.NM_LienHe = nhaMayUpdate.NM_LienHe;
            nhaMay.NM_DiaChi = nhaMayUpdate.NM_DiaChi;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật nhà máy này");
        }
    }

    public async Task ThemDoanhNghiepVaoNhaMayAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt)
    {
        var tonTaiDoanhNghiep = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync(dn_id);

        if (!tonTaiDoanhNghiep)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        if (nhaMay.NM_DN_Id == dn_id)
        {
            throw new Exception("Nhà máy đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new ThemDoanhNghiepVaoNhaMayRequirement(dn_id));

        if (checkAuth.Succeeded)
        {
            nhaMay.NM_DN_Id = dn_id;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm doanh nghiệp vào nhà máy này");
        }
    }

    public async Task XoaDoanhNghiepKhoiNhaMayAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new XoaDoanhNghiepKhoiNhaMayRequirement());

        if (checkAuth.Succeeded)
        {
            nhaMay.NM_DN_Id = null;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp của nhà máy này");
        }
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new XoaNhaMayRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _nhaMayRepo.XoaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa nhà máy này");
        }
    }

    public async Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangMaNhaMayAsync(nm_MaNM);
        if (nhaMay == null)
        {
            throw new Exception("Không tìm thấy nhà máy");
        }

        return nhaMay;
    }
}

/*
Nhà máy chỉ có thể có IndividualEnterpriseId hoặc EnterpriseId hoặc không có cả 2 (chỉ có thể có 1 trong 2 loại sở hữu là sở hữu hộ kinh doanh cá nhân và sở hữu doanh nghiệp hoặc không có sở hữu nào, không thể có cả 2)

Logic phân quyền thêm/đổi doanh nghiệp cho nhà máy với role Enterprise: 
- Là chủ cá nhân hộ kinh doanh cá nhân sở hữu nhà máy thì được thêm doanh nghiệp của mình vào nhà máy vì tình huống này người dùng đang là chủ duy nhất của nhà máy (không được thêm doanh nghiệp không phải của mình vào nhà máy vì làm vậy là thao tác với tài nguyên không phải của mình).
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được đổi doanh nghiệp khác của mình vào nhà máy vì tình huống này người đùng là chủ duy nhất của nhà máy.

Logic phân quyển đổi nhaMayCode giống logic phân quyền thêm/đổi doanh nghiệp cho nhà máy

Logic phân quyền xóa doanh nghiệp sở hữu nhà máy với role Enterprise:
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được xóa doanh nghiệp sở hữu nhà máy vì tình huống này người đùng là chủ duy nhất của nhà máy.


Logic phân quyền thêm hộ kinh doanh cá nhân cho nhà máy role Enterprise: 
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được quyền thêm hộ kinh doanh cá nhân của bản thân vào nhà máy vì tình huống này người dùng đã là chủ duy nhất của nhà máy.
- Là chủ cá nhân của hộ kinh doanh sở hữu nhà máy thì không được đổi hộ kinh doanh cá nhân cho nhà máy vì đổi như vậy là thao tác trực tiếp với dữ liệu của User khác => Không có quyền.

Logic phân quyền xóa hộ kinh doanh cá nhân của nhà máy role Enterprise:
- Là chủ hộ kinh doanh cá nhân của nhà máy thì có quyền xóa hộ kinh doanh cá nhân của nhà máy vì tình huống này người dùng là chủ duy nhất của nhà máy.

Logic phân quyền xóa nhà máy role Enterprise:
- Là chủ hộ kinh doanh cá nhân của nhà máy thì có quyền xóa nhà máy vì tình huống này người dùng là chủ duy nhất của nhà máy.
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì có quyền xóa nhà máy vì tình huống này người dùng đã là chủ duy nhất của nhà máy.
*/