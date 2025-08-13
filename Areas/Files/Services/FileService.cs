using System.Security.Claims;
using App.Areas.Files.Models;
using App.Areas.Files.Repositories;
using App.Areas.Files.ThongTin;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Files.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IFileRepository _fileRepo;

    public FileService(IWebHostEnvironment env, IFileRepository fileRepo)
    {
        _fileRepo = fileRepo;
        _env = env;
    }

    //được gọi trong api của tài nguyên khác
    public async Task<int> TaiLenAsync(List<IFormFile> listFiles, string kieuFile, string kieuTaiNguyen, Guid taiNguyenId, ClaimsPrincipal userNowFromJwt)
    {
        ValidateFiles(listFiles);

        List<FileModel> listFileModels = new List<FileModel>();

        foreach (var file in listFiles)
        {
            var tenFile = TaoTenFile(Path.GetExtension(file.FileName));
            var duongDanFile = LayDuongDanFile(tenFile, kieuFile);

            using (FileStream fileStream = new FileStream(duongDanFile, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var fileModel = new FileModel();
            fileModel.F_Ten = tenFile;
            fileModel.F_KieuFile = kieuFile;
            fileModel.F_KieuTaiNguyen = kieuTaiNguyen;
            fileModel.F_TaiNguyenId = taiNguyenId;
            fileModel.F_KichThuoc = file.Length;
            fileModel.F_NgayTao = DateTime.Now;
            fileModel.F_NguoiTao_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            listFileModels.Add(fileModel);
        }

        return await _fileRepo.ThemNhieuAsync(listFileModels);
    }


    public async Task<int> XoaNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0)
    {
        List<FileModel> listFileModels = await _fileRepo.LayNhieuBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId, kieuFile, limit);

        if (listFileModels.Count > 0)
        {
            foreach (var fileModel in listFileModels)
            {
                var filePath = LayDuongDanFile(fileModel.F_Ten, fileModel.F_KieuFile);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return await _fileRepo.XoaNhieuAsync(listFileModels);
        }
        else
        {
            return 0;
        }
    }

    public async Task<int> XoaMotBangIdAsync(Guid id)
    {
        var fileModel = await _fileRepo.LayMotBangIdAsync(id);

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }

        var filePath = LayDuongDanFile(fileModel.F_Ten, fileModel.F_KieuFile);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return await _fileRepo.XoaMotAsync(fileModel);
    }

    //được gọi trong api của file
    public async Task<List<FileModel>> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0, bool descending = false)
    {
        List<FileModel> listFileModels = await _fileRepo.LayNhieuBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId, kieuFile, limit, descending);

        return listFileModels;
    }

    public async Task<FileModel> LayMotBangIdAsync(Guid id)
    {
        var fileModel = await _fileRepo.LayMotBangIdAsync(id);

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }

        return fileModel;
    }

    private string TaoTenFile(string extension = null)
    {
        var random = new Random();

        return DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + random.Next(0, 100) + extension;
    }

    private string LayDuongDanFile(string tenFile, string kieuFile)
    {
        if (kieuFile == ThongTinFile.KieuFile.IMAGE || kieuFile == ThongTinFile.KieuFile.AVATAR)
        {
            return Path.Combine(_env.WebRootPath, "images", tenFile);
        }
        return null;
    }

    private void ValidateFiles(List<IFormFile> listFiles)
    {
        foreach (var file in listFiles)
        {
            if (file.Length == 0)
            {
                throw new Exception($"Kích thước File: {file.FileName} không hợp lệ");
            }

            if (file.Length > ThongTinFile.MAX_SIZE)
            {
                throw new Exception($"Kích thước File: {file.FileName} quá lớn");
            }

            var extensionFile = Path.GetExtension(file.FileName);

            if (!ThongTinFile.FILE_EXTENSIONS.Contains(extensionFile))
            {
                throw new Exception($"Đuôi File: {file.FileName} không hợp lệ, chỉ được Upload File có đuôi: " + ThongTinFile.LayDuoiFileChoPhep());
            }
        }
    }
}