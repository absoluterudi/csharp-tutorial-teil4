
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace BarBuddy.DTOs.Enums
{
    public enum ApplicationEnum
    {
        Admin = 0,
        Optiker = 1,
        Augenarzt = 2,
        Bar = 3,
        NotLoggedIn = 4
    }
    public enum ImageFileExtensionEnum
    {
        jpg = 0,
        png,
        tga,
        bmp,
        unknown,
        gif
    }

    public enum ImageTypeEnum
    {
        Unknown = 0,
        Article = 1 ,  
        SupplierLogo = 2,
        Brand = 3,
        Incentive= 4,
        Praemie = 5,
        Stoerer = 6,
        PraemieThumb = 7, 
        SupplierThumb =  8, 
        IncentiveThumb =  9, 
        PvLiveOnTop= 10,
        SupplierTag = 100,   // Warum = 100, wenn wie eben PvLiveOnTop dazwischengeraet, und die Werte in der db stehen geht's durcheinander
        SupplierWeek = 101,
        SupplierBackground = 102,
        DealOfTheWeek = 103,
        ImageBannerMediathek = 104,
        ActionBoxImageOnly = 105,
        Event3dBanner1 = 200,
        Event3dBanner2 = 201,
        Event3dBanner3 = 202,
    }
}

