import { Link } from "react-router-dom";

function Hero() {
    return (
        <section className="bg-slate-50">
            <div className="mx-auto max-w-7xl px-6 py-24">
                <div className="max-w-3xl">

                    <p className="mb-4 text-sm font-semibold uppercase tracking-wider text-blue-600">
                        Smart Repair & Replacement Estimator
                    </p>

                    <h1 className="text-4xl font-bold leading-tight text-slate-900 sm:text-5xl lg:text-6xl">
                        Repair it or replace it?
                        <span className="block text-blue-600">
                            Make the decision with confidence.
                        </span>
                    </h1>

                    <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-600">
                        Compare repair costs, replacement costs, expected
                        lifespan, energy usage and other factors before
                        making your decision.
                    </p>

                    <div className="mt-8 flex flex-wrap gap-4">
                        {/* <a
                            href="/calculator"
                            className="rounded-lg bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-700"
                        >
                            Start Estimator →
                        </a> */}
                        <Link to="/calculator" className="rounded-lg bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-700">Start Estimator →</Link>

                        <a
                            href="#how-it-works"
                            className="rounded-lg border border-slate-300 bg-white px-6 py-3 font-semibold text-slate-700 transition hover:bg-slate-100"
                        >
                            How It Works
                        </a>
                    </div>

                </div>
            </div>
        </section>
    );
}

export default Hero;