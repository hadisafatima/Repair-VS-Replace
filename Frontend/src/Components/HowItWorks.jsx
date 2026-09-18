const STEPS = [
    {
        number: 1,
        title: "Answer a few questions",
        description:
            "Tell it what you're deciding on, the repair cost, and what a replacement would cost. Takes about a minute.",
    },
    {
        number: 2,
        title: "It runs the numbers",
        description:
            "Cost per year, total cost of ownership, and break-even point — calculated instantly, nothing you enter is saved anywhere.",
    },
    {
        number: 3,
        title: "Get a clear recommendation",
        description:
            "Repair, replace, or a genuine toss-up — with the reasoning behind it shown, not just the verdict.",
    },
];

function HowItWorks() {
    return (
        <section id="how-it-works" className="border-t border-line bg-surface">
            <div className="mx-auto max-w-7xl px-6 py-24">

                <h2 className="font-display text-[32px] leading-tight text-ink">
                    How it works
                </h2>

                <div className="mt-12 grid gap-10 sm:grid-cols-3 sm:gap-8">
                    {STEPS.map((step) => (
                        <div key={step.number} className="relative pl-14 sm:pl-0">

                            <span className="absolute left-0 top-0 flex h-10 w-10 items-center justify-center rounded-full bg-ink font-mono text-[14px] text-paper sm:static sm:mb-6">
                                {step.number}
                            </span>

                            <h3 className="font-display text-[20px] text-ink">
                                {step.title}
                            </h3>

                            <p className="mt-2 text-[15px] leading-relaxed text-ink/60">
                                {step.description}
                            </p>

                        </div>
                    ))}
                </div>

            </div>
        </section>
    );
}

export default HowItWorks;