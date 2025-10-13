using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Auth;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.DoanhNghiep.Models;
using App.Areas.DTO;
using App.Areas.SanPham.Authorization;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;

namespace App.Areas.DoanhNghiep.Services;

public class DoanhNghiepService : IDoanhNghiepService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;
    private readonly IFileService _fileService;

    public DoanhNghiepService(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IDoanhNghiepRepository doanhNghiepRepo, IFileService fileService)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _doanhNghiepRepo = doanhNghiepRepo;
        _fileService = fileService;
    }

    public async Task<(int totalItems, List<DoanhNghiepModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _doanhNghiepRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<DoanhNghiepModel> listDoanhNghieps = await _doanhNghiepRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listDoanhNghieps);
    }
    public async Task<(int totalItems, List<DoanhNghiepModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int tongSo = await _doanhNghiepRepo.LayTongSoCuaNguoiDungAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<DoanhNghiepModel> listDoanhNghieps = await _doanhNghiepRepo.LayNhieuCuaNguoiDungAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        return (tongSo, listDoanhNghieps);
    }

    public async Task<(int totalItems, List<DoanhNghiepCoBanModel> listItems)> LayNhieuCoBanAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _doanhNghiepRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<DoanhNghiepCoBanModel> listDoanhNghieps = await _doanhNghiepRepo.LayNhieuCoBanAsync(pageNumber, limit, search, descending);

        return (tongSo, listDoanhNghieps);
    }

    public async Task<DoanhNghiepModel> LayMotBangIdAsync(Guid id)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);
        if (doanhNghiep == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return doanhNghiep;
    }

    public async Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string dn_MaSoThue)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangMaSoThueAsync(dn_MaSoThue);
        if (doanhNghiep == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return doanhNghiep;
    }

    public async Task<DoanhNghiepModel> LayMotBangMaGS1Async(string dn_MaGS1)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangMaGS1Async(dn_MaGS1);

        return doanhNghiep;
    }

    public async Task<DoanhNghiepModel> ThemAsync(DoanhNghiepModel doanhNghiepNew, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null)
        {
            throw new Exception("Tài khoản không hợp lệ");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaSoThueAsync(doanhNghiepNew.DN_MaSoThue))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã số thuế đã tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaGLNAsync(doanhNghiepNew.DN_MaGLN))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã GLN đã tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaGS1Async(doanhNghiepNew.DN_MaGS1))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã GS1 đã tồn tại");
        }

        doanhNghiepNew.DN_NguoiTao_Id = Guid.Parse(userIdNow);

        if (doanhNghiepNew.DN_KieuDN == null)
        {
            throw new Exception("Phải chọn kiểu doanh nghiệp");
        }

        int result = await _doanhNghiepRepo.ThemAsync(doanhNghiepNew);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo doanh nghiệp thất bại");
        }

        var chuDoanhNghiep = new ChuDoanhNghiepModel()
        {
            CDN_DN_Id = doanhNghiepNew.DN_Id,
            CDN_ChuDN_Id = Guid.Parse(userIdNow),
            CDN_NguoiTao_Id = Guid.Parse(userIdNow)
        };

        await _doanhNghiepRepo.ThemSoHuuDoanhNghiepAsync(chuDoanhNghiep);

        //Mặc định Phân tất cả các quyền cho người đầu tiên tạo doanh nghiệp có role khác Admin.
        if (!userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            var adminDoanhNghiepClaim = new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Admin, doanhNghiepNew.DN_Id));
            var adminSanPhamCuaDoanhNghiepClaim = new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Admin, doanhNghiepNew.DN_Id));

            await _userManager.AddClaimsAsync(user, new List<Claim>() { adminDoanhNghiepClaim, adminSanPhamCuaDoanhNghiepClaim });
        }

        doanhNghiepNew.DN_List_CDN = null;

        return doanhNghiepNew;
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var quyenXoa = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new XoaDoanhNghiepRequirement());

        if (quyenXoa.Succeeded)
        {
            int result = await _doanhNghiepRepo.XoaAsync(doanhNghiep);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp thất bại");
            }
            await _doanhNghiepRepo.XoaPhanQuyenDoanhNghiepAsync(id);
            await _doanhNghiepRepo.XoaPhanQuyenSanPhamTheoDoanhNghiepAsync(id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp");
        }
    }

    public async Task SuaAsync(Guid id, DoanhNghiepModel doanhNghiepUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaSoThueAsync(doanhNghiepUpdate.DN_MaSoThue, id))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaGLNAsync(doanhNghiepUpdate.DN_MaGLN, id))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã GLN đã tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaGS1Async(doanhNghiepUpdate.DN_MaGS1, id))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã GS1 đã tồn tại");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaDoanhNghiepRequirement());

        if (quyenSua.Succeeded)
        {
            doanhNghiep.DN_Ten = doanhNghiepUpdate.DN_Ten;
            doanhNghiep.DN_MaSoThue = doanhNghiepUpdate.DN_MaSoThue;
            doanhNghiep.DN_MaGLN = doanhNghiepUpdate.DN_MaGLN;
            doanhNghiep.DN_DiaChi = doanhNghiepUpdate.DN_DiaChi;
            doanhNghiep.DN_SoDienThoai = doanhNghiepUpdate.DN_SoDienThoai;
            doanhNghiep.DN_Email = doanhNghiepUpdate.DN_Email;
            doanhNghiep.DN_MaGS1 = doanhNghiepUpdate.DN_MaGS1;
            doanhNghiep.DN_JsonData = doanhNghiepUpdate.DN_JsonData;
            
            doanhNghiep.DN_NgaySua = DateTime.Now;
            doanhNghiep.DN_NguoiSua_Id = Guid.Parse(userIdNow);

            int result = await _doanhNghiepRepo.SuaAsync(doanhNghiep);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa doanh nghiệp");
        }
    }

    public async Task ThemSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        bool daLaCDN = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync(id, userId);
        if (daLaCDN)
        {
            throw new Exception("User này đang sở hữu doanh nghiệp này");
        }

        var userAdd = await _userManager.FindByIdAsync(userId.ToString());
        if (userAdd == null)
        {
            throw new Exception("Không tìm thấy User để thêm sở hữu");
        }

        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);
        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (doanhNghiep.DN_KieuDN == 1 && doanhNghiep.DN_List_CDN.Count == 1)
        {
            throw new Exception("Không thể thêm chủ doanh nghiệp cho doanh nghiệp cá nhân đã có chủ");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new ToanQuyenDoanhNghiepRequirement());

        if (quyenSua.Succeeded)
        {
            string roleUserAdd = (await _userManager.GetRolesAsync(userAdd))[0];

            if (roleUserAdd == Roles.ADMIN || roleUserAdd == Roles.DOANH_NGHIEP)
            {
                var chuDoanhNghiep = new ChuDoanhNghiepModel()
                {
                    CDN_DN_Id = doanhNghiep.DN_Id,
                    CDN_ChuDN_Id = userAdd.Id
                };

                int result = await _doanhNghiepRepo.ThemSoHuuDoanhNghiepAsync(chuDoanhNghiep);

                if (result == 0)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu. Không thể cập nhật sở hữu doanh nghiệp");
                }
            }
            else
            {
                throw new Exception($"User với vai trò {roleUserAdd} không được sở hữu doanh nghiệp");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm sở hữu doanh nghiệp");
        }
    }

    public async Task TuBoSoHuuDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int result = await _doanhNghiepRepo.TuBoSoHuuDoanhNghiepAsync(id, Guid.Parse(userIdNow));

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể từ bỏ sở hữu doanh nghiệp");
        }
    }

    public async Task XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var quyenAdminDN = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new ToanQuyenDoanhNghiepRequirement());

        if (quyenAdminDN.Succeeded)
        {
            int result = await _doanhNghiepRepo.XoaSoHuuDoanhNghiepAsync(id, userId);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Không thể xóa sở hữu doanh nghiệp");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sở hữu doanh nghiệp");
        }

    }

    public async Task PhanQuyenDoanhNghiepAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var userDuocPhanQuyen = await _userManager.FindByIdAsync(phanQuyenDTO.UserId.ToString());

        if (userDuocPhanQuyen == null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }

        var roleUserDuocPhanQuyen = (await _userManager.GetRolesAsync(userDuocPhanQuyen))[0];

        if (roleUserDuocPhanQuyen == Roles.ADMIN)
        {
            throw new Exception("Không thể phân quyền cho Admin");
        }

        var quyenAdminDN = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new ToanQuyenDoanhNghiepRequirement());

        if (quyenAdminDN.Succeeded)
        {
            List<Claim> claimPhanQuyens = new List<Claim>();
            if (phanQuyenDTO.Admin)
            {
                claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Admin, id)));
            }
            else
            {
                if (phanQuyenDTO.Sua)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Sua, id)));
                }

                if (phanQuyenDTO.Xoa)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Xoa, id)));
                }
            }
            await _doanhNghiepRepo.XoaPhanQuyenDoanhNghiepAsync(id, userDuocPhanQuyen.Id);
            await _userManager.AddClaimsAsync(userDuocPhanQuyen, claimPhanQuyens);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền phân quyền doanh nghiệp");
        }
    }

    public async Task PhanQuyenSanPhamTheoDoanhNghiepAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);
        if (doanhNghiep == null) throw new Exception("Không tồn tại doanh nghiệp");
        var userDuocPhanQuyen = await _userManager.FindByIdAsync(phanQuyenDTO.UserId.ToString());

        if (userDuocPhanQuyen == null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }

        var roleUserDuocPhanQuyen = (await _userManager.GetRolesAsync(userDuocPhanQuyen))[0];

        if (roleUserDuocPhanQuyen == Roles.ADMIN)
        {
            throw new Exception("Không thể phân quyền cho Admin");
        }

        var quyenAdminSP_DN = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new ToanQuyenSanPhamRequirement());

        if (quyenAdminSP_DN.Succeeded)
        {
            List<Claim> claimPhanQuyens = new List<Claim>();
            if (phanQuyenDTO.Admin)
            {
                claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Admin, id)));
            }
            else
            {
                if (phanQuyenDTO.Them)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Them, id)));
                }

                if (phanQuyenDTO.Sua)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Sua, id)));
                }

                if (phanQuyenDTO.Xoa)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Xoa, id)));
                }
            }
            await _doanhNghiepRepo.XoaPhanQuyenSanPhamTheoDoanhNghiepAsync(id, userDuocPhanQuyen.Id);
            await _userManager.AddClaimsAsync(userDuocPhanQuyen, claimPhanQuyens);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền phân quyền sản phẩm theo doanh nghiệp");
        }
    }

    public async Task TaiLenAvatarDoanhNghiepAsync(Guid id, IFormFile avatar, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaDoanhNghiepRequirement());

        if (checkAuth.Succeeded)
        {
            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.DOANH_NGHIEP, id, ThongTinFile.KieuFile.AVATAR);

            int result = await _fileService.TaiLenAsync(new List<IFormFile>() { avatar }, ThongTinFile.KieuFile.AVATAR, KieuTaiNguyen.DOANH_NGHIEP, id, userNowFromJwt);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thiết lập ảnh đại diện thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thay đổi ảnh đại diện doanh nghiệp");
        }
    }

    public async Task XoaAvatarDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaDoanhNghiepRequirement());

        if (checkAuth.Succeeded)
        {
            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.DOANH_NGHIEP, id, ThongTinFile.KieuFile.AVATAR);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa ảnh đại diện doanh nghiệp");
        }
    }

    public async Task TaiLenAnhBiaDoanhNghiepAsync(Guid id, IFormFile coverPhoto, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaDoanhNghiepRequirement());

        if (checkAuth.Succeeded)
        {
            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.DOANH_NGHIEP, id, ThongTinFile.KieuFile.COVER_PHOTO);

            int result = await _fileService.TaiLenAsync(new List<IFormFile>() { coverPhoto }, ThongTinFile.KieuFile.COVER_PHOTO, KieuTaiNguyen.DOANH_NGHIEP, id, userNowFromJwt);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thiết lập ảnh bìa thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thay đổi ảnh bìa doanh nghiệp");
        }
    }
    public async Task XoaAnhBiaDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaDoanhNghiepRequirement());

        if (checkAuth.Succeeded)
        {
            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.DOANH_NGHIEP, id, ThongTinFile.KieuFile.COVER_PHOTO);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa ảnh bìa doanh nghiệp");
        }
    }

    public async Task<bool> KiemTraDangTheoDoiDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return await _doanhNghiepRepo.KiemTraDangTheoDoiDoanhNghiepAsync(id, userIdNow);
    }

    public async Task TheoDoiHoacHuyTheoDoiDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        bool daTheoDoi = await _doanhNghiepRepo.KiemTraDangTheoDoiDoanhNghiepAsync(id, userIdNow);

        int result = 0;

        if (daTheoDoi)
        {
            result = await _doanhNghiepRepo.HuyTheoDoiDoanhNghiepAsync(id, userIdNow);
        }
        else
        {
            var theoDoiDoanhNghiep = new TheoDoiDoanhNghiepModel()
            {
                TDDN_DN_Id = id,
                TDDN_NguoiTheoDoi_Id = userIdNow
            };
            result = await _doanhNghiepRepo.ThemTheoDoiDoanhNghiepAsync(theoDoiDoanhNghiep);
        }

        if (result == 0)
        {
            throw new Exception();
        }
    }

    public async Task<int> LaySoTheoDoiAsync(Guid id)
    {
        return await _doanhNghiepRepo.LaySoTheoDoiAsync(id);
    }
}