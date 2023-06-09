﻿using System;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Enums;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class CreateJob : IJobService
    {
        public string CommandName => "create";

        private string _directory;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICreateManager _createManager;
        private readonly OptionSet _options;

        public CreateJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, ICreateManager createManager)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _createManager = createManager;
            _options = new OptionSet
            {
                {
                    "d|directory=", "Site directory name",
                    v => _directory = v
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: create static pages");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            var directory = _directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = string.Empty;
            }

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (!FileUtils.IsFileExists(configPath))
            {
                await console.WriteErrorAsync($"The sscms.json file does not exist: {configPath}");
                return;
            }

            await console.WriteLineAsync($"Database type: {_settingsManager.DatabaseType.GetDisplayName()}");
            await console.WriteLineAsync($"Database connection string: {_settingsManager.DatabaseConnectionString}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message: {errorMessage}");
                return;
            }

            var site = await _databaseManager.SiteRepository.GetSiteByDirectoryAsync(directory);
            if (site == null)
            {
                await console.WriteErrorAsync($"Unable to find the site, directory: {directory}");
                return;
            }
            await console.WriteLineAsync($"site: {site.SiteName}");

            await _createManager.ExecuteAsync(site.Id, CreateType.All, 0, 0, 0, 0);

            await console.WriteSuccessAsync("create pages successfully!");
        }
    }
}
