﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Reflection;
using Yontech.Fat.WebControls;
using Yontech.Fat.Selenium.WebControls;

namespace Yontech.Fat.Selenium
{
    internal class SeleniumWebBrowser : BaseWebBrowser, IWebBrowser
    {
        public readonly IWebDriver WebDriver;
        private bool _disposedValue;
        private readonly Lazy<SeleniumJsExecutor> _jsExecutorLazy;
        private readonly Lazy<SeleniumControlFinder> _seleniumControlFinderLazy;
        private readonly Lazy<IFrameControl> _frameControlLazy;

        public SeleniumWebBrowser(IWebDriver webDriver, BrowserType browserType, IEnumerable<IBusyCondition> busyConditions) : base(browserType)
        {
            this.WebDriver = webDriver;
            this._jsExecutorLazy = new Lazy<SeleniumJsExecutor>(() => new SeleniumJsExecutor(this));
            this._seleniumControlFinderLazy = new Lazy<SeleniumControlFinder>(() => new SeleniumControlFinder(this));
            this._frameControlLazy = new Lazy<IFrameControl>(() => new IFrameControl(this));

            if (busyConditions != null)
            {
                this.Configuration.BusyConditions.AddRange(busyConditions);
            }
        }

        public override IControlFinder ControlFinder => this._seleniumControlFinderLazy.Value;

        public override IJsExecutor JavaScriptExecutor => this._jsExecutorLazy.Value;

        public override IIFrameControl IFrameControl => this._frameControlLazy.Value;

        public override string CurrentUrl => WebDriver.Url;

        public override void Close()
        {
            WebDriver.Close();
        }


        public override void Navigate(string url)
        {
            WebDriver.Url = url;
            WebDriver.Navigate();
            this.WaitForIdle();
        }

        public override ISnapshot TakeSnapshot()
        {
            ITakesScreenshot takesScreenshot = WebDriver as ITakesScreenshot;
            if (takesScreenshot != null)
            {
                var shot = takesScreenshot.GetScreenshot();
                return new SeleniumSnapshot(shot);
            }
            IHasCapabilities hasCapability = WebDriver as IHasCapabilities;
            if (hasCapability == null)
            {
                throw new WebDriverException("Driver does not implement ITakesScreenshot or IHasCapabilities");
            }
            if (!hasCapability.Capabilities.HasCapability(CapabilityType.TakesScreenshot) || !(bool)hasCapability.Capabilities.GetCapability(CapabilityType.TakesScreenshot))
            {
                throw new WebDriverException("Driver capabilities do not support taking screenshots");
            }

            MethodInfo method = WebDriver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
            object[] screenshot = new object[] { DriverCommand.Screenshot, null };
            Response response = method.Invoke(WebDriver, screenshot) as Response;
            if (response == null)
            {
                throw new WebDriverException("Unexpected failure getting screenshot; response was not in the proper format.");
            }

            return new SeleniumSnapshot(new Screenshot(response.Value.ToString()));
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    WebDriver.Dispose();
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public override void Dispose()
        {
            Dispose(true);
        }

        public override void SwitchToIframe(string iframeId)
        {
            WebDriver.SwitchTo().Frame(iframeId);
        }
    }
}
