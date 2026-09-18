// Wrap your page's content in this. On screens narrower than the xl
// breakpoint the rails simply don't render — there's no cramped or
// horizontally-scrolling ad slot on mobile, they just disappear and the
// worksheet column takes the full width.
//
// Replace the placeholder <div> inside each <aside> with your actual
// AdSense <ins> unit once you have your ad slot IDs.

function PageLayout({ children }) {
    return (
        <div className="min-h-screen bg-paper">
            <div className="mx-auto flex max-w-[1400px] justify-center gap-8 px-6 py-12">

                <aside className="hidden w-[300px] shrink-0 xl:block">
                    <div className="sticky top-12 flex h-[600px] items-center justify-center rounded border border-dashed border-line font-mono text-xs text-ink/30">
                        Ad space — 300×600
                    </div>
                </aside>

                <main className="w-full max-w-[680px]">
                    {children}
                </main>

                <aside className="hidden w-[300px] shrink-0 xl:block">
                    <div className="sticky top-12 flex h-[600px] items-center justify-center rounded border border-dashed border-line font-mono text-xs text-ink/30">
                        Ad space — 300×600
                    </div>
                </aside>

            </div>
        </div>
    );
}

export default PageLayout;