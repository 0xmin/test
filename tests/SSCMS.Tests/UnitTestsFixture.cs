﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Configuration;
using SSCMS.Core.Services;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Tests
{
    public class UnitTestsFixture
    {
        public ISettingsManager SettingsManager { get; }

        public UnitTestsFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            var contentRootPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(dirPath)));

            //var contentRootPath = AppDomain.CurrentDomain.BaseDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile(Constants.ConfigFileName)
                .Build();

            SettingsManager = new SettingsManager(null, config, contentRootPath, PathUtils.Combine(contentRootPath, Constants.WwwrootDirectory), null);
        }
    }
}
