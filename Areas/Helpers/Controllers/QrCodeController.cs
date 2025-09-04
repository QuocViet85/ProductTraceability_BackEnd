using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing.ImageSharp;

[ApiController]
[Route("api/[controller]")]
public class QrCodeController : ControllerBase
{
    public class Base64ImageRequest
    {
        public string ImageBase64 { get; set; }
    }

    [HttpPost("giai-ma")]
    [AllowAnonymous]
    public async Task<IActionResult> GiaiMaQrCode([FromBody] Base64ImageRequest request)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(request.ImageBase64);

            using var image = Image.Load<Rgba32>(imageBytes);
            var reader = new BarcodeReader<Rgba32>();
            var result = reader.Decode(image);

            if (result != null)
            {
                return Ok(new { success = true, data = result.Text });
            }

            return Ok(new { success = false, message = "Không tìm thấy mã QR" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}