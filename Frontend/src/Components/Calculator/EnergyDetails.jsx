import {
    inputClass,
    labelClass,
    helperClass,
    sectionTitleClass,
    sectionSubtitleClass,
} from "./formStyles";

function EnergyDetails({ enabled, formData, onToggle, onChange }) {
    return (
        <div className="space-y-6">

            <div>
                <h2 className={sectionTitleClass}>Energy</h2>
                <p className={sectionSubtitleClass}>
                    Optional — include this if energy efficiency matters to your decision.
                </p>
            </div>

            <label className="flex cursor-pointer items-start gap-4 border border-line px-4 py-4 transition hover:border-ink/30">
                <input
                    type="checkbox"
                    checked={enabled}
                    onChange={(e) => onToggle(e.target.checked)}
                    className="mt-1 h-4 w-4 rounded-sm border-line text-repair focus:ring-repair/30"
                />

                <div>
                    <p className="text-[15px] font-medium text-ink">
                        Include energy costs
                    </p>
                    <p className="mt-1 text-[13px] text-ink/50">
                        Compare the energy cost of keeping the current item versus replacing it.
                    </p>
                </div>
            </label>

            {enabled && (
                <div className="grid gap-6 sm:grid-cols-2">

                    <div>
                        <label htmlFor="currentAnnualKwh" className={labelClass}>
                            Current annual energy use
                        </label>
                        <input
                            id="currentAnnualKwh"
                            type="number"
                            min="0"
                            step="0.01"
                            value={formData.currentAnnualKwh}
                            onChange={(e) => onChange("currentAnnualKwh", e.target.value)}
                            placeholder="e.g. 45"
                            className={inputClass}
                        />
                        <p className={helperClass}>Annual electricity consumption in kWh.</p>
                    </div>

                    <div>
                        <label htmlFor="replacementAnnualKwh" className={labelClass}>
                            Replacement annual energy use
                        </label>
                        <input
                            id="replacementAnnualKwh"
                            type="number"
                            min="0"
                            step="0.01"
                            value={formData.replacementAnnualKwh}
                            onChange={(e) => onChange("replacementAnnualKwh", e.target.value)}
                            placeholder="e.g. 40"
                            className={inputClass}
                        />
                    </div>

                    <div>
                        <label htmlFor="energyPricePerKwh" className={labelClass}>
                            Electricity price
                        </label>
                        <input
                            id="energyPricePerKwh"
                            type="number"
                            min="0"
                            step="0.001"
                            value={formData.energyPricePerKwh}
                            onChange={(e) => onChange("energyPricePerKwh", e.target.value)}
                            placeholder="e.g. 0.15"
                            className={inputClass}
                        />
                        <p className={helperClass}>Cost per kWh in your currency.</p>
                    </div>

                    <div>
                        <label htmlFor="annualEnergyPriceInflation" className={labelClass}>
                            Annual energy price increase
                        </label>
                        <input
                            id="annualEnergyPriceInflation"
                            type="number"
                            min="0"
                            step="0.1"
                            value={formData.annualEnergyPriceInflation}
                            onChange={(e) => onChange("annualEnergyPriceInflation", e.target.value)}
                            placeholder="e.g. 0.1"
                            className={inputClass}
                        />
                        <p className={helperClass}>Expected yearly increase, as a decimal (0.1 = 10%).</p>
                    </div>

                </div>
            )}

        </div>
    );
}

export default EnergyDetails;