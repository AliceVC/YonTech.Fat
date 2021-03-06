﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yontech.Fat.WebControls
{
    public interface ITextBoxControl : IWebControl
    {
        string Text { get; }
        void SendKeys(string keys);
        void ClearText();
        void Click();
    }
}
