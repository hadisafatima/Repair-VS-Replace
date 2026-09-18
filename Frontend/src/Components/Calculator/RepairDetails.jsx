import { inputClass, labelClass, helperClass } from "./formStyles";

function RepairDetails({ formData, onChange }) {
    return (
        <div className="space-y-6">

            <div>
                <h2 className="text-xl font-semibold text-slate-900">
                    Repair Information
                </h2>

                <p className="mt-1 text-sm text-slate-500">
                    Tell us about the current condition and proposed repair.
                </p>
            </div>

            <div className="grid gap-6 sm:grid-cols-2">

                {/* Age */}
                <div>
                    <label
                        htmlFor="ageYears"
                        className={labelClass}
                    >
                        Current age
                    </label>

                    <input
                        id="ageYears"
                        type="number"
                        min="0"
                        value={formData.ageYears}
                        onChange={(e) =>
                            onChange("ageYears", e.target.value)
                        }
                        placeholder="e.g. 8"
                        className={inputClass}
                    />

                    <p className={helperClass}>
                        How many years old is the item?
                    </p>
                </div>

                {/* Repair Cost */}
                <div>
                    <label
                        htmlFor="repairCost"
                        className={labelClass}
                    >
                        Repair cost
                    </label>

                    <input
                        id="repairCost"
                        type="number"
                        min="0"
                        step="0.01"
                        value={formData.repairCost}
                        onChange={(e) =>
                            onChange("repairCost", e.target.value)
                        }
                        placeholder="e.g. 400"
                        className={inputClass}
                    />
                </div>

                {/* Previous Repairs */}
                <div>
                    <label
                        htmlFor="priorRepairCount"
                        className={labelClass}
                    >
                        Previous repairs
                    </label>

                    <input
                        id="priorRepairCount"
                        type="number"
                        min="0"
                        value={formData.priorRepairCount}
                        onChange={(e) =>
                            onChange(
                                "priorRepairCount",
                                e.target.value
                            )
                        }
                        placeholder="e.g. 2"
                        className={inputClass}
                    />

                    <p className={helperClass}>
                        Number of times this item has been repaired before.
                    </p>
                </div>

                {/* Previous Repair Cost */}
                <div>
                    <label
                        htmlFor="priorRepairTotalCost"
                        className={labelClass}
                    >
                        Total previous repair cost
                    </label>

                    <input
                        id="priorRepairTotalCost"
                        type="number"
                        min="0"
                        step="0.01"
                        value={formData.priorRepairTotalCost}
                        onChange={(e) =>
                            onChange(
                                "priorRepairTotalCost",
                                e.target.value
                            )
                        }
                        placeholder="e.g. 300"
                        className={inputClass}
                    />
                </div>

            </div>

            {/* Expected years after repair */}
            <div>
                <label
                    htmlFor="expectedYearsAfterRepair"
                    className={labelClass}
                >
                    Expected years after repair
                </label>

                <input
                    id="expectedYearsAfterRepair"
                    type="number"
                    min="1"
                    value={formData.expectedYearsAfterRepair}
                    onChange={(e) =>
                        onChange(
                            "expectedYearsAfterRepair",
                            e.target.value
                        )
                    }
                    placeholder="e.g. 3"
                    className={inputClass}
                />

                <p className={helperClass}>
                    How many more years do you expect the item to work after
                    this repair?
                </p>
            </div>

        </div>
    );
}

export default RepairDetails;