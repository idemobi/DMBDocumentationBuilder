#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationCodeBlockRenderer
    {
        #region Static fields and properties

        private static readonly Regex HtmlTagRegex = new(
            "<[^>]+>",
            RegexOptions.Compiled);

        private static readonly Regex InlineCodeRegex = new(
            "<code(?<attributes>[^>]*)>(?<code>[\\s\\S]*?)</code>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PreCodeRegex = new(
            "<pre[^>]*>\\s*<code[^>]*>(?<code>[\\s\\S]*?)</code>\\s*</pre>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Static methods

        private static void AppendToken(StringBuilder builder, string kind, string value)
        {
            builder
                .Append("<span class=\"dmb-code-token-")
                .Append(kind)
                .Append("\">")
                .Append(WebUtility.HtmlEncode(value))
                .Append("</span>");
        }

        private static string DecodeCodeText(string encodedCode)
        {
            string withoutTags = HtmlTagRegex.Replace(encodedCode, string.Empty);
            return WebUtility.HtmlDecode(withoutTags) ?? string.Empty;
        }

        internal static string EnhanceCSharpCodeBlocks(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;

            string enhanced = PreCodeRegex.Replace(html, match => { return RenderBlock(match.Groups["code"].Value, "Declaration", showHeader: true); });

            enhanced = InlineCodeRegex.Replace(enhanced, match =>
            {
                string attributes = match.Groups["attributes"].Value;
                if (attributes.Contains("dmb-code-block-code", StringComparison.OrdinalIgnoreCase))
                {
                    return match.Value;
                }

                string code = match.Groups["code"].Value;

                if (!LooksLikeCSharpSignature(code))
                {
                    return match.Value;
                }

                return RenderBlock(code, null, showHeader: false);
            });

            return enhanced;
        }

        private static string HighlightCSharp(string source)
        {
            StringBuilder result = new();
            int index = 0;

            while (index < source.Length)
            {
                char current = source[index];
                char next = index + 1 < source.Length ? source[index + 1] : '\0';

                if (current == '/' && next == '/')
                {
                    int end = source.IndexOf('\n', index);
                    if (end < 0) end = source.Length;

                    AppendToken(result, "comment", source[index..end]);
                    index = end;
                    continue;
                }

                if (current == '/' && next == '*')
                {
                    int end = source.IndexOf("*/", index + 2, StringComparison.Ordinal);
                    end = end < 0 ? source.Length : end + 2;
                    AppendToken(result, "comment", source[index..end]);
                    index = end;
                    continue;
                }

                if (current == '"' || current == '\'')
                {
                    int end = ReadQuoted(source, index, current);
                    AppendToken(result, "string", source[index..end]);
                    index = end;
                    continue;
                }

                if (char.IsDigit(current))
                {
                    Match number = Regex.Match(source[index..], "^\\d[\\d_]*(\\.\\d[\\d_]*)?([eE][+-]?\\d+)?[fFdDmMuUlL]*");
                    AppendToken(result, "number", number.Value);
                    index += number.Value.Length;
                    continue;
                }

                if (IsIdentifierStart(current))
                {
                    int start = index;
                    index++;

                    while (index < source.Length && IsIdentifierPart(source[index]))
                    {
                        index++;
                    }

                    string word = source[start..index];
                    string cleanWord = word[0] == '@' ? word[1..] : word;

                    if (IsCSharpKeyword(cleanWord))
                    {
                        AppendToken(result, cleanWord is "true" or "false" or "null" ? "boolean" : "keyword", word);
                    }
                    else if (IsCSharpType(cleanWord) || Regex.IsMatch(cleanWord, "^[A-Z][A-Za-z0-9_]*$"))
                    {
                        AppendToken(result, "type", word);
                    }
                    else
                    {
                        result.Append(WebUtility.HtmlEncode(word));
                    }

                    continue;
                }

                if ("{}()[];,.<>:+-*/=%!&|?".Contains(current))
                {
                    AppendToken(result, "+-*/=%!&|?".Contains(current) ? "operator" : "punctuation", current.ToString());
                    index++;
                    continue;
                }

                result.Append(WebUtility.HtmlEncode(current.ToString()));
                index++;
            }

            return result.ToString();
        }

        private static bool IsAsciiLetter(char value)
        {
            return value is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
        }

        private static bool IsCSharpKeyword(string value)
        {
            return value is
                "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or "catch" or "char" or "checked" or
                "class" or "const" or "continue" or "decimal" or "default" or "delegate" or "do" or "double" or "else" or
                "enum" or "event" or "explicit" or "extern" or "false" or "finally" or "fixed" or "float" or "for" or
                "foreach" or "get" or "global" or "goto" or "if" or "implicit" or "in" or "init" or "int" or "interface" or
                "internal" or "is" or "lock" or "long" or "namespace" or "new" or "null" or "object" or "operator" or
                "out" or "override" or "params" or "private" or "protected" or "public" or "readonly" or "record" or
                "ref" or "return" or "sbyte" or "sealed" or "set" or "short" or "sizeof" or "stackalloc" or "static" or
                "string" or "struct" or "switch" or "this" or "throw" or "true" or "try" or "typeof" or "uint" or
                "ulong" or "unchecked" or "unsafe" or "ushort" or "using" or "var" or "virtual" or "void" or "volatile" or
                "while" or "with" or "yield";
        }

        private static bool IsCSharpType(string value)
        {
            return value is
                "Action" or "Array" or "DateTime" or "DateTimeOffset" or "Dictionary" or "Func" or "Guid" or "HashSet" or
                "IEnumerable" or "IList" or "IReadOnlyCollection" or "IReadOnlyDictionary" or "IReadOnlyList" or
                "List" or "Math" or "String" or "Task" or "ValueTask";
        }

        private static bool IsIdentifierPart(char value)
        {
            return IsAsciiLetter(value) || char.IsDigit(value) || value is '_' or '@';
        }

        private static bool IsIdentifierStart(char value)
        {
            return IsAsciiLetter(value) || value is '_' or '@';
        }

        private static bool LooksLikeCSharpPropertySignature(string plainText)
        {
            Match signature = Regex.Match(
                plainText,
                "^(?<type>[A-Za-z_][A-Za-z0-9_\\.<>\\[\\],? ]+)\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)$",
                RegexOptions.IgnoreCase);

            if (!signature.Success) return false;

            string memberName = signature.Groups["name"].Value.ToLowerInvariant();

            return !IsCSharpKeyword(memberName);
        }

        private static bool LooksLikeCSharpSignature(string encodedCode)
        {
            string plainText = DecodeCodeText(encodedCode).Trim();

            if (plainText.Length == 0) return false;

            if (plainText.Contains('(') && plainText.Contains(')')) return true;

            if (Regex.IsMatch(plainText, "\\{\\s*(get|set|init)\\b|\\b(get|set|init)\\s*;", RegexOptions.IgnoreCase)) return true;

            if (plainText.StartsWith("event ", StringComparison.OrdinalIgnoreCase)) return true;

            if (LooksLikeCSharpPropertySignature(plainText)) return true;

            return Regex.IsMatch(
                plainText,
                "^(public|private|protected|internal|static|readonly|const|sealed|abstract|virtual|override)\\s+",
                RegexOptions.IgnoreCase);
        }

        private static int ReadQuoted(string source, int index, char quote)
        {
            int current = index + 1;

            while (current < source.Length)
            {
                if (source[current] == '\\')
                {
                    current += 2;
                    continue;
                }

                if (source[current] == quote)
                {
                    return current + 1;
                }

                current++;
            }

            return source.Length;
        }

        private static string RenderBlock(string encodedCode, string? title, bool showHeader)
        {
            string obsoleteClass = encodedCode.Contains("text-decoration-line-through", StringComparison.OrdinalIgnoreCase)
                ? " text-decoration-line-through text-danger"
                : string.Empty;

            string rootCompactClass = showHeader ? string.Empty : " dmb-code-block-compact my-2";
            string preCompactClass = showHeader ? string.Empty : " dmb-code-block-pre-compact";

            string result = "<div class=\"dmb-code-block dmb-code-block-default" + rootCompactClass + "\" " +
                            "data-code-block=\"true\" " +
                            "data-code-block-language=\"csharp\" " +
                            "data-code-block-highlight=\"true\" " +
                            "data-code-block-line-numbers=\"false\" " +
                            "data-code-block-theme=\"default\" " +
                            "data-code-block-state=\"normal\">";

            if (showHeader)
            {
                result += "<div class=\"dmb-code-block-header\">" +
                          "<div class=\"dmb-code-block-meta\">" +
                          "<span class=\"dmb-code-block-language dmb-code-block-language-icon-only\" title=\"C#\">" +
                          "<span class=\"bi bi-filetype-cs\" aria-hidden=\"true\"></span>" +
                          "</span>" +
                          "<span class=\"dmb-code-block-title\">" + WebUtility.HtmlEncode(title ?? "Code") + "</span>" +
                          "</div>" +
                          "<div class=\"dmb-code-block-actions\">" +
                          "<button type=\"button\" class=\"btn btn-sm btn-outline-secondary dmb-code-block-action dmb-code-block-copy\" data-code-block-copy=\"true\" title=\"Copy code\">" +
                          "<span class=\"bi bi-clipboard\" aria-hidden=\"true\"></span>" +
                          "<span class=\"visually-hidden\">Copy code</span>" +
                          "</button>" +
                          "</div>" +
                          "</div>";
            }

            string codeText = DecodeCodeText(encodedCode);

            result += "<pre class=\"dmb-code-block-pre code-language-csharp" +
                      preCompactClass +
                      "\"><code class=\"dmb-code-block-code code-language-csharp" +
                      obsoleteClass +
                      "\">" +
                      WebUtility.HtmlEncode(codeText) +
                      "</code></pre></div>";

            return result;
        }

        #endregion
    }
}