import { useState } from "react";

import Navbar from "../components/Navbar";
import CalculatorForm from "../components/calculator/CalculatorForm";
import ResultDisplay from "../components/calculator/ResultDisplay";

function Calculator() {
    const [result, setResult] = useState(null);

    return (
        <div className="min-h-screen bg-paper">
            <Navbar />

            {result ? (
                <ResultDisplay
                    result={result}
                    onReset={() => setResult(null)}
                />
            ) : (
                <CalculatorForm onResult={setResult} />
            )}
        </div>
    );
}

export default Calculator;