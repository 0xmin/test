﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using SixLabors.Fonts;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class FontUtils
    {
        public static FontCollection Fonts { get; }

        static FontUtils()
        {
            Fonts = new FontCollection();

            var list = new List<string>
            {
                "Fonts/arial.ttf",
                "Fonts/arialbd.ttf",
                "Fonts/arialbi.ttf",
                "Fonts/ariali.ttf",
                "Fonts/ariblk.ttf",
                "Fonts/simhei.ttf",
            };

            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            foreach (var fileName in list)
            {
                using var reader = embeddedProvider.GetFileInfo(fileName).CreateReadStream();
                Fonts.Add(reader);
            }

            //var assembly = typeof(FontManager).Assembly;
            //var resource = assembly.GetManifestResourceStream("SS.CMS.Fonts.OpenSans-Regular.ttf");
            //var font1 = Fonts.Install(resource);
        }

        public static List<string> GetFontFamilies()
        {
            return Fonts.Families.Select(x => x.Name).ToList();
        }

        public static FontFamily GetFont(string fontName)
        {
            var fonts = Fonts.Families.Where(x => StringUtils.EqualsIgnoreCase(x.Name, fontName)).ToList();
            return fonts.Count == 0 ? DefaultFont : fonts.First();
        }

        public static FontFamily DefaultFont => Fonts.Families.First();
    }
}
