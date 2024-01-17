import { useNavigate, useLocation } from "react-router-dom";

import { history } from "@/helpers";

// eslint-disable-next-line react/prop-types
const NavigationWrapper = ({ children }) => {
    // Init custom history object to allow navigation from
    // anywhere in the react app (inside or outside of components)
    history.navigate = useNavigate();
    history.location = useLocation();

    return <>{children}</>;
};

export { NavigationWrapper };
