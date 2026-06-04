#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBPageBuilder;

#endregion

namespace DMBDocumentationBuilderLabs.Helpers
{
    /// <summary>
    ///     Provides view styling helpers used by the DMBDocumentationBuilder labs views.
    /// </summary>
    public static class DMBDocumentationBuilderLabsViewExtensions
    {
        #region PB_ParagraphBuilder

        /// <summary>
        ///     Applies muted text styling to a paragraph.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsMuted(this PB_ParagraphBuilder builder)
            => builder.AddClass("text-muted");

        /// <summary>
        ///     Applies lead paragraph styling to a paragraph.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsLead(this PB_ParagraphBuilder builder)
            => builder.AddClass("lead");

        /// <summary>
        ///     Applies small muted text styling to a paragraph.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsSmallMuted(this PB_ParagraphBuilder builder)
            => builder.AddClass("small text-muted");

        /// <summary>
        ///     Removes the bottom margin from a paragraph.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsNoMargin(this PB_ParagraphBuilder builder)
            => builder.AddClass("mb-0");

        #endregion

        #region TitleBuilder

        /// <summary>
        ///     Applies the first section heading spacing used by the labs pages.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsSectionHeading(this TitleBuilder builder)
            => builder.AddClass("mb-4");

        /// <summary>
        ///     Applies subsequent section heading spacing used by the labs pages.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsSubSectionHeading(this TitleBuilder builder)
            => builder.AddClass("mb-4 mt-5");

        /// <summary>
        ///     Applies Bootstrap h4 sizing to a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsH4(this TitleBuilder builder)
            => builder.AddClass("h4");

        /// <summary>
        ///     Removes bottom margin from a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsCompact(this TitleBuilder builder)
            => builder.AddClass("mb-0");

        /// <summary>
        ///     Applies Bootstrap alert heading styling to a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsAlertHeading(this TitleBuilder builder)
            => builder.AddClass("alert-heading");

        #endregion

        #region PB_SpanBuilder

        /// <summary>
        ///     Applies monospace font styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsMonospace(this PB_SpanBuilder builder)
            => builder.AddClass("font-monospace");

        /// <summary>
        ///     Applies small monospace font styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsMonospaceSmall(this PB_SpanBuilder builder)
            => builder.AddClass("font-monospace small");

        /// <summary>
        ///     Applies bold font styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsBold(this PB_SpanBuilder builder)
            => builder.AddClass("fw-bold");

        /// <summary>
        ///     Applies semi-bold font styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsSemiBold(this PB_SpanBuilder builder)
            => builder.AddClass("fw-semibold");

        #endregion

        #region BlockBuilder

        /// <summary>
        ///     Centers text inside a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTextCenter(this BlockBuilder builder)
            => builder.AddClass("text-center");

        /// <summary>
        ///     Applies standard section content spacing.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSectionContent(this BlockBuilder builder)
            => builder.AddClass("container-lg px-4 pb-5");

        /// <summary>
        ///     Applies standard section content spacing with top padding.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSectionContentTop(this BlockBuilder builder)
            => builder.AddClass("container-lg px-4 pt-4 pb-5");

        /// <summary>
        ///     Applies sidebar card styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarCard(this BlockBuilder builder)
            => builder.AddClass("border-0 shadow-sm rounded-4 overflow-hidden");

        /// <summary>
        ///     Applies sidebar note styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarNote(this BlockBuilder builder)
            => builder.AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm");

        /// <summary>
        ///     Applies sidebar row styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarRow(this BlockBuilder builder)
            => builder.AddClass("border-top py-2");

        /// <summary>
        ///     Applies pillar card styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPillarCard(this BlockBuilder builder)
            => builder.AddClass("p-3 border rounded-4 h-100 shadow-sm");

        /// <summary>
        ///     Applies benefit row styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsBenefitRow(this BlockBuilder builder)
            => builder.AddClass("mb-3 d-flex align-items-start");

        /// <summary>
        ///     Applies timeline step styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTimelineStep(this BlockBuilder builder)
            => builder.AddClass("position-relative ps-5 border-start border-2 border-primary mb-5");

        /// <summary>
        ///     Applies timeline badge styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <param name="bgClass">The Bootstrap background class to apply.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTimelineStepBadge(this BlockBuilder builder, string bgClass = "bg-primary")
            => builder
                .AddClass($"position-absolute top-0 start-0 translate-middle-x {bgClass} text-white rounded-circle d-flex align-items-center justify-content-center")
                .SetStyle("width", "40px")
                .SetStyle("height", "40px")
                .SetStyle("left", "-1px");

        /// <summary>
        ///     Applies information callout styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsInfoCallout(this BlockBuilder builder)
            => builder.AddClass("alert alert-info border-0 shadow-sm rounded-4 p-4 my-4");

        /// <summary>
        ///     Applies callout icon spacing.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsCalloutIcon(this BlockBuilder builder)
            => builder.AddClass("me-3");

        /// <summary>
        ///     Applies top margin spacing.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTopMargin(this BlockBuilder builder)
            => builder.AddClass("mt-3");

        /// <summary>
        ///     Applies uniform padding.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPaddedBlock(this BlockBuilder builder)
            => builder.AddClass("p-4");

        /// <summary>
        ///     Applies preview wrapper styling.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPreviewWrapper(this BlockBuilder builder)
            => builder.AddClass("rounded-3 overflow-hidden");

        #endregion

        #region RowBuilder

        /// <summary>
        ///     Applies pillar grid spacing.
        /// </summary>
        /// <param name="builder">The row builder to configure.</param>
        /// <returns>The configured <see cref="RowBuilder"/>.</returns>
        public static RowBuilder AsPillarGrid(this RowBuilder builder)
            => builder.AddClass("g-4 mt-2");

        #endregion
    }
}
