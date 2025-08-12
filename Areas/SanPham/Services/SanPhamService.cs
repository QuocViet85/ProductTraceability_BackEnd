using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.NhaMay.Repositories;
using App.Areas.Files.DTO;
using App.Areas.Files.Services;
using App.Areas.SanPham.Authorization;
using App.Areas.SanPham.Models;
using App.Areas.SanPham.Repositories;
using App.Database;
using App.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Areas.Files;
using App.Areas.DanhMuc.Models;
using App.Areas.DanhMuc.Repositories;

namespace App.Areas.SanPham.Services;

public class SanPhamService : ISanPhamService
{
    private readonly ISanPhamRepository _sanPhamRepo;
    private readonly IAuthorizationService _authorizationService;
    private readonly INhaMayRepository _nhaMayRepo;
    private readonly IDanhMucRepository _danhMucRepo;
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;
    private readonly IFileService _fileService;
    private readonly UserManager<AppUser> _userManager;

    public SanPhamService(ISanPhamRepository sanPhamRepo, IAuthorizationService authorizationService, IDoanhNghiepRepository doanhNghiepRepo, INhaMayRepository nhaMayRepo, UserManager<AppUser> userManager, IFileService fileService, IDanhMucRepository danhMucRepo)
    {
        _sanPhamRepo = sanPhamRepo;
        _authorizationService = authorizationService;
        _doanhNghiepRepo = doanhNghiepRepo;
        _nhaMayRepo = nhaMayRepo;
        _userManager = userManager;
        _fileService = fileService;
        _danhMucRepo = danhMucRepo;
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int tongSo = await _sanPhamRepo.LayTongSoCuaNguoiDungAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuCuaNguoiDungAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task<SanPhamModel> LayMotBangIdAsync(Guid id)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);
        if (sanPham == null)
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
        return sanPham;
    }

    public async Task<SanPhamModel> LayMotBangMaTruyXuatAsync(string maTruyXuat)
    {
        var sanPham = await _sanPhamRepo.LayMotBangMaTruyXuatAsync(maTruyXuat);
        if (sanPham == null)
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }

        return sanPham;
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSoHuuAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoBangDoanhNghiepSoHuuAsync(dn_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangDoanhNghiepSoHuuAsync(dn_id, pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepVanTaiAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoBangDoanhNghiepVanTaiAsync(dn_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangDoanhNghiepVanTaiAsync(dn_id, pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSanXuatAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoBangDoanhNghiepSanXuatAsync(dn_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangDoanhNghiepSanXuatAsync(dn_id, pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }


    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDanhMucAsync(Guid dm_id, int pageNumber, int limit, string search, bool descending)
    {
        DanhMucModel danhMuc = await _danhMucRepo.LayMotBangIdAsync(dm_id);

        if (danhMuc == null)
        {
            throw new Exception("Không tồn tại danh mục");
        }

        int tongSo = await _sanPhamRepo.LayTongSoBangDanhMucAsync(dm_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangDanhMucAsync(dm_id, pageNumber, limit, search, descending);

        if ((bool) danhMuc.DM_LaDMCha && danhMuc.DM_List_DMCon != null)
        {
            if (listSanPhams.Count < limit)
            {
                int quantitysanPhamGetByChildCategories = limit - listSanPhams.Count;
                var listSanPhamsGetByChildCategories = new List<SanPhamModel>();

                foreach (var danhMucCon in danhMuc.DM_List_DMCon)
                {
                    if (quantitysanPhamGetByChildCategories > 0)
                    {
                        var listSanPhamsByChild = await _sanPhamRepo.LayNhieuBangDanhMucAsync(danhMucCon.DM_Id, 1, quantitysanPhamGetByChildCategories, search, descending);
                        listSanPhamsGetByChildCategories.AddRange(listSanPhamsByChild);

                        quantitysanPhamGetByChildCategories -= listSanPhamsByChild.Count;
                    }
                    tongSo += await _sanPhamRepo.LayTongSoBangDanhMucAsync(danhMucCon.DM_Id);
                }

                listSanPhams.AddRange(listSanPhamsGetByChildCategories);
            }
        }

        return (tongSo, listSanPhams);
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangNguoiPhuTrachAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoBangNguoiPhuTrachAsync(userId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangNguoiPhuTrachAsync(userId, pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangNhaMayAsync(Guid nm_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _sanPhamRepo.LayTongSoBangNhaMayAsync(nm_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SanPhamModel> listSanPhams = await _sanPhamRepo.LayNhieuBangNhaMayAsync(nm_id, pageNumber, limit, search, descending);

        return (tongSo, listSanPhams);
    }

    public async Task ThemAsync(SanPhamModel sanPhamNew, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (sanPhamNew.SP_DN_SoHuu_Id == null)
        {
            throw new Exception("Không thể tạo sản phẩm không có sở hữu doanh nghiệp");
        }

        bool laChuDoanhNghiep = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync((Guid) sanPhamNew.SP_DN_SoHuu_Id, Guid.Parse(userIdNow));

        if (!laChuDoanhNghiep)
        {
            throw new Exception("Không phải là chủ doanh nghiệp nên không thể tạo sản phẩm");
        }


        if (sanPhamNew.SP_MaTruyXuat != null)
        {
            bool daTonTaiMaTruyXuat = await _sanPhamRepo.KiemTraTonTaiBangMaTruyXuatAsync(PrefixCode.SANPHAM + sanPhamNew.SP_MaTruyXuat);

            if (daTonTaiMaTruyXuat)
            {
                throw new Exception("Mã sản phẩm đã tồn tại nên không tạo sản phẩm");
            }
        }
        else
        {
            sanPhamNew.SP_MaTruyXuat = CreateCode.GenerateCodeFromTicks(PrefixCode.SANPHAM);
        }

        if (await _sanPhamRepo.KiemTraTonTaiBangMaVachAsync(sanPhamNew.SP_MaVach))
        {
            throw new Exception("Mã vạch đã tồn tại nên không thể tạo sản phẩm");
        }

        sanPhamNew.SP_NguoiTao_Id = Guid.Parse(userIdNow);

        int result = await _sanPhamRepo.ThemAsync(sanPhamNew);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo sản phẩm thất bại");
        }

    }

    public async Task UpdateAsync(Guid id, SanPhamModel SanPhamModel, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("sản phẩm không tồn tại");
        }

        string traceCode = sanPham.TraceCode;
        if (SanPhamModel.TraceCode != null)
        {
            bool existtraceCode = await _sanPhamRepo.CheckExistExceptThisByTraceCode(id, PrefixCode.sanPham + SanPhamModel.TraceCode);

            if (existtraceCode)
            {
                throw new Exception("Mã sản phẩm đã tồn tại nên không cập nhật sản phẩm");
            }

            traceCode = PrefixCode.sanPham + SanPhamModel.TraceCode;
        }

        if (await _sanPhamRepo.CheckExistExceptThisByBarCode(id, SanPhamModel.BarCode))
        {
            throw new Exception("Mã vạch đã tồn tại nên không thể cập nhật sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdatesanPhamRequirement(traceCode));

        if (checkAuth.Succeeded)
        {
            sanPham = sanPhamMapper.DtoToModel(SanPhamModel, sanPham);
            sanPham.TraceCode = traceCode;
            sanPham.UpdatedAt = DateTime.Now;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật sản phẩm này");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanDeletesanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _sanPhamRepo.DeleteAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sản phẩm thất bại");
            }

            await _fileService.DeleteManyByEntityAsync(FileInformation.EntityType.sanPham, id.ToString());
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sản phẩm này");
        }
    }

    public async Task AddOwnerIndividualEnterpriseOfsanPhamAsync(Guid id, Guid individualEnterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        bool existIndividualEnterprise = await _individualdoanhNghiepRepo.CheckExistByOwnerUserIdAsync(individualEnterpriseId);

        if (!existIndividualEnterprise)
        {
            throw new Exception("Không tồn tại hộ kinh doanh cá nhân");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanAddOwnerIndividualEnterpriseOfsanPhamRequirement(individualEnterpriseId.ToString()));

        if (checkAuth.Succeeded)
        {
            sanPham.OwnerIndividualEnterpriseId = individualEnterpriseId;
            sanPham.OwnerEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm hộ kinh doanh của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm hộ kinh doanh cá nhân vào sản phẩm này");
        }
    }

    public async Task DeleteOwnerIndividualEnterpriseOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanDeleteOwnerIndividualEnterpriseOfsanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.OwnerIndividualEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sở hữu cá nhân sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa cá nhân sở hữu sản phẩm này");
        }
    }

    public async Task AddOwnerEnterpriseOfsanPhamAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _doanhNghiepRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        if (sanPham.OwnerEnterpriseId == enterpriseId)
        {
            throw new Exception("sản phẩm đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanAddOwnerEnterpriseOfsanPhamRequirement(enterpriseId));

        if (checkAuth.Succeeded)
        {
            sanPham.OwnerEnterpriseId = enterpriseId;
            sanPham.OwnerIndividualEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm doanh nghiệp sở hữu sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm doanh nghiệp vào sản phẩm này");
        }
    }

    public async Task DeleteOwnerEnterpriseOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanDeleteEnterpriseInFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.OwnerEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sở hữu sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp của sản phẩm này");
        }
    }

    public async Task AddCarrierEnterpriseOfsanPhamAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _doanhNghiepRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.CarrierEnterpriseId = enterpriseId;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp vận chuyển sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật doanh nghiệp vận chuyển của sản phẩm này");
        }
    }

    public async Task DeleteCarrierEnterpriseOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.CarrierEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp vận chuyển sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp vận chuyển của sản phẩm này");
        }
    }

    public async Task AddProducerEnterpriseOfsanPhamAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _doanhNghiepRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.ProducerEnterpriseId = enterpriseId;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp sản xuất sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật doanh nghiệp sản xuất của sản phẩm này");
        }
    }

    public async Task DeleteProducerEnterpriseOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.ProducerEnterpriseId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sản xuất sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp sản xuất của sản phẩm này");
        }
    }

    public async Task AddResponsibleUserOfsanPhamAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        var responsibleUser = await _userManager.FindByIdAsync(userId.ToString());

        if (responsibleUser == null)
        {
            throw new Exception("Không tồn tại người dùng");
        }

        var roleResponsibleUser = (await _userManager.GetRolesAsync(responsibleUser))[0];

        if (roleResponsibleUser != Roles.ADMIN && roleResponsibleUser != Roles.ENTERPRISE)
        {
            throw new Exception("Người dùng này không có vai trò phù hợp làm người phụ trách sản phẩm");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.ResponsibleUserId = userId;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật người phụ trách sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật người phụ trách sản phẩm này");
        }
    }

    public async Task DeleteResponsibleUserOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.ResponsibleUserId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa người phụ trách sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa người phụ trách sản phẩm này");
        }
    }

    public async Task AddFactoryOfsanPhamAsync(Guid id, Guid factoryId, ClaimsPrincipal userNowFromJwt)
    {
        var existFactory = await _nhaMayRepo.CheckExistByIdAsync(factoryId);

        if (!existFactory)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.FactoryId = factoryId;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật nhà máy của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật nhà máy của sản phẩm này");
        }
    }

    public async Task DeleteFactoryOfsanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdateSomeRelationssanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.FactoryId = null;
            sanPham.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.UpdateAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa nhà máy của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa nhà máy của sản phẩm này");
        }
    }

    public async Task UploadPhotosOfsanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdatesanPhamRequirement(sanPham.TraceCode));

        if (checkAuth.Succeeded)
        {
            int result = await _fileService.UploadAsync(listFiles, new FileDTO(FileInformation.FileType.IMAGE, FileInformation.EntityType.sanPham, id.ToString()));

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Đăng ảnh thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền đăng ảnh cho sản phẩm này");
        }
    }

    public async Task DeletePhotoOfsanPhamAsync(Guid id, Guid fileId, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.GetOneByIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var file = await _fileService.GetOneByIdAsync(fileId);

        if (file == null)
        {
            throw new Exception("Không tồn tại ảnh");
        }

        if (file.EntityType == FileInformation.EntityType.sanPham && file.EntityId == id.ToString())
        {
            var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new CanUpdatesanPhamRequirement(sanPham.TraceCode));

            if (checkAuth.Succeeded)
            {
                int result = await _fileService.DeleteOneByIdAsync(fileId);

                if (result == 0)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu. Xóa ảnh thất bại");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Không có quyền đăng ảnh cho sản phẩm này");
            }
        }
        else
        {
            throw new Exception("Ảnh này không phải của sản phẩm này nên không thể xóa");
        }
    }

    private void AddRelationToDTO(SanPhamModel SanPhamModel, SanPhamModel sanPham)
    {
        if (sanPham.OwnerIndividualEnterprise != null)
        {
            SanPhamModel.OwnerIndividualEnterprise = IndividualEnterpriseMapper.ModelToDto(sanPham.OwnerIndividualEnterprise);
        }

        if (sanPham.OwnerEnterprise != null)
        {
            SanPhamModel.OwnerEnterprise = EnterpriseMapper.ModelToDto(sanPham.OwnerEnterprise);
        }

        if (sanPham.Category != null)
        {
            SanPhamModel.Category = CategoryMapper.ModelToDto(sanPham.Category);
        }

        if (sanPham.ProducerEnterprise != null)
        {
            SanPhamModel.ProducerEnterprise = EnterpriseMapper.ModelToDto(sanPham.ProducerEnterprise);
        }

        if (sanPham.CarrierEnterprise != null)
        {
            SanPhamModel.CarrierEnterprise = EnterpriseMapper.ModelToDto(sanPham.CarrierEnterprise);
        }

        if (sanPham.ResponsibleUser != null)
        {
            SanPhamModel.ResponsibleUser = UserMapper.ModelToDto(sanPham.ResponsibleUser);
        }

        if (sanPham.Factory != null)
        {
            SanPhamModel.Factory = FactoryMapper.ModelToDto(sanPham.Factory);
        }

        if (sanPham.CreatedUser != null)
        {
            SanPhamModel.CreatedUser = UserMapper.ModelToDto(sanPham.CreatedUser);
        }

        if (sanPham.UpdatedUser != null)
        {
            SanPhamModel.UpdatedUser = UserMapper.ModelToDto(sanPham.UpdatedUser);
        }
    }
}