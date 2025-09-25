using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;
using App.Areas.LoSanPham.Models;
using App.Areas.LoSanPham.Repositories;
using App.Areas.NhaMay.Repositories;
using App.Areas.SanPham.Authorization;
using App.Areas.SanPham.Repositories;
using App.Helper;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.LoSanPham.Services;

public class LoSanPhamService : ILoSanPhamService
{
    private readonly ILoSanPhamRepository _loSanPhamRepo;
    private readonly ISanPhamRepository _sanPhamRepo;
    private readonly INhaMayRepository _nhaMayRepo;
    private readonly IAuthorizationService _authorizationService;
    private readonly IFileService _fileService;

    public LoSanPhamService(ILoSanPhamRepository loSanPhamRepo, ISanPhamRepository sanPhamRepo, INhaMayRepository nhaMayRepo, IAuthorizationService authorizationService, IFileService fileService)
    {
        _loSanPhamRepo = loSanPhamRepo;
        _sanPhamRepo = sanPhamRepo;
        _nhaMayRepo = nhaMayRepo;
        _authorizationService = authorizationService;
        _fileService = fileService;
    }

    public async Task<(int totalItems, List<LoSanPhamModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _loSanPhamRepo.LayTongSoBangSanPhamAsync(sp_Id);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<LoSanPhamModel> listLoSanPhams = await _loSanPhamRepo.LayNhieuBangSanPhamAsync(sp_Id, pageNumber, limit, search, descending);

        return (tongSo, listLoSanPhams);
    }

    public async Task<LoSanPhamModel> LayMotBangIdAsync(Guid id)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync(id);

        if (loSanPham == null)
        {
            throw new Exception("Không tìm thấy lô hàng");
        }
        return loSanPham;
    }

    public async Task<LoSanPhamModel> LayMotBangMaLoSanPhamAsync(string lsp_MaLSP)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangMaLoSanPhamAsync(lsp_MaLSP);

        if (loSanPham == null)
        {
            throw new Exception("Không tìm thấy lô hàng");
        }

        return loSanPham;
    }

    public async Task ThemAsync(LoSanPhamModel loSanPhamNew, ClaimsPrincipal userNowFromJwt)
    {
        if (loSanPhamNew.LSP_SP_Id == null)
        {
            throw new Exception("Phải nhập sản phẩm cho lô hàng");
        }
        Guid sp_Id = (Guid)loSanPhamNew.LSP_SP_Id;
        var sanPham = await _sanPhamRepo.LayMotBangIdAsync(sp_Id);

        if (sanPham == null)
        {
            throw new Exception("Không tồn tại sản phẩm nền không thể thêm lô hàng cho sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, sanPham, new SuaSanPhamRequirement()); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            if (loSanPhamNew.LSP_MaLSP != null)
            {
                bool daTonTaiMaLoSanPham = await _loSanPhamRepo.KiemTraTonTaiBangMaLoSanPhamAsync(loSanPhamNew.LSP_MaLSP);

                if (daTonTaiMaLoSanPham)
                {
                    throw new Exception("Mã lô sản phẩm đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }
            }
            else
            {
                loSanPhamNew.LSP_MaLSP = CreateCode.GenerateCodeFromTicks();
            }

            if (loSanPhamNew.LSP_NM_Id != null)
            {
                bool daTonTaiNhaMay = await _nhaMayRepo.KiemTraTonTaiBangIdAsync((Guid)loSanPhamNew.LSP_NM_Id);
                if (!daTonTaiNhaMay)
                {
                    loSanPhamNew.LSP_NM_Id = null;
                }
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            loSanPhamNew.LSP_NguoiTao_Id = Guid.Parse(userIdNow);

            var result = await _loSanPhamRepo.ThemAsync(loSanPhamNew);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo lô hàng thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền tạo lô hàng với sản phẩm này");
        }

    }

    public async Task SuaAsync(Guid id, LoSanPhamModel loSanPhamUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync(id);

        if (loSanPham == null)
        {
            throw new Exception("Không tồn tại lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, loSanPham.LSP_SP, new SuaSanPhamRequirement()); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            if (loSanPhamUpdate.LSP_MaLSP != null)
            {
                bool daTonTaiMaLoSanPham = await _loSanPhamRepo.KiemTraTonTaiBangMaLoSanPhamAsync(loSanPhamUpdate.LSP_MaLSP, id);

                if (daTonTaiMaLoSanPham)
                {
                    throw new Exception("Mã lô sản phẩm đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                loSanPham.LSP_MaLSP = loSanPhamUpdate.LSP_MaLSP;
            }

            if (loSanPhamUpdate.LSP_NM_Id != null)
            {
                bool daTonTaiNhaMay = await _nhaMayRepo.KiemTraTonTaiBangIdAsync((Guid)loSanPhamUpdate.LSP_NM_Id);
                if (!daTonTaiNhaMay)
                {
                    loSanPham.LSP_NM_Id = null;
                }
                else
                {
                    loSanPham.LSP_NM_Id = loSanPhamUpdate.LSP_NM_Id;
                }
            }
            else
            {
                loSanPham.LSP_NM_Id = loSanPhamUpdate.LSP_NM_Id;
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            loSanPham.LSP_Ten = loSanPhamUpdate.LSP_Ten;
            loSanPham.LSP_NgaySanXuat = loSanPhamUpdate.LSP_NgaySanXuat;
            loSanPham.LSP_NgayHetHan = loSanPhamUpdate.LSP_NgayHetHan;
            loSanPham.LSP_SoLuong = loSanPhamUpdate.LSP_SoLuong;
            loSanPham.LSP_MoTa = loSanPhamUpdate.LSP_MoTa;
            loSanPham.LSP_JsonData = loSanPhamUpdate.LSP_JsonData;
            
            loSanPham.LSP_NguoiSua_Id = Guid.Parse(userIdNow);
            loSanPham.LSP_NgayTao = DateTime.Now;

            var result = await _loSanPhamRepo.SuaAsync(loSanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo lô hàng thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa lô hàng của sản phẩm này");
        }
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync(id);

        if (loSanPham == null)
        {
            throw new Exception("Không tồn tại lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, loSanPham.LSP_SP, new SuaSanPhamRequirement()); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var result = await _loSanPhamRepo.XoaAsync(loSanPham);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa lô hàng thất bại");
            }

            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.LO_SAN_PHAM, id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa lô hàng của sản phẩm này");
        }
    }

    public async Task TaiLenAnhLoSanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync(id);

        if (loSanPham == null)
        {
            throw new Exception("Không tồn tại lô sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, loSanPham.LSP_SP, new SuaSanPhamRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _fileService.TaiLenAsync(listFiles, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.LO_SAN_PHAM, id, userNowFromJwt);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Đăng ảnh thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền đăng ảnh cho lô sản phẩm này");
        }
    }

    public async Task XoaAnhLoSanPhamAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt)
    {
        var loSanPham = await _loSanPhamRepo.LayMotBangIdAsync(id);

        if (loSanPham == null)
        {
            throw new Exception("Không tồn tại lô sản phẩm");
        }

        var file = await _fileService.LayMotBangIdAsync(f_id);

        if (file == null)
        {
            throw new Exception("Không tồn tại ảnh");
        }

        if (file.F_KieuTaiNguyen == KieuTaiNguyen.LO_SAN_PHAM && file.F_TaiNguyen_Id == id && file.F_KieuFile == ThongTinFile.KieuFile.IMAGE)
        {
            var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, loSanPham.LSP_SP, new SuaSanPhamRequirement());

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
                throw new UnauthorizedAccessException("Không có quyền xóa ảnh của lô sản phẩm này");
            }
        }
        else
        {
            throw new Exception("Ảnh này không phải của lô sản phẩm này nên không thể xóa");
        }
    }

    public async Task<Guid?> LayDoanhNghiepSoHuuIdAsync(Guid id)
    {
        return await _loSanPhamRepo.LayDoanhNghiepSoHuuIdAsync(id);
    }
    
    //Not Implement
    public Task<(int totalItems, List<LoSanPhamModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalItems, List<LoSanPhamModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}