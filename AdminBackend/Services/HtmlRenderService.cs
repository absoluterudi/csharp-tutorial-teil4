﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace BarBuddyBackend.Services
{
    public class HtmlRenderService
    {
        private readonly ILogger<HtmlRenderService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HtmlRenderService(ILogger<HtmlRenderService> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public string RenderHTMLBody(string templateName, Dictionary<string, string> replacedValues)
        {
            try
            {
                templateName = templateName.Replace(".html", "");

                var rootPath = _webHostEnvironment.ContentRootPath;
                var pathToFile = $"{rootPath}\\HtmlTemplates\\{templateName}.html";

                string htmlBody = null;

                using (StreamReader reader = File.OpenText(pathToFile))
                {
                    htmlBody = reader.ReadToEnd();
                }

                if (replacedValues != null)
                {
                    foreach (var item in replacedValues)
                    {
                        htmlBody = htmlBody.Replace("#" + item.Key.ToUpper() + "#", item.Value);
                    }
                }

                return htmlBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
