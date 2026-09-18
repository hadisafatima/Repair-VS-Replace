function Navbar() {
    return (
        <nav className="border-b border-line bg-paper">
            <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">

                <a
                    href="/"
                    className="font-display text-xl text-ink"
                >
                    Repair<span className="text-repair">Replace</span>
                </a>

                <div className="flex items-center gap-8">
                    <a
                        href="/"
                        className="text-sm font-medium text-ink/60 hover:text-ink"
                    >
                        Home
                    </a>

                    <a
                        href="/calculator"
                        className="rounded bg-ink px-5 py-2.5 text-sm font-semibold text-paper transition hover:bg-ink/90"
                    >
                        Start estimator
                    </a>
                </div>

            </div>
        </nav>
    );
}

export default Navbar;