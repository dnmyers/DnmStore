import {
    Form,
    FormGroup,
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
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { useSelector, useDispatch } from "react-redux";

import { login, selectMessage, selectIsAuthenticated } from "@/store";
import { history } from "@/helpers";
import { FormInput } from "@/components/FormInput";

import "./Login.scss";

// Validation Rules
const schema = yup.object().shape({
    userName: yup.string().min(3).required("Email is required"),
    password: yup.string().required("Password is required"),
});

const Login = () => {
    const dispatch = useDispatch();

    const location = history.location;
    const navigate = history.navigate;

    const message = useSelector(selectMessage);
    const isAuthenticated = useSelector(selectIsAuthenticated);

    // useForm
    const {
        register,
        handleSubmit,
        formState: { errors },
        reset,
        isSubmitting,
    } = useForm({ resolver: yupResolver(schema) });

    const onSubmitHandler = (data) => {
        const { userName, password } = data;

        dispatch(login({ userName, password }));
        reset();

        const { from } = location.state || {
            from: {
                pathname: "/",
            },
        };

        navigate(from);
    };

    return (
        <div className='login me-auto'>
            <Container className='login-form-container'>
                {!isAuthenticated && message && (
                    <Row>
                        <Col>
                            <p
                                className={`message ${
                                    message.includes("error")
                                        ? "error-message"
                                        : ""
                                }`}
                            >
                                {message}
                            </p>
                        </Col>
                    </Row>
                )}
                <Row className='login-form-content'>
                    <Col>
                        <Card className='login-form-card'>
                            <CardHeader className='login-form-title'>
                                <h1 className='login-form-header mt-2'>
                                    Login
                                </h1>
                            </CardHeader>
                            <CardBody className='login-form-body'>
                                <Form onSubmit={handleSubmit(onSubmitHandler)}>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='test'
                                            name='userName'
                                            id='userName'
                                            placeholder='Username'
                                            className='mt-3'
                                        />
                                        <Label for='userName'>Username</Label>
                                        {errors.userName && (
                                            <p className='error'>
                                                {errors.userName?.message}
                                            </p>
                                        )}
                                    </FormGroup>
                                    <FormGroup floating>
                                        <FormInput
                                            register={register}
                                            type='password'
                                            name='password'
                                            id='password'
                                            placeholder='Password'
                                            className='mt-3'
                                        />
                                        <Label for='password'>Password</Label>
                                        {errors.password && (
                                            <p className='error'>
                                                {errors.password?.message}
                                            </p>
                                        )}
                                    </FormGroup>
                                    <FormGroup check>
                                        <Label check>
                                            <FormInput
                                                register={register}
                                                type='checkbox'
                                                name='rememberMe'
                                                id='rememberMe'
                                                className='mb-3'
                                            />
                                            Remember me
                                        </Label>
                                    </FormGroup>
                                    <Button
                                        type='submit'
                                        color='primary'
                                        disabled={isSubmitting}
                                        className='login-button'
                                    >
                                        {isSubmitting ? (
                                            <span className='spinner-border spinner-border-sm me-2'></span>
                                        ) : (
                                            "Login"
                                        )}
                                    </Button>
                                    <Button
                                        type='button'
                                        className='btn-link register-link m-2'
                                        onClick={() => navigate("/register")}
                                    >
                                        Register
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

export { Login };
