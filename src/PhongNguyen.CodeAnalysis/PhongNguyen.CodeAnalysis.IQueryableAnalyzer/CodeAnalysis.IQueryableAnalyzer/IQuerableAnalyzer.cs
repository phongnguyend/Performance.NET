using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PhongNguyen.CodeAnalysis.IQueryableAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IQuerableAnalyzer : DiagnosticAnalyzer
    {
        public const string UseContainsMethodInExpressionRule = "IQ001";
        public const string EvaluateIQueryableInsideLoopRule = "IQ002";

        public const string UnusedLocalVariableRule = "LC001";
        public const string HighNumberOfParametersRule = "MT001";
        public const string DeepNestedBlocksRule = "MT002";

        public const string AbuseStringCompareRule = "CC001";
        public const string ComplexIfConditionRule = "CC002";

        private static Dictionary<string, DiagnosticDescriptor> _rules;
        public static Dictionary<string, DiagnosticDescriptor> Rules
        {
            get
            {
                if (_rules == null)
                {
                    _rules = new Dictionary<string, DiagnosticDescriptor>
                {
                    {UseContainsMethodInExpressionRule, new DiagnosticDescriptor(UseContainsMethodInExpressionRule, "Expression has 'Contains' method", "Using '{0}' might generate Adhoc queries.", "Optimazation", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Using 'Contains' cause generating Adhoc queries.") },
                    {EvaluateIQueryableInsideLoopRule, new DiagnosticDescriptor(EvaluateIQueryableInsideLoopRule, "Evaluate IQueryable Inside a Loop", "Using {0} inside a loop might generate multiple queries to database.", "Optimazation", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Evaluate IQueryable Inside a Loop might generate multiple queries to database.") },
                    {UnusedLocalVariableRule, new DiagnosticDescriptor(UnusedLocalVariableRule, "Unused Local Variable.", "{0} is unused.", "Optimazation", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Unused Local Variable.") },
                    {HighNumberOfParametersRule, new DiagnosticDescriptor(HighNumberOfParametersRule, "Number of Parameters.", "{0} has too many parameters ({1} parameters).", "Optimazation", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Number of Parameters.") },
                    {DeepNestedBlocksRule, new DiagnosticDescriptor(DeepNestedBlocksRule, "Deep Nested Blocks.", "{0} has deep nested blocks ({1} levels).", "Optimazation", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Deep Nested Blocks.") },

                    {AbuseStringCompareRule, new DiagnosticDescriptor(AbuseStringCompareRule, "Use string.Equals instead of string.Compare.", "Use string.Equals instead of string.Compare.", "CleanCode", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Use string.Equals instead of string.Compare.") },
                    {ComplexIfConditionRule, new DiagnosticDescriptor(ComplexIfConditionRule, "Complex If Condition.", "Complex If Condition ({0}).", "CleanCode", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Complex If Condition.") }
                };
                }
                return _rules;
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rules.Values.ToArray()); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLinqExpression, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression/*, SyntaxKind.QueryExpression*/);
            context.RegisterSyntaxNodeAction(AnalizeLoop, SyntaxKind.WhileStatement, SyntaxKind.ForStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalizeLocalVariables, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalizeMethodParameters, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalizeMethodDeepNestedBlocks, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalizeStringCompareUsage, SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(AnalizeComplexIfCondition, SyntaxKind.IfStatement);
        }

        private void AnalizeComplexIfCondition(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as IfStatementSyntax;

            if(node is null || node.Condition is null)
            {
                return;
            }

            var found = 0;
            TraverseIfCondition(context, node.Condition, ref found);

            if (found > 2)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rules[ComplexIfConditionRule], node.Condition.GetLocation(), found));
            }
        }

        private void TraverseIfCondition(SyntaxNodeAnalysisContext context, SyntaxNode node, ref int found)
        {
            foreach (var child in node.ChildTokens())
            {
                if (child.ValueText == "&&" || child.ValueText == "||")
                {
                    found++;
                }
            }

            foreach (var child in node.ChildNodes())
            {
                TraverseIfCondition(context, child, ref found);
            }
        }

        private void AnalizeStringCompareUsage(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BinaryExpressionSyntax;
            if (node != null && node.Left.ToString().StartsWith("string.Compare(") && node.OperatorToken.ValueText == "==" && node.Right.ToString() == "0")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rules[AbuseStringCompareRule], node.GetLocation(), node.ToString()));
            }
        }

        private void AnalyzeLinqExpression(SyntaxNodeAnalysisContext context)
        {
            string receiverType = string.Empty;
            string returnType = string.Empty;
            string method = string.Empty;

            var lambda = context.Node as LambdaExpressionSyntax;
            var arg = lambda.Parent as ArgumentSyntax;

            if (!lambda.Body.ToString().Contains(".Contains("))
            {
                return;
            }

            SyntaxNode foundNode = HasContainsMethodInLamdaExpression(context, lambda);

            if (foundNode == null)
            {
                return;
            }

            if (arg != null)
            {
                var parent = arg.Parent;
                while (parent != null && !(parent is ArgumentListSyntax))
                {
                    parent = parent.Parent;
                }

                if (parent != null)
                {
                    var argList = parent;
                    var invoc = argList?.Parent as InvocationExpressionSyntax;
                    if (invoc != null)
                    {
                        var si = context.SemanticModel.GetSymbolInfo(invoc.Expression).Symbol as IMethodSymbol;
                        receiverType = si?.ReceiverType?.Name;
                        returnType = si?.ReturnType?.Name;
                    }

                }
            }

            if (receiverType == "IQueryable" || returnType == "IQueryable" || receiverType == "IOrderedQueryable" || returnType == "IOrderedQueryable")
            {
                var diagnostic = Diagnostic.Create(Rules[UseContainsMethodInExpressionRule], foundNode.GetLocation(), foundNode.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }

        private SyntaxNode HasContainsMethodInLamdaExpression(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            SyntaxNode rs = null;

            foreach (var child in node.ChildNodes())
            {
                if (child is IdentifierNameSyntax)
                {
                    var identifier = child as IdentifierNameSyntax;
                    var si = context.SemanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;
                    var type = si?.ReceiverType?.Name;

                    if (identifier.Identifier.ValueText == "Contains" && !string.IsNullOrEmpty(type) && type != "String")
                    {
                        return child;
                    }
                }
                var checkChild = HasContainsMethodInLamdaExpression(context, child);
                if (checkChild != null)
                {
                    return checkChild;
                }
            }
            return rs;
        }

        private void AnalizeLoop(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            TraverseChilds(context, node);
        }

        private void TraverseChilds(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.ForEachStatement))
            {
                TraverseChilds(context, node.ChildNodes().Last());
                return;
            }

            foreach (var child in node.ChildNodes())
            {
                if (child is InvocationExpressionSyntax)
                {
                    var inv = child as InvocationExpressionSyntax;
                    var si = context.SemanticModel.GetSymbolInfo(inv).Symbol as IMethodSymbol;
                    var type = si?.ReceiverType?.Name;
                    var returnType = si?.ReturnType?.Name;

                    if ((type == "IQueryable" || type == "IOrderedQueryable") && returnType != "IQueryable" && returnType != "IOrderedQueryable")
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rules[EvaluateIQueryableInsideLoopRule], child.GetLocation(), child.ToString()));
                        return;
                    }
                }
                TraverseChilds(context, child);
            }
        }

        private void AnalizeLocalVariables(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            if (node == null || node.Body == null)
            {
                return;
            }

            var test = context.SemanticModel.AnalyzeDataFlow(node.Body);

            var unused = test.VariablesDeclared.Except(test.ReadInside);
            foreach (var symbol in unused)
            {
                if (symbol is ILocalSymbol)
                {
                    var local = symbol as ILocalSymbol;
                    context.ReportDiagnostic(Diagnostic.Create(Rules[UnusedLocalVariableRule], local.Locations[0], local.ToString()));
                }
            }
        }


        private const int _highNumberOfParemeters = 3;
        private void AnalizeMethodParameters(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;
            if (node == null || node.Body == null)
            {
                return;
            }

            var methodName = node.Identifier;
            var numberOfParas = node.ChildNodes().Where(x => x is ParameterListSyntax).FirstOrDefault()
                ?.ChildNodes().Where(x => x is ParameterSyntax).Count();

            if (numberOfParas.HasValue && numberOfParas.Value > _highNumberOfParemeters)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rules[HighNumberOfParametersRule], methodName.GetLocation(), methodName.ToString(), numberOfParas.Value));
            }
        }

        private const int _deepNestedBlocks = 3;
        private void AnalizeMethodDeepNestedBlocks(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;
            if (node == null || node.Body == null)
            {
                return;
            }

            var depthTracker = new List<int>();
            DrilldownBlocks(node, 0, depthTracker);

            var depth = depthTracker.Any() ? depthTracker.Max() : 0;

            if (depth > _deepNestedBlocks)
            {
                var methodName = node.Identifier;
                context.ReportDiagnostic(Diagnostic.Create(Rules[DeepNestedBlocksRule], methodName.GetLocation(), methodName.ToString(), depth));
            }
        }

        private void DrilldownBlocks(SyntaxNode node, int depth, List<int> depthTracker)
        {
            foreach (var child in node.ChildNodes())
            {
                if (child is BlockSyntax)
                {
                    depthTracker.Add(depth);
                    DrilldownBlocks(child, depth + 1, depthTracker);
                }

                DrilldownBlocks(child, depth, depthTracker);
            }
        }
    }
}
