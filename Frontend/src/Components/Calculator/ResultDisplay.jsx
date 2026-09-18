import PageLayout from "../PageLayout";
import { buttonSecondaryClass, dividerClass } from "./FormStyles";

const RECOMMENDATION_STYLES = {
    Repair: {
        text: "text-repair",
        bar: "bg-repair",
    },
    Replace: {
        text: "text-replace",
        bar: "bg-replace",
    },
    Borderline: {
        text: "text-ink",
        bar: "bg-ink/40",
    },
};

function formatCurrency(value, currencyCode) {
    if (value === null || value === undefined) return "—";

    try {
        return new Intl.NumberFormat(undefined, {
            style: "currency",
            currency: currencyCode || "PKR",
            maximumFractionDigits: 0,
        }).format(value);
    } catch {
        return `${currencyCode || "PKR"} ${Math.round(value)}`;
    }
}

function CostRow({ label, path, currencyCode, accentClass }) {
    return (
        <div className="flex items-start gap-4 py-4">
            <div className={"mt-1 h-full w-1 shrink-0 self-stretch " + accentClass} />
            <div className="flex-1">
                <p className="text-[13px] font-medium text-ink/60">{label}</p>
                <p className="mt-1 font-mono text-2xl text-ink">
                    {formatCurrency(path.costPerYearOfService, currencyCode)}
                    <span className="ml-1 text-sm font-sans text-ink/40">/ year</span>
                </p>
                <p className="mt-1 font-mono text-[13px] text-ink/50">
                    {formatCurrency(path.totalCostOfOwnership, currencyCode)} over {path.expectedServiceYears} yr
                </p>
            </div>
        </div>
    );
}

const DIRECTION_DOT = {
    FavorsRepair: "bg-repair",
    FavorsReplace: "bg-replace",
};

function FactorRow({ factor }) {
    return (
        <div className="flex items-start gap-3 py-3">
            <span
                className={
                    "mt-1.5 h-2 w-2 shrink-0 rounded-full " +
                    (DIRECTION_DOT[factor.direction] ?? "bg-ink/20")
                }
            />
            <div className="flex-1">
                <div className="flex items-baseline justify-between gap-4">
                    <p className="text-[14px] text-ink">{factor.name}</p>
                    <p className="shrink-0 font-mono text-[13px] text-ink/40">
                        {factor.contribution > 0 ? `+${factor.contribution.toFixed(2)}` : "0.00"}
                    </p>
                </div>
                <p className="mt-0.5 text-[13px] text-ink/50">{factor.explanation}</p>
            </div>
        </div>
    );
}

function ResultDisplay({ result, onReset }) {
    const style = RECOMMENDATION_STYLES[result.recommendation] ?? RECOMMENDATION_STYLES.Borderline;

    return (
        <PageLayout>
            <div className="mb-10">
                <h1 className="font-display text-[36px] leading-tight text-ink">
                    Your decision
                </h1>
                <p className="mt-2 text-[15px] text-ink/60">
                    Based on the numbers you entered.
                </p>
            </div>

            <div className="relative border border-line bg-surface px-8 pb-8 pt-10">
                {/* Perforated edge, receipt-style */}
                <div
                    className="absolute inset-x-0 top-0 h-0 border-t-2 border-dashed border-line"
                    aria-hidden="true"
                />

                <p className="text-[13px] font-medium text-ink/50">Recommendation</p>
                <p className={"mt-2 font-display text-[44px] leading-none " + style.text}>
                    {result.recommendation}
                </p>

                <p className="mt-4 max-w-[52ch] text-[15px] text-ink/70">
                    {result.summary}
                </p>

                {result.uncertainty && (
                    <p className="mt-3 font-mono text-[13px] text-ink/40">
                        {Math.round(result.uncertainty.confidenceLevel * 100)}% confidence
                    </p>
                )}

                <div className={"mt-8 " + dividerClass}>
                    <CostRow
                        label="Repair"
                        path={result.repairPath}
                        currencyCode={result.currencyCode}
                        accentClass="bg-repair"
                    />
                    <div className={dividerClass} />
                    <CostRow
                        label="Replace"
                        path={result.replacePath}
                        currencyCode={result.currencyCode}
                        accentClass="bg-replace"
                    />
                </div>

                <div className={"mt-2 grid grid-cols-2 gap-4 pt-6 " + dividerClass}>
                    <div>
                        <p className="text-[13px] text-ink/50">Net advantage</p>
                        <p className="mt-1 font-mono text-lg text-ink">
                            {formatCurrency(Math.abs(result.netAdvantage), result.currencyCode)}
                        </p>
                    </div>

                    {result.breakEvenYears !== null && result.breakEvenYears !== undefined && (
                        <div>
                            <p className="text-[13px] text-ink/50">Break-even</p>
                            <p className="mt-1 font-mono text-lg text-ink">
                                {result.breakEvenYears} yr
                            </p>
                        </div>
                    )}
                </div>

                {result.factors && result.factors.length > 0 && (
                    <div className={"mt-2 pt-6 " + dividerClass}>
                        <p className="mb-1 text-[13px] font-medium text-ink/50">
                            What's driving this
                        </p>
                        <div className="divide-y divide-line">
                            {result.factors.map((factor) => (
                                <FactorRow key={factor.name} factor={factor} />
                            ))}
                        </div>
                    </div>
                )}

                {result.uncertainty && !result.uncertainty.isVerdictStable && (
                    <p className="mt-6 border-l-2 border-ink/20 pl-3 text-[13px] text-ink/50">
                        This recommendation is close — a ±20% change in cost estimates could
                        flip the result. Treat it as a lean, not a certainty.
                    </p>
                )}
            </div>

            <button
                type="button"
                onClick={onReset}
                className={buttonSecondaryClass + " mt-8"}
            >
                Calculate again
            </button>
        </PageLayout>
    );
}

export default ResultDisplay;