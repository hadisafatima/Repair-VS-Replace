import { useState } from "react";

import ProductSelector from "./ProductSelector";
import RepairDetails from "./RepairDetails";
import ReplacementDetails from "./ReplacementDetails";
import EnergyDetails from "./EnergyDetails";
import PageLayout from "../PageLayout";

import {
    dividerClass,
    buttonPrimaryClass,
    buttonSecondaryClass,
} from "./FormStyles";

import { calculateRepairReplace } from "../../Services/CalculationService";

const STEPS = [
    { number: 1, label: "The item" },
    { number: 2, label: "Replacement" },
    { number: 3, label: "Energy" },
];

const initialFormData = {
    productTypeId: "",

    ageYears: "",
    repairCost: "",
    priorRepairCount: "",
    priorRepairTotalCost: "",
    expectedYearsAfterRepair: "",

    replacementCost: "",
    expectedLifespanYears: "",
    residualValue: "",

    currentAnnualKwh: "",
    replacementAnnualKwh: "",
    energyPricePerKwh: "",
    annualEnergyPriceInflation: "",

    // currencyCode: "PKR",
};

function StepRail({ step }) {
    return (
        <div className="mb-10 flex items-center">
            {STEPS.map((item, index) => (
                <div key={item.number} className="flex flex-1 items-center last:flex-none">
                    <div className="flex items-center gap-3">
                        <div
                            className={
                                "flex h-8 w-8 shrink-0 items-center justify-center rounded-full font-mono text-[13px] " +
                                (item.number === step
                                    ? "bg-ink text-paper"
                                    : item.number < step
                                        ? "bg-repair/15 text-repair"
                                        : "bg-transparent text-ink/30 ring-1 ring-inset ring-line")
                            }
                        >
                            {item.number}
                        </div>
                        <span
                            className={
                                "hidden text-sm sm:inline " +
                                (item.number === step ? "text-ink" : "text-ink/40")
                            }
                        >
                            {item.label}
                        </span>
                    </div>

                    {index < STEPS.length - 1 && (
                        <div
                            className={
                                "mx-4 h-px flex-1 " +
                                (item.number < step ? "bg-repair/40" : "bg-line")
                            }
                        />
                    )}
                </div>
            ))}
        </div>
    );
}

function CalculatorForm({ onResult }) {
    const [step, setStep] = useState(1);

    const [formData, setFormData] =
        useState(initialFormData);

    const [includeEnergy, setIncludeEnergy] =
        useState(false);

    const [error, setError] = useState("");

    const [loading, setLoading] =
        useState(false);

    function updateField(field, value) {
        setFormData((previous) => ({
            ...previous,
            [field]: value,
        }));
    }

    function validateStep(currentStep) {
        setError("");

        if (currentStep === 1) {
            if (!formData.productTypeId) {
                setError("Select a product to continue.");
                return false;
            }

            if (formData.ageYears === "") {
                setError("Enter the item's current age.");
                return false;
            }

            if (formData.repairCost === "") {
                setError("Enter the repair cost.");
                return false;
            }

            if (formData.priorRepairCount === "") {
                setError("Enter the number of previous repairs.");
                return false;
            }

            if (formData.expectedYearsAfterRepair === "") {
                setError("Enter how many years the repair should last.");
                return false;
            }
        }

        if (currentStep === 2) {
            if (formData.replacementCost === "") {
                setError("Enter the replacement cost.");
                return false;
            }

            if (formData.expectedLifespanYears === "") {
                setError("Enter the expected lifespan of a replacement.");
                return false;
            }
        }

        if (currentStep === 3 && includeEnergy) {
            if (formData.currentAnnualKwh === "") {
                setError("Enter the current annual energy usage.");
                return false;
            }

            if (formData.replacementAnnualKwh === "") {
                setError("Enter the replacement's annual energy usage.");
                return false;
            }

            if (formData.energyPricePerKwh === "") {
                setError("Enter the electricity price.");
                return false;
            }

            if (formData.annualEnergyPriceInflation === "") {
                setError("Enter the annual energy price increase.");
                return false;
            }
        }

        return true;
    }

    function nextStep() {
        if (!validateStep(step)) {
            return;
        }

        setStep((previous) => previous + 1);
    }

    function previousStep() {
        setError("");
        setStep((previous) => previous - 1);
    }

    async function runCalculation() {
        if (!validateStep(3)) {
            return;
        }

        setLoading(true);
        setError("");

        try {
            const requestData = {
                productTypeId: formData.productTypeId,
                ageYears: Number(formData.ageYears),
                repairCost: Number(formData.repairCost),
                priorRepairCount: Number(formData.priorRepairCount),

                priorRepairTotalCost:
                    formData.priorRepairTotalCost === ""
                        ? 0
                        : Number(formData.priorRepairTotalCost),

                expectedYearsAfterRepair: Number(formData.expectedYearsAfterRepair),
                replacementCost: Number(formData.replacementCost),

                expectedLifespanYears:
                    formData.expectedLifespanYears === ""
                        ? null
                        : Number(formData.expectedLifespanYears),

                residualValue:
                    formData.residualValue === ""
                        ? null
                        : Number(formData.residualValue),

                // currencyCode: formData.currencyCode,

                energy: includeEnergy
                    ? {
                        currentAnnualKwh: Number(formData.currentAnnualKwh),
                        replacementAnnualKwh: Number(formData.replacementAnnualKwh),
                        energyPricePerKwh: Number(formData.energyPricePerKwh),
                        annualEnergyPriceInflation: Number(formData.annualEnergyPriceInflation),
                    }
                    : null,
            };

            const result = await calculateRepairReplace(requestData);
            onResult(result);

        } catch (error) {
            setError(error.message || "Unable to calculate the result.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <PageLayout>
            <div className="mb-10">
                <h1 className="font-display text-[36px] leading-tight text-ink">
                    Repair or replace?
                </h1>
                <p className="mt-2 text-[15px] text-ink/60">
                    Answer a few questions about the item and the numbers involved.
                    The math is shown alongside the recommendation.
                </p>
            </div>

            <StepRail step={step} />

            {error && (
                <div className="mb-6 border-l-2 border-replace bg-replace/5 px-4 py-3 text-sm text-ink/80">
                    {error}
                </div>
            )}

            <div className="space-y-8">
                {step === 1 && (
                    <div className="space-y-8">
                        <ProductSelector
                            value={formData.productTypeId}
                            onChange={(value) => updateField("productTypeId", value)}
                        />
                        <div className={dividerClass} />
                        <RepairDetails
                            formData={formData}
                            onChange={updateField}
                        />
                    </div>
                )}

                {step === 2 && (
                    <ReplacementDetails
                        formData={formData}
                        onChange={updateField}
                    />
                )}

                {step === 3 && (
                    <EnergyDetails
                        enabled={includeEnergy}
                        formData={formData}
                        onToggle={setIncludeEnergy}
                        onChange={updateField}
                    />
                )}
            </div>

            <div className={"mt-10 flex items-center justify-between pt-8 " + dividerClass}>
                {step > 1 ? (
                    <button type="button" onClick={previousStep} className={buttonSecondaryClass}>
                        Back
                    </button>
                ) : (
                    <div />
                )}

                {step < 3 ? (
                    <button type="button" onClick={nextStep} className={buttonPrimaryClass}>
                        Continue
                    </button>
                ) : (
                    <button
                        type="button"
                        disabled={loading}
                        onClick={runCalculation}
                        className={buttonPrimaryClass}
                    >
                        {loading ? "Calculating…" : "Calculate decision"}
                    </button>
                )}
            </div>
        </PageLayout>
    );
}

export default CalculatorForm;