import { useState } from "react";

import Navbar from "../Components/Navbar";
import CalculatorForm from "../Components/Calculator/CalculatorForm";
import ResultDisplay from "../Components/Calculator/ResultDisplay";

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