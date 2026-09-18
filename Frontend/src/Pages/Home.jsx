import Navbar from "../Components/Navbar";
import Hero from "../Components/Hero";
import HowItWorks from "../Components/HowItWorks";
import Footer from "../Components/Footer";

function Home() {
    return (
        <div className="min-h-screen bg-white">

            {/* <Navbar /> */}

            <main>
                <Hero />
                <HowItWorks />
            </main>

            {/* <Footer /> */}

        </div>
    );
}

export default Home;