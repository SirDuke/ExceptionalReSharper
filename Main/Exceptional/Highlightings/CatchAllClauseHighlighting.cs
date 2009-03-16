/// <copyright file="CatchAllClauseHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION)]
    public class CatchAllClauseHighlighting : CSharpHighlightingBase, IHighlighting
    {
        private CatchClauseModel CatchClauseModel { get; set; }

        internal CatchAllClauseHighlighting(CatchClauseModel catchClauseModel)
        {
            CatchClauseModel = catchClauseModel;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override DocumentRange Range
        {
            get { return this.CatchClauseModel.DocumentRange; }
        }

        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private static string Message
        {
            get { return String.Format(Resources.HighLightCatchAllClauses); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}