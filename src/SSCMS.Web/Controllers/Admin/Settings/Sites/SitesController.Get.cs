﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();
            //var siteIdList = await _siteRepository.GetSiteIdListOrderByLevelAsync();
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{

            //    var site = await _siteRepository.GetAsync(siteId);
            //    if (string.IsNullOrEmpty(keyword) || site.SiteName.Contains(keyword) || site.TableName.Contains(keyword) || site.SiteDir.Contains(keyword))
            //    {
            //        sites.Add(site);
            //    }
            //}
            //var siteIdList = await _siteRepository.GetSiteIdListAsync(0);
            //var sites = new List<Site>();
            //foreach (var siteId in siteIdList)
            //{
            //    var site = await _siteRepository.GetAsync(siteId);
            //    if (site != null)
            //    {
            //        sites.Add(site);
            //    }
            //}

            var siteTypes = _settingsManager.GetSiteTypes();

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x =>
            {
                var parentIds = await _siteRepository.GetParentIds(x.Id);
                if (parentIds.Count == 0)
                {
                    parentIds.Add(0);
                }
                return new
                {
                    SiteUrl = await _pathManager.GetSiteUrlAsync(x, false),
                    ParentIds = parentIds,
                };
            });
            var parentSites = await _siteRepository.GetCascadeChildrenAsync(0);
            parentSites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<无上级站点>"
            });

            var tableNames = await _siteRepository.GetSiteTableNamesAsync();

            return new GetResult
            {
                SiteTypes = siteTypes,
                Sites = sites,
                RootSiteId = rootSiteId,
                TableNames = tableNames,
                ParentSites = parentSites
            };
        }
    }
}