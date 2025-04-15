using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Document = Microsoft.CodeAnalysis.Document;

namespace CodeAnalyzers.SystemTextJson
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeAnalyzersSystemTextJsonCodeFixProvider)), Shared]
    public class CodeAnalyzersSystemTextJsonCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CodeAnalyzersSystemTextJsonAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => AddMissingParameter(context.Document, context.Span, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                context.Diagnostics);

            return Task.CompletedTask;
        }

        private static async Task<Document> AddMissingParameter(Document document, TextSpan span, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindNode(span);

            if (node is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return document;
            }

            var a1 = SyntaxFactory.ParseArgumentList("new JsonSerializerOptions()").Arguments[0];

            var newArgumentList = invocationExpressionSyntax.ArgumentList.AddArguments(a1);
            var newMethodInvocation = invocationExpressionSyntax.WithArgumentList(newArgumentList);

            var newRoot = root.ReplaceNode(invocationExpressionSyntax, newMethodInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
