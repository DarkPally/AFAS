﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.ViewModel
{
    public class VMMain
    {
        private static readonly VMMain instance = new VMMain();
        public static VMMain Instance { get { return instance; } }

        private VMMain()
        {
            VMTagAnalysis.VMForensic = VMForensic;
            VMTimeAnalysis.VMForensic = VMForensic;
        }

        public VMForensic VMForensic { get; } = new VMForensic();
        public VMRuleEdit VMRuleEdit { get; } = new VMRuleEdit();
        public VMTagAnalysis VMTagAnalysis { get; } = new VMTagAnalysis();
        public VMTimeAnalysis VMTimeAnalysis { get; } = new VMTimeAnalysis();
    }
}
