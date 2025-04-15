using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = CodeAnalyzers.SystemTextJson.Test.CSharpCodeFixVerifier<
    CodeAnalyzers.SystemTextJson.CodeAnalyzersSystemTextJsonAnalyzer,
    CodeAnalyzers.SystemTextJson.CodeAnalyzersSystemTextJsonCodeFixProvider>;

namespace CodeAnalyzers.SystemTextJson.Test
{
    [TestClass]
    public class CodeAnalyzersSystemTextJsonUnitTest
    {
        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task Test()
        {
            const string sourceCode = """
                                          using System;
                                          using System.Collections.Generic;
                                          using System.Linq;
                                          using System.Text;
                                          using System.Threading.Tasks;
                                          using System.Diagnostics;
                                          using System.Text.Json;
                                      
                                          namespace ConsoleApplication1
                                          {
                                              public static class Test
                                              {
                                                  public static void DoStuff() {
                                                       string json = "{}";
                                                       JsonDocument obj = [|JsonSerializer.Deserialize<JsonDocument>(json)|];
                                                  }
                                              }
                                          }
                                      """;
            const string fixedSourceCode = """
                                          using System;
                                          using System.Collections.Generic;
                                          using System.Linq;
                                          using System.Text;
                                          using System.Threading.Tasks;
                                          using System.Diagnostics;
                                          using System.Text.Json;
                                      
                                          namespace ConsoleApplication1
                                          {
                                              public static class Test
                                              {
                                                  public static void DoStuff() {
                                                       string json = "{}";
                                                       JsonDocument obj = JsonSerializer.Deserialize<JsonDocument>(json, new JsonSerializerOptions());
                                                  }
                                              }
                                          }
                                      """;
            await VerifyCS.VerifyCodeFixAsync(sourceCode, fixedSourceCode);
        }
    }
}
