using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;
using App.Areas.LoSanPham.Repositories;
using App.Areas.SanPham.Authorization;
using App.Areas.SanPham.Repositories;
using App.Areas.SuKienTruyXuat.Models;
using App.Areas.SuKienTruyXuat.Repositories;
using App.Helper;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.SuKienTruyXuat.Services;

public class SuKienTruyXuatService : ISuKienTruyXuatService
{
    private readonly ISuKienTruyXuatRepository _suKienRepo;

    private readonly ISanPhamRepository _sanPhamRepo;

    private readonly ILoSanPhamRepository _loSanPhamRepo;

    private readonly IAuthorizationService _authorizationService;

    private readonly IFileService _fileService;

    public SuKienTruyXuatService(ISuKienTruyXuatRepository suKienRepo, ISanPhamRepository sanPhamRepo, ILoSanPhamRepository loSanPhamRepo, IAuthorizationService authorizationService, IFileService fileService)
    {
        _suKienRepo = suKienRepo;
        _sanPhamRepo = sanPhamRepo;
        _loSanPhamRepo = loSanPhamRepo;
        _authorizationService = authorizationService;
        _fileService = fileService;
    }

    public async Task<(int totalItems, List<SuKienTruyXuatModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _suKienRepo.LayTongSoAsync();
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SuKienTruyXuatModel> listSuKienTruyXuats = await _suKienRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listSuKienTruyXuats);
    }

    public async Task<(int totalItems, List<SuKienTruyXuatModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _suKienRepo.LayTongSoBangSanPhamAsync(sp_id);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<SuKienTruyXuatModel> listSuKienTruyXuats = await _suKienRepo.LayNhieuBangSanPhamAsync(sp_id, pageNumber, limit, search, descending);

        return (tongSo, listSuKienTruyXuats);
    }

    public async Task<SuKienTruyXuatModel> LayMotBangIdAsync(Guid id)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangIdAsync(id);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tìm thấy sự kiện truy xuất");
        }

        return suKienTruyXuat;
    }

    public async Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangMaSuKienAsync(sk_MaSK);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tìm thấy sự kiện truy xuất");
        }

        return suKienTruyXuat;
    }

    public async Task<SuKienTruyXuatModel> ThemAsync(SuKienTruyXuatModel suKienTruyXuatNew, ClaimsPrincipal userNowFromJwt)
    {
        if (suKienTruyXuatNew.SK_SP_Id == null)
        {
            throw new Exception("Phải nhập sản phẩm cho sự kiện truy xuất");
        }

        Guid sp_id = suKienTruyXuatNew.SK_SP_Id;
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(sp_id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm nền không thể thêm sự kiện cho sản phẩm");
        }

        if (suKienTruyXuatNew.SK_LSP_Id != null)
        {
            var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync((Guid)suKienTruyXuatNew.SK_LSP_Id);

            if (loSanPham == null)
            {
                throw new Exception("Lô sản phẩm không tồn tại");
            }

            if (loSanPham.LSP_SP_Id != sanPham.SP_Id)
            {
                throw new Exception("Không thể thêm lô sản phẩm của sản phẩm khác sản phẩm của sự kiện");
            }
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement()); //Bản chất của sự kiện truy xuất vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            if (suKienTruyXuatNew.SK_MaSK != null)
            {
                bool daTonTaiMaSuKien = await _suKienRepo.KiemTraTonTaiBangMaSuKienAsync(suKienTruyXuatNew.SK_MaSK);

                if (daTonTaiMaSuKien)
                {
                    throw new Exception("Mã sự kiện truy xuất đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }
            }
            else
            {
                suKienTruyXuatNew.SK_MaSK = CreateCode.GenerateCodeFromTicks();
            }

            if (suKienTruyXuatNew.SK_ThoiGian.Year < 1970)
            {
                suKienTruyXuatNew.SK_ThoiGian = DateTime.Now;
            }
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            suKienTruyXuatNew.SK_NguoiTao_Id = Guid.Parse(userIdNow);
            suKienTruyXuatNew.SK_NgayTao = DateTime.Now;

            var result = await _suKienRepo.ThemAsync(suKienTruyXuatNew);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo sự kiện truy xuất thất bại");
            }
            return suKienTruyXuatNew;
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền tạo sự kiên truy xuất với lô hàng này");
        }
    }

    public async Task SuaAsync(Guid id, SuKienTruyXuatModel suKienTruyXuatUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangIdAsync(id);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tồn tại sự kiện truy xuất");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, suKienTruyXuat.SK_LSP.LSP_SP, new SuaSanPhamRequirement()); //Bản chất của sự kiện truy xuất vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            if (suKienTruyXuatUpdate.SK_MaSK != null)
            {
                bool daTonTaiMaSuKien = await _suKienRepo.KiemTraTonTaiBangMaSuKienAsync(suKienTruyXuatUpdate.SK_MaSK, id);

                if (daTonTaiMaSuKien)
                {
                    throw new Exception("Mã sự kiện truy xuất đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                suKienTruyXuat.SK_MaSK = suKienTruyXuatUpdate.SK_MaSK;
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            suKienTruyXuat.SK_Ten = suKienTruyXuatUpdate.SK_Ten;
            suKienTruyXuat.SK_MoTa = suKienTruyXuatUpdate.SK_MoTa;
            suKienTruyXuat.SK_DiaDiem = suKienTruyXuatUpdate.SK_DiaDiem;
            suKienTruyXuat.SK_ThoiGian = suKienTruyXuatUpdate.SK_ThoiGian;
            suKienTruyXuat.SK_JsonData = suKienTruyXuatUpdate.SK_JsonData;

            if (suKienTruyXuatUpdate.SK_LSP_Id != null && suKienTruyXuatUpdate.SK_LSP_Id != suKienTruyXuat.SK_LSP_Id)
            {
                var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync((Guid)suKienTruyXuatUpdate.SK_LSP_Id);

                if (loSanPham != null && loSanPham.LSP_SP_Id == suKienTruyXuat.SK_SP?.SP_Id)
                {
                    suKienTruyXuat.SK_LSP_Id = suKienTruyXuatUpdate.SK_LSP_Id;
                }
            }

            suKienTruyXuat.SK_NguoiSua_Id = Guid.Parse(userIdNow);
            suKienTruyXuat.SK_NgaySua = DateTime.Now;

            var result = await _suKienRepo.SuaAsync(suKienTruyXuat);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo sự kiện truy xuất thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa sự kiện truy xuất của lô hàng này");
        }
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangIdAsync(id);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tồn tại sự kiện truy xuất");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, suKienTruyXuat.SK_SP, new SuaSanPhamRequirement()); //Bản chất của sự kiện truy xuất vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var result = await _suKienRepo.XoaAsync(suKienTruyXuat);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sự kiện truy xuất thất bại");
            }

            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.SK_TRUY_XUAT, id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sự kiện truy xuất này");
        }
    }

    public async Task TaiLenAnhSuKienAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangIdAsync(id);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tồn tại sự kiện truy xuất");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, suKienTruyXuat.SK_LSP.LSP_SP, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _fileService.TaiLenAsync(listFiles, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.SK_TRUY_XUAT, id, userNowFromJwt);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Đăng ảnh thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền đăng ảnh cho sự kiện truy xuất này");
        }
    }

    public async Task XoaAnhSuKienAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt)
    {
        var suKienTruyXuat = await _suKienRepo.LayMotBangIdAsync(id);

        if (suKienTruyXuat == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var file = await _fileService.LayMotBangIdAsync(f_id);

        if (file == null)
        {
            throw new Exception("Không tồn tại ảnh");
        }

        if (file.F_KieuTaiNguyen == KieuTaiNguyen.SK_TRUY_XUAT && file.F_TaiNguyen_Id == id && file.F_KieuFile == ThongTinFile.KieuFile.IMAGE)
        {
            var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, suKienTruyXuat.SK_LSP.LSP_SP, new SuaSanPhamRequirement());

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
                throw new UnauthorizedAccessException("Không có quyền xóa ảnh của sự kiện truy xuất này");
            }
        }
        else
        {
            throw new Exception("Ảnh này không phải của sự kiện truy xuất này nên không thể xóa");
        }
    }
    

    //Not Implement
    public Task<(int totalItems, List<SuKienTruyXuatModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
    
}