﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using Yontech.Fat.Configuration;
using Yontech.Fat.Selenium.DriverFactories;

namespace Yontech.Fat.Selenium
{
    public class SeleniumWebBrowserFactory : IWebBrowserFactory
    {
        public IWebBrowser Create(BrowserType browserType)
        {
            return this.Create(browserType, null, null);
        }

        public IWebBrowser Create(BrowserType browserType, BrowserStartOptions startOptions)
        {
            return this.Create(browserType, startOptions, null);
        }

        public IWebBrowser Create(BrowserType browserType, BrowserStartOptions startOptions, IEnumerable<IBusyCondition> busyConditions)
        {
            IWebDriver webDriver = null;
            string driversPath = Path.Combine(Environment.CurrentDirectory, "Drivers");

            switch (browserType)
            {
                case BrowserType.Chrome:
                    webDriver = ChromeDriverFactory.Create(driversPath, startOptions);
                    break;
                case BrowserType.InternetExplorer:
                    webDriver = InternetExplorerDriverFactory.Create(driversPath, startOptions);
                    break;
                default:
                    throw new NotSupportedException();
            }

            SeleniumWebBrowser browser = new SeleniumWebBrowser(
                webDriver: webDriver, 
                browserType: browserType,
                busyConditions: busyConditions ?? new List<IBusyCondition>());

            return browser;
        }
    }
}
