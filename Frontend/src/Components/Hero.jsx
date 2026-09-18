import { Link } from "react-router-dom";

function Hero() {
    return (
        <section className="bg-paper">
            <div className="mx-auto max-w-7xl px-6 py-24">
                <div className="max-w-3xl">

                    <h1 className="font-display text-[44px] leading-tight text-ink sm:text-[56px]">
                        <span className="text-repair">Repair</span> it or{" "}
                        <span className="text-replace">replace</span> it?
                    </h1>

                    <p className="mt-6 max-w-2xl text-[17px] leading-8 text-ink/60">
                        Compare repair costs, replacement costs, expected lifespan, and
                        energy use — then see which one actually costs less over time.
                    </p>

                    <div className="mt-8 flex flex-wrap items-center gap-4">
                        <Link
                            to="/calculator"
                            className="rounded bg-ink px-6 py-3 font-sans text-[15px] font-semibold text-paper transition hover:bg-ink/90"
                        >
                            Start estimator
                        </Link>

                        <a
                            href="#how-it-works"
                            className="rounded border border-line px-6 py-3 font-sans text-[15px] font-medium text-ink/70 transition hover:border-ink/30 hover:text-ink"
                        >
                            How it works
                        </a>
                    </div>

                </div>
            </div>
        </section>
    );
}

export default Hero;