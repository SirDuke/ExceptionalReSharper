/// <copyright file="ExceptionalDaemonStageProcess.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon
    /// needs to re highlight a given file. This object is short-lived. It executes
    /// the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        public ExceptionalDaemonStageProcess(IDaemonProcess process) : base(process) { }

        public override void Execute(Action<DaemonStageResult> commiter)
        {
            HighlightInFile(file => file.ProcessDescendants(this), commiter);
        }

        public override void ProcessBeforeInterior(IElement element)
        {
            if (element is IMethodDeclaration)
            {
                var methodDeclaration = element as IMethodDeclaration;
                if(ShouldProcessMethod(methodDeclaration))
                {
                    ProcessContext.Instance.StartProcess(element as IMethodDeclaration);
                }
            }
            else if (element is IDocCommentBlockNode)
            {
                ProcessContext.Instance.Process(element as IDocCommentBlockNode);
            }
            else if (element is ITryStatement)
            {
                ProcessContext.Instance.EnterTryBlock(element as ITryStatement);
            }
            else if(element is ICatchClause)
            {
                ProcessContext.Instance.EnterCatchClause(element as ICatchClause);
            }
        }

        private static bool ShouldProcessMethod(IMethodDeclaration methodDeclaration)
        {
            return methodDeclaration.Body != null;
        }

        /// <summary>This is executed after processing the contents of a given element.</summary>
        public override void ProcessAfterInterior(IElement element)
        {
            //This call triggers visiting so it must be called first.
            base.ProcessAfterInterior(element);

            if(element is IMethodDeclaration)
            {
                var methodDeclaration = element as IMethodDeclaration;
                if(ShouldProcessMethod(methodDeclaration))
                {
                    ProcessContext.Instance.EndProcess(this);
                }
            }
            else if (element is ITryStatement)
            {
                ProcessContext.Instance.LeaveTryBlock();
            }
            else if (element is ICatchClause)
            {
                ProcessContext.Instance.LeaveCatchClause();
            }
        }

        public override void VisitThrowStatement(IThrowStatement throwStatement)
        {
            ProcessContext.Instance.Process(throwStatement);
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclaration)
        {
            ProcessContext.Instance.Process(catchVariableDeclaration);
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpression)
        {
            ProcessContext.Instance.Process(invocationExpression);
        }
    }
}