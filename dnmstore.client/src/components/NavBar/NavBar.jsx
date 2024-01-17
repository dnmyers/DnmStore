import { useState } from "react";
import {
    Collapse,
    Navbar,
    NavbarToggler,
    NavbarBrand,
    Nav,
    NavItem,
    NavLink,
} from "reactstrap";
import { motion } from "framer-motion";
import { useSelector } from "react-redux";

import { selectIsAuthenticated } from "@/store";
import { history } from "@/helpers";

const NavBar = () => {
    const navigate = history.navigate;
    const location = history.location;

    const isAuthenticated = useSelector(selectIsAuthenticated);

    const [isOpen, setIsOpen] = useState(false);

    const toggle = () => setIsOpen(!isOpen);

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/login");
    };

    return (
        <Navbar color='dark' dark className='mb-2 navbar-expand-md'>
            <NavbarBrand href='/'>
                <motion.div
                    initial={{
                        y: -10,
                        scale: 0,
                        opacity: 0,
                    }}
                    animate={{
                        y: 0,
                        scale: 1,
                        opacity: 1,
                    }}
                    whileHover={{
                        scale: 1.1,
                    }}
                    whileTap={{
                        scale: 0.95,
                    }}
                    className='brand'
                >
                    DNM
                </motion.div>
            </NavbarBrand>
            <NavbarToggler onClick={toggle} />
            <Collapse isOpen={isOpen} navbar>
                <Nav className='mr-auto' navbar>
                    <NavItem>
                        <NavLink href='/'>Home</NavLink>
                    </NavItem>
                    {location.pathname !== "/login" && !isAuthenticated && (
                        <NavItem>
                            <NavLink href='/login'>Login</NavLink>
                        </NavItem>
                    )}
                    {location.pathname !== "/register" && !isAuthenticated && (
                        <NavItem>
                            <NavLink href='/register'>Register</NavLink>
                        </NavItem>
                    )}
                    {isAuthenticated && (
                        <NavItem>
                            <NavLink href='#' onClick={handleLogout}>
                                Logout
                            </NavLink>
                        </NavItem>
                    )}
                </Nav>
            </Collapse>
        </Navbar>
    );
};

export { NavBar };
