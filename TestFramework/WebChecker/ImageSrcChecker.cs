﻿using TestFramework.Abot.Core;
using TestFramework.Abot.Poco;
using TestFramework.Abot.Util;
using HtmlAgilityPack;
using System;
using System.Xml;

namespace TestFramework.WebChecker
{
    public class ImageSrcChecker : WebChecker
    {
        public ImageSrcChecker()
            : base()
        {
        }

        public ImageSrcChecker(CrawlConfiguration crawlConfiguration)
            : base(crawlConfiguration, null, null, null, null, null, null, null, null)
        {
        }

        public ImageSrcChecker(CrawlConfiguration crawlConfiguration, ICrawlDecisionMaker crawlDecisionMaker, IThreadManager threadManager, IScheduler scheduler, IPageRequester pageRequester, IHyperLinkParser hyperLinkParser, IMemoryManager memoryManager, IDomainRateLimiter domainRateLimiter, IRobotsDotTextFinder robotsDotTextFinder)
            : base(crawlConfiguration, crawlDecisionMaker, threadManager, scheduler, pageRequester, hyperLinkParser, memoryManager, domainRateLimiter, robotsDotTextFinder)
        {
        }

        protected override void CheckThePage(string uri, HtmlDocument htmlDocument, out string errorSource)
        {
            errorSource = string.Empty;
            try
            {
                foreach (var srcNode in htmlDocument.DocumentNode.SelectNodes("//img"))
                {
                    if (srcNode.Attributes["src"] != null)
                    {
                        if (!string.IsNullOrEmpty(srcNode.Attributes["src"].Value))
                        {
                            string imageUrl = srcNode.Attributes["src"].Value;
                            if (imageUrl.StartsWith("/"))
                            {
                                imageUrl = Utility.ConvertRelativeUrl(uri, imageUrl);
                            }

                            if (!Utility.FileExist(imageUrl))
                            {
                                errorSource += srcNode.Attributes["src"].Value + ";";
                            }
                        }
                        else
                        {
                            errorSource += srcNode.XPath + ";";
                        }
                    }
                    else
                    {
                        errorSource += srcNode.XPath + ";";
                    }
                }

                if (errorSource != string.Empty)
                {
                    _logger.InfoFormat("The following resources have image src issue on url {0}\r\n{1}", uri, errorSource.Substring(0, errorSource.Length - 1));
                    errorSource = string.Format("The following resources have image src issue on url {0}\r\n{1}\r\n", uri, errorSource.Substring(0, errorSource.Length - 1));
                }
            }
            catch (Exception e)
            {
                _logger.InfoFormat("Exception is thrown when checking image src issue on url {0} with message {1}", uri, e.Message);
                errorSource = string.Format("Exception is thrown when checking image src issue on url {0} with message {1}\r\n", uri, e.Message);
            }
        }
    }
}
