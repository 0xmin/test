﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isCloudVod = _vodManager is ICloudManager && vodSettings.IsVod;

            var options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerVideoUploadController)}"), new Options
            {
                IsChangeFileName = true,
                IsLibrary = false,
            });

            return new GetResult
            {
                IsChangeFileName = options.IsChangeFileName,
                IsLibrary = options.IsLibrary,
                IsCloudVod = isCloudVod
            };
        }
    }
}
