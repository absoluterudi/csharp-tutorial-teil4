using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Model
{
    public class ImageHolder
    {
        private bool _noImage = false;
        private byte[] _data = null;
        private string _base64String = string.Empty;

        public ImageHolder()
        {
            IsNew = true;
            _noImage = true;
        }

        public ImageHolder(byte[] data, string name)
        {
            IsNew = true;
            Name = name;
            _data = data;
        }

        //public ImageHolder(ModuleGrafik g)
        //{
        //    IsNew = false;
        //    Name = g.Ressource.ToString();
        //    Resource = g.Ressource;
        //}

        public ImageHolder(Guid g, string name = "")
        {
            IsNew = false;
            Name = name;
            Resource = g;
        }

        public ImageHolder(Guid? g)
        {
            IsNew = false;
            if (g.HasValue)
            {
                IsNew = false;
                Name = g.ToString();
                Resource = g.Value;
            }
            else
            {
                _noImage = true;
                Resource = Guid.Empty;
                Name = string.Empty;
            }
        }

        public byte[] Data => _data;

        public string Name { get; set; }

        public Guid Resource { get; set; }

        public bool IsNew { get; set; }

        public string Uri
        {
            get
            {
                if (IsNew)
                {

                    if (_data != null)
                    {
                        if (string.IsNullOrEmpty(_base64String))
                            _base64String = $"data:image/png;base64, {Convert.ToBase64String(_data)}";
                        return _base64String;
                    }

                    return $"file://{Name}";
                }

                //var res = $"{Settings.ImageUri}/api/Image/{Resource}";
                return string.Empty;
            }
        }


        //public async Task Check(ImageService imageService)
        //{
        //    if (IsNew)
        //    {
        //        var iid = await imageService.UploadImage(Name, _data);
        //        if (iid != Guid.Empty)
        //        {
        //            IsNew = false;
        //            Resource = iid;
        //        }
        //    }
        //}
    }
}
