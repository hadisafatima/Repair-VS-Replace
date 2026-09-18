import { inputClass, labelClass, helperClass } from "./FormStyles";

function ReplacementDetails({ formData, onChange }) {
    return (
        <div className="space-y-6">

            <div>
                <h2 className="text-xl font-semibold text-slate-900">
                    Replacement Information
                </h2>

                <p className="mt-1 text-sm text-slate-500">
                    Tell us about the replacement option.
                </p>
            </div>

            <div className="grid gap-6 sm:grid-cols-2">

                {/* Replacement Cost */}
                <div>
                    <label
                        htmlFor="replacementCost"
                        className={labelClass}
                    >
                        Replacement cost
                    </label>

                    <input
                        id="replacementCost"
                        type="number"
                        min="0"
                        step="0.01"
                        value={formData.replacementCost}
                        onChange={(e) =>
                            onChange(
                                "replacementCost",
                                e.target.value
                            )
                        }
                        placeholder="e.g. 1200"
                        className={inputClass}
                    />
                </div>

                {/* Lifespan */}
                <div>
                    <label
                        htmlFor="expectedLifespanYears"
                        className={labelClass}
                    >
                        Expected lifespan
                    </label>

                    <input
                        id="expectedLifespanYears"
                        type="number"
                        min="1"
                        value={formData.expectedLifespanYears}
                        onChange={(e) =>
                            onChange(
                                "expectedLifespanYears",
                                e.target.value
                            )
                        }
                        placeholder="e.g. 13"
                        className={inputClass}
                    />

                    <p className={helperClass}>
                        Expected useful lifespan of the replacement.
                    </p>
                </div>

                {/* Residual Value */}
                <div className="sm:col-span-2">
                    <label
                        htmlFor="residualValue"
                        className={labelClass}
                    >
                        Residual / trade-in value
                    </label>

                    <input
                        id="residualValue"
                        type="number"
                        min="0"
                        step="0.01"
                        value={formData.residualValue}
                        onChange={(e) =>
                            onChange(
                                "residualValue",
                                e.target.value
                            )
                        }
                        placeholder="e.g. 100"
                        className={inputClass}
                    />

                    <p className={helperClass}>
                        Estimated value you could recover by selling,
                        trading in, or salvaging the current item.
                    </p>
                </div>

            </div>

        </div>
    );
}

export default ReplacementDetails;