import {
    Form,
    FormGroup,
    FormFeedback,
    Card,
    CardHeader,
    CardBody,
    Button,
    Label,
    Container,
    Row,
    Col,
} from "reactstrap";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useDispatch } from "react-redux";

import { register as registerUser } from "@/store";
import { history } from "@/helpers";
import { FormInput } from "@/components/FormInput";

import "./Register.scss";

// Validation Rules
const schema = yup.object().shape({
    userName: yup.string().min(3).max(32).required("Username is required"),
    email: yup.string().email().required("Email is required"),
    phoneNumber: yup.string().required("Phone Number is required"),
    password: yup.string().min(6).max(30).required("Password is required"),
    confirmPassword: yup
        .string()
        .oneOf([yup.ref("password"), null], "Passwords must match")
        .required("Please confirm your password"),
});

const Register = () => {
    const dispatch = useDispatch();

    const location = history.location;
    const navigate = history.navigate;

    // useForm
    const {
        register,
        handleSubmit,
        formState: { errors },
        reset,
        isSubmitting,
    } = useForm({ resolver: yupResolver(schema) });

    const onSubmitHandler = (data) => {
        const { userName, email, phoneNumber, password } = data;

        dispatch(registerUser({ userName, email, phoneNumber, password }));
        reset();

        const { from } = location.state || {
            from: {
                pathname: "/login",
            },
        };

        navigate(from);
    };

    return (
        <div className='register-form me-auto'>
            <Container className='register-form-container'>
                <Row className='register-form-content'>
                    <Col>
                        <Card className='register-form-card'>
                            <CardHeader className='register-form-header'>
                                <h1 className='register-form-title mt-2'>
                                    Register
                                </h1>
                            </CardHeader>
                            <CardBody className='register-form-body'>
                                <Form onSubmit={handleSubmit(onSubmitHandler)}>
                                    <FormGroup floating>
                                        <FormInput
                                            type='text'
                                            name='userName'
                                            register={register}
                                            id='userName'
                                            placeholder='Username'
                                            className={`mt-3 ${
                                                errors?.userName
                                                    ? "is-invalid"
                                                    : ""
                                            }`}
                                        />
                                        <Label for='userName'>Username</Label>
                                        {errors.userName && (
                                            <FormFeedback className='error-message'>
                                                {errors.userName?.message}
                                            </FormFeedback>
                                        )}
                                    </FormGroup>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='email'
                                            name='email'
                                            id='email'
                                            placeholder='Email'
                                            className={`mt-3 ${
                                                errors?.email
                                                    ? "is-invalid"
                                                    : ""
                                            }`}
                                        />
                                        <Label for='email'>Email</Label>
                                        {errors.email && (
                                            <FormFeedback className='error-message'>
                                                {errors.email?.message}
                                            </FormFeedback>
                                        )}
                                    </FormGroup>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='tel'
                                            name='phoneNumber'
                                            id='phoneNumber'
                                            placeholder='Phone Number'
                                            className={`mt-3 ${
                                                errors?.phoneNumber
                                                    ? "is-invalid"
                                                    : ""
                                            }`}
                                        />
                                        <Label for='phoneNumber'>
                                            Phone Number
                                        </Label>
                                        {errors.phoneNumber && (
                                            <FormFeedback className='error-message'>
                                                {errors.phoneNumber?.message}
                                            </FormFeedback>
                                        )}
                                    </FormGroup>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='password'
                                            id='password'
                                            name='password'
                                            placeholder='Password'
                                            className={`mt-3 ${
                                                errors?.password
                                                    ? "is-invalid"
                                                    : ""
                                            }`}
                                        />
                                        <Label for='password'>Password</Label>
                                        {errors.password && (
                                            <FormFeedback className='error-message'>
                                                {errors.password?.message}
                                            </FormFeedback>
                                        )}
                                    </FormGroup>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='password'
                                            id='confirmPassword'
                                            name='confirmPassword'
                                            placeholder='Confirm Password'
                                            className={`mt-3 ${
                                                errors?.confirmPassword
                                                    ? "is-invalid"
                                                    : ""
                                            }`}
                                        />
                                        <Label for='confirmPassword'>
                                            Confirm Password
                                        </Label>
                                        {errors.confirmPassword && (
                                            <FormFeedback className='error-message'>
                                                {
                                                    errors.confirmPassword
                                                        ?.message
                                                }
                                            </FormFeedback>
                                        )}
                                    </FormGroup>
                                    <Button
                                        type='submit'
                                        className='register-form-button mb-3'
                                        color='primary'
                                        disabled={isSubmitting}
                                    >
                                        {isSubmitting ? (
                                            <span className='spinner-border spinner-border-sm me-2'></span>
                                        ) : (
                                            "Register"
                                        )}
                                    </Button>
                                    <Button
                                        type='button'
                                        className='btn-link login-link mb-3'
                                        onClick={() => navigate("/login")}
                                    >
                                        Login
                                    </Button>
                                </Form>
                            </CardBody>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </div>
    );
};

export { Register };
