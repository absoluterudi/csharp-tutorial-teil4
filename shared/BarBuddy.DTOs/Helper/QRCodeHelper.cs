using QRCoder;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace BarBuddy.DTOs.Helper
{
    public static class QRCodeHelper
    {
        public static string CreateQrCodeForDoor(string qrCodeSalt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(qrCodeSalt))
                {
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                    QRCodeData data = qrCodeGenerator.CreateQrCode(qrCodeSalt, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(data);
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string CreateQrCodeForSpot(string qrCodeSalt, long spotId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(qrCodeSalt) || spotId <= 0)
                {
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                    QRCodeData data = qrCodeGenerator.CreateQrCode(qrCodeSalt + "#SPOT:" + spotId, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(data);
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
