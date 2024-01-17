import { useEffect } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { Provider } from "react-redux";

import { NavigationWrapper } from "@/helpers";
import { NavBar } from "@/components/NavBar/NavBar";
import { store } from "@/store";

import { Home } from "@/pages/Home";
import { Login } from "@/pages/Login";
import { Register } from "@/pages/Register";
import { NotFound } from "@/pages/NotFound";

import "bootstrap/dist/css/bootstrap.min.css";
import "./App.scss";

const App = () => {
    useEffect(() => {
        const token = localStorage.getItem("authorizationToken");

        // Verify if the token is expired
        if (token) {
            const tokenExpiration = jwtDecode(token).exp;
            const currentTimestamp = Math.floor(Date.now() / 1000); // Convert to seconds

            // If the token is expired, clear localStorage
            if (tokenExpiration < currentTimestamp) {
                localStorage.clear();
            }
        }
    }, []);

    return (
        <BrowserRouter>
            <Provider store={store}>
                <NavigationWrapper>
                    <div className='app'>
                        <NavBar />
                        <Routes>
                            <Route path='/' element={<Home />} />
                            <Route path='/login' element={<Login />} />
                            <Route path='/register' element={<Register />} />
                            <Route path='*' element={<NotFound />} />
                        </Routes>
                    </div>
                </NavigationWrapper>
            </Provider>
        </BrowserRouter>
    );
};

export default App;
