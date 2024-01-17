/* eslint-disable react/prop-types */
import { Input } from "reactstrap";

const FormInput = ({ register, name, ...rest }) => {
    const { ref, ...registerField } = register(name);

    return <Input innerRef={ref} {...registerField} {...rest} />;
};

export { FormInput };
