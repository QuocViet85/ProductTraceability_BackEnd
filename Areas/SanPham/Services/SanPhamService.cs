using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.Auth.Mapper;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.NhaMay.Repositories;
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
using App.Areas.Files.ThongTin;
using App.Areas.DTO;

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

        List<DanhMucModel> tatCaDanhMuc = await _danhMucRepo.LayTatCaAsync();

        await LaySanPhamTrongDanhMucCon(listSanPhams, danhMuc, tatCaDanhMuc, limit, search, descending);

        foreach (var sanPham in listSanPhams)
        {
            //Lấy ra tất cả các danh mục nên danh mục cha tham chiếu đến danh mục con, danh mục con tham chiếu đến danh mục cha và gán danh mục vào sản phẩm trả về nên tạo ra vòng lặp json vô hạn khi trả về. Đặt NULL để tránh điều này
            sanPham.SP_DM = null;
        }

        return (tongSo, listSanPhams);
    }

    private async Task LaySanPhamTrongDanhMucCon(List<SanPhamModel> listSanPhams, DanhMucModel danhMucCha, List<DanhMucModel> tatCaDanhMuc, int limit, string search, bool descending)
    {
        int soLuongSanPhamCanLay = limit - listSanPhams.Count;

        List<DanhMucModel> listDanhMucCons = tatCaDanhMuc.Where(dm => dm.DM_DMCha_Id == danhMucCha.DM_Id).ToList();

        foreach (var danhMucCon in listDanhMucCons)
        {
            if (soLuongSanPhamCanLay <= 0)
            {
                break;
            }

            List<SanPhamModel> listSanPhamTrongDMCon = await _sanPhamRepo.LayNhieuBangDanhMucAsync(danhMucCon.DM_Id, 1, soLuongSanPhamCanLay, search, descending);
            listSanPhams.AddRange(listSanPhamTrongDMCon);
            soLuongSanPhamCanLay -= listSanPhamTrongDMCon.Count;

            await LaySanPhamTrongDanhMucCon(listSanPhams, danhMucCon, tatCaDanhMuc, limit, search, descending);
        }
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

        var doanhNghiepSoHuuSanPham = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)sanPhamNew.SP_DN_SoHuu_Id);

        if (doanhNghiepSoHuuSanPham == null) throw new Exception("Không tồn tại doanh nghiệp để sở hữu sản phẩm");

        var quyenThem = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiepSoHuuSanPham, new ThemSanPhamRequirement());

        if (quyenThem.Succeeded)
        {
            if (sanPhamNew.SP_MaTruyXuat != null)
            {
                sanPhamNew.SP_MaTruyXuat = PrefixCode.SANPHAM + sanPhamNew.SP_MaTruyXuat;
                bool daTonTaiMaTruyXuat = await _sanPhamRepo.KiemTraTonTaiBangMaTruyXuatAsync(sanPhamNew.SP_MaTruyXuat);

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
        else
        {
            throw new Exception("Không có quyền thêm sản phẩm");
        }
    }

    public async Task SuaAsync(Guid id, SanPhamModel sanPhamUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("sản phẩm không tồn tại");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            if (sanPhamUpdate.SP_MaTruyXuat != null)
            {
                sanPhamUpdate.SP_MaTruyXuat = PrefixCode.SANPHAM + sanPhamUpdate.SP_MaTruyXuat;
                bool daTonTaiMaTruyXuat = await _sanPhamRepo.KiemTraTonTaiBangMaTruyXuatAsync(sanPhamUpdate.SP_MaTruyXuat, id);

                if (daTonTaiMaTruyXuat)
                {
                    throw new Exception("Mã sản phẩm đã tồn tại nên không cập nhật sản phẩm");
                }

                sanPham.SP_MaTruyXuat = sanPhamUpdate.SP_MaTruyXuat;
            }

            if (await _sanPhamRepo.KiemTraTonTaiBangMaVachAsync(sanPhamUpdate.SP_MaVach, id))
            {
                throw new Exception("Mã vạch đã tồn tại nên không thể cập nhật sản phẩm");
            }

            sanPham.SP_Ten = sanPhamUpdate.SP_Ten;
            sanPham.SP_MaVach = sanPhamUpdate.SP_MaVach;
            sanPham.SP_MoTa = sanPhamUpdate.SP_MoTa;
            sanPham.SP_Website = sanPhamUpdate.SP_Website;
            sanPham.SP_Gia = sanPhamUpdate.SP_Gia;
            sanPham.SP_MaQuocGia = sanPhamUpdate.SP_MaQuocGia;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new XoaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _sanPhamRepo.XoaAsync(sanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sản phẩm thất bại");
            }

            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.SAN_PHAM, id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sản phẩm này");
        }
    }

    public async Task DoiDoanhNghiepSoHuuSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt)
    {
        var daTonTaiDoanhNghiep = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync(dn_id);

        if (!daTonTaiDoanhNghiep)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        if (sanPham.SP_DN_SoHuu_Id == dn_id)
        {
            throw new Exception("Sản phẩm đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new ToanQuyenSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_DN_SoHuu_Id = dn_id;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task ThemDoanhNghiepVanTaiSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt)
    {
        var daTonTaiDoanhNghiep = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync(dn_id);

        if (!daTonTaiDoanhNghiep)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_DN_VanTai_Id = dn_id;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task XoaDoanhNghiepVanTaiSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_DN_VanTai_Id = null;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task ThemDoanhNghiepSanXuatSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt)
    {
        var daTonTaiDoanhNghiep = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync(dn_id);

        if (!daTonTaiDoanhNghiep)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_DN_SanXuat_Id = dn_id;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task XoaDoanhNghiepSanXuatSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_DN_SanXuat_Id = null;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task ThemNguoiPhuTrachSanPhamAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        var nguoiPhuTrach = await _userManager.FindByIdAsync(userId.ToString());

        if (nguoiPhuTrach == null)
        {
            throw new Exception("Không tồn tại người dùng");
        }

        var rolenguoiPhuTrach = (await _userManager.GetRolesAsync(nguoiPhuTrach))[0];

        if (rolenguoiPhuTrach != Roles.ADMIN && rolenguoiPhuTrach != Roles.DOANH_NGHIEP)
        {
            throw new Exception("Người dùng này không có vai trò phù hợp làm người phụ trách sản phẩm");
        }

        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new ToanQuyenSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_NguoiPhuTrach_Id = userId;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task XoaNguoiPhuTrachSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new ToanQuyenSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_NguoiPhuTrach_Id = null;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task ThemNhaMayCuaSanPhamAsync(Guid id, Guid nm_id, ClaimsPrincipal userNowFromJwt)
    {
        var daTonTaiNM = await _nhaMayRepo.KiemTraTonTaiBangIdAsync(nm_id);

        if (!daTonTaiNM)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_NM_Id = nm_id;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task XoaNhaMayCuaSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            sanPham.SP_NM_Id = null;
            sanPham.SP_NgaySua = DateTime.Now;
            sanPham.SP_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _sanPhamRepo.SuaAsync(sanPham);

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

    public async Task TaiLenAnhSanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _fileService.TaiLenAsync(listFiles, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.SAN_PHAM, id, userNowFromJwt);

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

    public async Task XoaAnhSanPhamAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt)
    {
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var file = await _fileService.LayMotBangIdAsync(f_id);

        if (file == null)
        {
            throw new Exception("Không tồn tại ảnh");
        }

        if (file.F_KieuTaiNguyen == KieuTaiNguyen.SAN_PHAM && file.F_TaiNguyenId == id)
        {
            var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement());

            if (checkAuth.Succeeded)
            {
                int result = await _fileService.XoaMotBangIdAsync(f_id);

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
}