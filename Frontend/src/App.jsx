import { BrowserRouter, Routes, Route } from "react-router-dom";

import Home from "./Pages/Home";
import Calculator from "./Pages/Calculator";

function App() {
    return (
        <BrowserRouter>
            <Routes>

                <Route
                    path="/"
                    element={<Home />}
                />

                <Route
                    path="/calculator"
                    element={<Calculator />}
                />

            </Routes>
        </BrowserRouter>
    );
}

export default App;