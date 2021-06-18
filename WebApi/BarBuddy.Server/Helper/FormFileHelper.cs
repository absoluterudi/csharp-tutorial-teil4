using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using BarBuddy.DTOs.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarBuddy.Server.Helper
{
    public class FormFileHelper : IDisposable
    {
        public void Dispose()
        {

        }
        public static long UpdateImagecontent(byte[] image, int id)
        {
            // return ImportSupplierImageToDataBase(image, filename, fileExtension, ImageTypeEnum.Supplier,SupplierId);
            int resultId = -1;
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                BarBuddy.Server.Entities.GlaukomImage dbImage;
                dbImage = db.FundusImages.Where(w => w.Id == id).FirstOrDefault();
                if (null != dbImage)
                {
                    dbImage.ByteContent = image;
                    dbImage.ModificationDate = DateTime.UtcNow;
                    if (db.SaveChanges() > 0)
                    {
                        return dbImage.Id;
                    }
                    return -1;
                }
                return -1;
            }
        }

        public static bool FileExist(string filename, string staArtnr)
        {
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                BarBuddy.Server.Entities.GlaukomImage dbImage;
                dbImage = db.FundusImages.Where(w => w.Filename == filename).FirstOrDefault();
                // dbImage = db.FundusImages.Where(w => w.StaArtnr == staArtnr).FirstOrDefault();
                if (dbImage != null)
                    return true;
                else
                    return false;
            }
        }

        public static long GetImageId(string filename)
        {
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                BarBuddy.Server.Entities.GlaukomImage dbImage;
                dbImage = db.FundusImages.Where(w => w.Filename == filename).FirstOrDefault();
                // dbImage = db.FundusImages.Where(w => w.StaArtnr == staArtnr).FirstOrDefault();
                if (dbImage != null)
                    return dbImage.Id;
                else
                    return -1;
            }
        }
        public static System.Drawing.Image ScaleBySize(System.Drawing.Image imgPhoto, int size)
        {
            var logoSize = size;

            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight;
            float destWidth;
            const int sourceX = 0;
            const int sourceY = 0;
            const int destX = 0;
            const int destY = 0;

            // Resize Image to have the height = logoSize/2 or width = logoSize.
            // Height is greater than width, set Height = logoSize and resize width accordingly
            if (sourceWidth == sourceHeight)
            {
                destWidth = logoSize;
                destHeight = logoSize;
            }
            else
            if (sourceWidth > (2 * sourceHeight))
            {
                destWidth = logoSize;
                destHeight = sourceHeight * logoSize / sourceWidth;
            }
            else
            {
                int h = logoSize / 2;
                destHeight = h;
                destWidth = sourceWidth * h / sourceHeight;
            }
            // Width is greater than height, set Width = logoSize and resize height accordingly

            var bmPhoto = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                        new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                        GraphicsUnit.Pixel);
                grPhoto.Dispose();
            }
            return bmPhoto;
        }
        private static ImageFileExtensionEnum GetExtensionType(string imagename)
        {
            string[] parts = imagename.Split(".");
            string Extension = parts[1].Trim().ToLower();
            ImageFileExtensionEnum ExtensionType = BarBuddy.DTOs.Enums.ImageFileExtensionEnum.unknown;
            switch (Extension)
            {
                case "jpg":
                    ExtensionType = ImageFileExtensionEnum.jpg;
                    break;
                case "bmp":
                    ExtensionType = ImageFileExtensionEnum.bmp;
                    break;
                case "png":
                    ExtensionType = ImageFileExtensionEnum.png;
                    break;
                case "tga":
                    ExtensionType = ImageFileExtensionEnum.tga;
                    break;
                case "gif":
                    ExtensionType = ImageFileExtensionEnum.gif;
                    break;
                default:
                    ExtensionType = ImageFileExtensionEnum.unknown;
                    // OnLog?.Invoke(string.Format("ACHTUNG StaArtikelnummer {0} hat ein unbekanntes Bildformat {1}", staArtnr, imagename));
                    break;
            }
            return ExtensionType;
        }

        public static bool LoadBlob ( GlaukomImage dbImage,
                                                        byte[] image,
                                                        string filename)
        {
            long resultId = -1;
            try
            {
                ImageFileExtensionEnum fileExtension = GetExtensionType(filename);
                dbImage.Filename = filename;
                dbImage.FileExtension = fileExtension;
                dbImage.ByteContent = image;
                // 11.03.2021
                // Jetzt noch Thumbnails erzeugen 
                // Wenn die Bildweite > 128 ist, dann auf 128 in der Width skalieren und hier speichern 
                // https://stackoverflow.com/questions/21555394/how-to-create-bitmap-from-byte-array
                Bitmap bmp;
                Image SrcImage = null;
                using (var memstr = new MemoryStream(image))
                {
                    // https://stackoverflow.com/questions/9173904/byte-array-to-image-conversion
                    SrcImage = Image.FromStream(memstr);
                    // bmp = new Bitmap(ms);
                }
                System.Drawing.Image img128 = ScaleBySize(SrcImage, 256);
                using (var memstr128 = new MemoryStream())
                {
                    img128.Save(memstr128, SrcImage.RawFormat);
                    dbImage.ByteContentThumb128 = memstr128.ToArray();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static (long, bool) SaveImageToDataBase( long optikerId, 
                                                        byte[] image,
                                                        string filename )
        {
            long resultId = -1;
            try
            {
                ImageFileExtensionEnum fileExtension = GetExtensionType(filename);
                using (ApplicationDBContext db = new ApplicationDBContext())
                {
                    BarBuddy.Server.Entities.GlaukomImage dbImage;
                    dbImage = db.FundusImages.Where(w => (w.Filename == filename)).FirstOrDefault();
                    if (null == dbImage)
                    {
                        dbImage = new BarBuddy.Server.Entities.GlaukomImage();
                        // dbImage.OptikerId = optikerId;
                        dbImage.Filename = filename;
                        dbImage.FileExtension = fileExtension;
                        db.FundusImages.Add(dbImage);
                    }
                    else
                    {
                        resultId = dbImage.Id;
                        // return (resultId,true);   // TODO 10.09.2020 Rudi, wieder rausnehmen , 09.04.2021, 09:22
                    }
                    dbImage.ByteContent = image;
                    dbImage.ModificationDate = DateTime.UtcNow;
                    // 11.03.2021
                    // Jetzt noch Thumbnails erzeugen 
                    // Wenn die Bildweite > 128 ist, dann auf 128 in der Width skalieren und hier speichern 
                    // https://stackoverflow.com/questions/21555394/how-to-create-bitmap-from-byte-array
                    Bitmap bmp;
                    Image SrcImage = null;
                    using (var memstr = new MemoryStream(image))
                    {
                        // https://stackoverflow.com/questions/9173904/byte-array-to-image-conversion
                        SrcImage = Image.FromStream(memstr);
                        // bmp = new Bitmap(ms);
                    }
                    System.Drawing.Image img128 = ScaleBySize(SrcImage, 256);
                    using (var memstr128 = new MemoryStream())
                    {
                        img128.Save(memstr128, SrcImage.RawFormat);
                        dbImage.ByteContentThumb128 = memstr128.ToArray();
                    }
                    if (db.SaveChanges() > 0)
                    {
                        resultId = dbImage.Id;
                    }
                }
                return (resultId,false);
            }
            catch (Exception ex)
            {
                return (-1, false); ;
            }
        }

        public static async Task<(long, bool)> SaveImageToDataBaseAsync(
                                                long optikerId,
                                                byte[] image,
                                                string filename )
        {
            long resultId = -1;
            try
            {
                using (ApplicationDBContext db = new ApplicationDBContext())
                {
                    BarBuddy.Server.Entities.GlaukomImage dbImage;

                    ImageFileExtensionEnum fileExtension = GetExtensionType(filename);

                    dbImage = await db.FundusImages.Where(w => (w.Filename == filename)).FirstOrDefaultAsync();
                    if (null == dbImage)
                    {
                        dbImage = new BarBuddy.Server.Entities.GlaukomImage();
                        // dbImage.OptikerId = optikerId;
                        dbImage.Filename = filename;
                        dbImage.FileExtension = fileExtension;
                        db.FundusImages.Add(dbImage);
                    }
                    else
                    {
                        resultId = dbImage.Id;
                        // return (resultId, true);   // TODO 10.09.2020 Rudi, wieder rausnehmen 
                    }
                    dbImage.ByteContent = image;
                    dbImage.ModificationDate = DateTime.UtcNow;
                    dbImage.CreationDate = DateTime.UtcNow;
                    // 11.03.2021
                    // Jetzt noch Thumbnails erzeugen 
                    // Wenn die Bildweite > 128 ist, dann auf 128 in der Width skalieren und hier speichern 
                    // https://stackoverflow.com/questions/21555394/how-to-create-bitmap-from-byte-array
                    Bitmap bmp;
                    Image SrcImage = null;
                    using (var memstr = new MemoryStream(image))
                    {
                        // https://stackoverflow.com/questions/9173904/byte-array-to-image-conversion
                        SrcImage = Image.FromStream(memstr);
                        // bmp = new Bitmap(ms);
                    }
                    System.Drawing.Image img128 = ScaleBySize(SrcImage, 128);
                    using (var memstr128 = new MemoryStream())
                    {
                        img128.Save(memstr128, SrcImage.RawFormat);
                        dbImage.ByteContentThumb128 = memstr128.ToArray();
                    }
                    if (await db.SaveChangesAsync() > 0)
                    {
                        resultId = dbImage.Id;
                    }
                }
                return (resultId, false);
            }
            catch (Exception ex)
            {
                return (-1, false); ;
            }
        }
    }
}
