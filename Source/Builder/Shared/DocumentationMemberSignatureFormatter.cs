#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder.Shared
{
    internal static class DocumentationMemberSignatureFormatter
    {
        #region Static fields and properties

        private static readonly string[] KnownLeadingModifiers =
        [
            "public",
            "private",
            "protected",
            "internal",
            "static",
            "readonly",
            "const",
            "abstract",
            "virtual",
            "override",
            "sealed",
            "extern",
            "unsafe",
            "async",
            "partial",
            "new"
        ];

        #endregion

        #region Static methods

        private static void AddAccessibility(List<string> modifiers, ISymbol symbol)
        {
            string accessibility = BuildAccessibility(symbol.DeclaredAccessibility);
            if (!string.IsNullOrWhiteSpace(accessibility)) AddModifier(modifiers, accessibility);
        }

        private static void AddModifier(List<string> modifiers, string modifier)
        {
            if (!modifiers.Exists(x => string.Equals(x, modifier, StringComparison.Ordinal))) modifiers.Add(modifier);
        }

        private static string BuildAccessibility(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedAndInternal => "private protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => string.Empty
            };
        }

        private static List<string> BuildModifiers(IEventSymbol eventSymbol)
        {
            List<string> modifiers = [];
            AddAccessibility(modifiers, eventSymbol);
            if (eventSymbol.IsStatic) AddModifier(modifiers, "static");
            return modifiers;
        }

        private static List<string> BuildModifiers(IFieldSymbol fieldSymbol)
        {
            List<string> modifiers = [];
            AddAccessibility(modifiers, fieldSymbol);
            if (fieldSymbol.IsStatic && !fieldSymbol.IsConst) AddModifier(modifiers, "static");
            return modifiers;
        }

        private static List<string> BuildModifiers(IMethodSymbol methodSymbol)
        {
            List<string> modifiers = [];
            AddAccessibility(modifiers, methodSymbol);
            if (methodSymbol.IsStatic) AddModifier(modifiers, "static");
            if (methodSymbol.IsAbstract) AddModifier(modifiers, "abstract");
            if (methodSymbol.IsVirtual) AddModifier(modifiers, "virtual");
            if (methodSymbol.IsOverride) AddModifier(modifiers, "override");
            if (methodSymbol.IsSealed) AddModifier(modifiers, "sealed");
            return modifiers;
        }

        private static List<string> BuildModifiers(IPropertySymbol propertySymbol)
        {
            List<string> modifiers = [];
            AddAccessibility(modifiers, propertySymbol);
            if (propertySymbol.IsStatic) AddModifier(modifiers, "static");
            if (propertySymbol.IsAbstract) AddModifier(modifiers, "abstract");
            if (propertySymbol.IsVirtual) AddModifier(modifiers, "virtual");
            if (propertySymbol.IsOverride) AddModifier(modifiers, "override");
            if (propertySymbol.IsSealed) AddModifier(modifiers, "sealed");
            return modifiers;
        }

        private static string EnsureLeadingModifiers(string signature, IReadOnlyList<string> requiredModifiers)
        {
            string trimmedSignature = signature.Trim();
            if (requiredModifiers.Count == 0) return trimmedSignature;

            List<string> existingModifiers = [];
            string remainingSignature = ExtractLeadingModifiers(trimmedSignature, existingModifiers);
            List<string> finalModifiers = [];

            foreach (string modifier in requiredModifiers)
            {
                AddModifier(finalModifiers, modifier);
            }

            foreach (string modifier in existingModifiers)
            {
                AddModifier(finalModifiers, modifier);
            }

            if (finalModifiers.Count == 0) return remainingSignature;

            return $"{string.Join(" ", finalModifiers)} {remainingSignature}".Trim();
        }

        private static string ExtractLeadingModifiers(string signature, List<string> existingModifiers)
        {
            string remainingSignature = signature;

            while (true)
            {
                string? modifier = ReadLeadingModifier(remainingSignature);
                if (modifier is null) return remainingSignature;

                existingModifiers.Add(modifier);
                remainingSignature = remainingSignature[modifier.Length..].TrimStart();
            }
        }

        private static string FormatAccessor(IPropertySymbol propertySymbol, IMethodSymbol accessorSymbol, string keyword)
        {
            string propertyAccessibility = BuildAccessibility(propertySymbol.DeclaredAccessibility);
            string accessorAccessibility = BuildAccessibility(accessorSymbol.DeclaredAccessibility);

            if (string.IsNullOrWhiteSpace(accessorAccessibility) ||
                string.Equals(accessorAccessibility, propertyAccessibility, StringComparison.Ordinal))
            {
                return $"{keyword};";
            }

            return $"{accessorAccessibility} {keyword};";
        }

        internal static string FormatEvent(IEventSymbol eventSymbol)
        {
            return EnsureLeadingModifiers(
                eventSymbol.ToDisplayString(DocumentationSymbolDisplayFormat.SignatureFormat),
                BuildModifiers(eventSymbol));
        }

        internal static string FormatField(IFieldSymbol fieldSymbol)
        {
            List<string> modifiers = BuildModifiers(fieldSymbol);

            if (fieldSymbol.IsConst)
            {
                AddModifier(modifiers, "const");
            }
            else
            {
                if (fieldSymbol.IsReadOnly) AddModifier(modifiers, "readonly");
            }

            return EnsureLeadingModifiers(
                fieldSymbol.ToDisplayString(DocumentationSymbolDisplayFormat.SignatureFormat),
                modifiers);
        }

        internal static string FormatMethod(IMethodSymbol methodSymbol)
        {
            return EnsureLeadingModifiers(
                methodSymbol.ToDisplayString(DocumentationSymbolDisplayFormat.SignatureFormat),
                BuildModifiers(methodSymbol));
        }

        internal static string FormatProperty(IPropertySymbol propertySymbol)
        {
            string signature = EnsureLeadingModifiers(
                propertySymbol.ToDisplayString(DocumentationSymbolDisplayFormat.SignatureFormat),
                BuildModifiers(propertySymbol));

            if (signature.Contains("{", StringComparison.Ordinal)) return signature;

            return $"{signature} {FormatPropertyAccessors(propertySymbol)}";
        }

        private static string FormatPropertyAccessors(IPropertySymbol propertySymbol)
        {
            List<string> accessors = [];

            if (propertySymbol.GetMethod is not null)
            {
                accessors.Add(FormatAccessor(propertySymbol, propertySymbol.GetMethod, "get"));
            }

            if (propertySymbol.SetMethod is not null)
            {
                accessors.Add(FormatAccessor(propertySymbol, propertySymbol.SetMethod, "set"));
            }

            return accessors.Count == 0
                ? "{ }"
                : $"{{ {string.Join(" ", accessors)} }}";
        }

        private static string? ReadLeadingModifier(string signature)
        {
            string trimmedSignature = signature.TrimStart();

            if (StartsWithModifier(trimmedSignature, "protected internal")) return "protected internal";
            if (StartsWithModifier(trimmedSignature, "private protected")) return "private protected";

            foreach (string modifier in KnownLeadingModifiers)
            {
                if (StartsWithModifier(trimmedSignature, modifier)) return modifier;
            }

            return null;
        }

        private static bool StartsWithModifier(string signature, string modifier)
        {
            if (!signature.StartsWith(modifier, StringComparison.Ordinal)) return false;
            if (signature.Length == modifier.Length) return true;

            return char.IsWhiteSpace(signature[modifier.Length]);
        }

        #endregion
    }
}