import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const baseUrl = "https://localhost:7197";

export const login = createAsyncThunk(
    "auth/login",
    async ({ userName, password }) => {
        return axios
            .post(`${baseUrl}/authorization/token`, { userName, password })
            .then((response) => {
                const user = JSON.stringify(response.data);
                const userId = response.data.userId;
                const refreshToken = response.data.refreshToken;
                const authorizationToken = response.data.authorizationToken;

                localStorage.setItem("user", user);
                localStorage.setItem("userId", userId);
                localStorage.setItem("refreshToken", refreshToken);
                localStorage.setItem("authorizationToken", authorizationToken);

                if (authorizationToken) {
                    axios.defaults.headers.common[
                        "Authorization"
                    ] = `Bearer ${authorizationToken}`;
                }

                return { user, userId, refreshToken, authorizationToken };
            })
            .catch((err) => {
                localStorage.removeItem("user");
                localStorage.removeItem("userId");
                localStorage.removeItem("refreshToken");
                localStorage.removeItem("authorizationToken");

                console.error(err);
                throw err;
            });
    }
);

export const register = createAsyncThunk(
    "auth/register",
    async ({ userName, email, phoneNumber, password }) => {
        return axios
            .post(`${baseUrl}/register`, {
                userName,
                email,
                phoneNumber,
                password,
            })
            .then((response) => {
                const status = response.status;

                return status;
            })
            .catch((err) => {
                console.error(err);
                throw err;
            });
    }
);

export const authSlice = createSlice({
    name: "auth",
    initialState: {
        user: {},
        userId: null,
        refreshToken: null,
        authorizationToken: null,
        isAuthenticated: false,
        isLoading: false,
        message: null,
        error: null,
    },
    reducers: {
        logout: (state) => {
            localStorage.removeItem("user");
            localStorage.removeItem("userId");
            localStorage.removeItem("refreshToken");
            localStorage.removeItem("authorizationToken");

            state.user = null;
            state.userId = null;
            state.refreshToken = null;
            state.authorizationToken = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(login.pending, (state) => {
                state.isLoading = true;
            })
            .addCase(login.fulfilled, (state, action) => {
                state.user = action.payload.user;
                state.userId = action.payload.userId;
                state.refreshToken = action.payload.refreshToken;
                state.authorizationToken = action.payload.authorizationToken;
                state.isLoading = false;
                state.isAuthenticated = true;
                state.message = "User logged in successfully";
            })
            .addCase(login.rejected, (state, action) => {
                state.isLoading = false;
                state.isAuthenticated = false;
                state.message =
                    "An error occurred while trying to log in.  Please try again.";
                state.error =
                    action.error?.message ||
                    action.error ||
                    "An error occurred while trying to log the user in";
            })
            .addCase(register.pending, (state) => {
                state.isLoading = true;
            })
            .addCase(register.fulfilled, (state) => {
                state.isLoading = false;
                state.message = "User registered successfully";
            })
            .addCase(register.rejected, (state, action) => {
                state.isLoading = false;
                state.message =
                    "An error occurred while trying to register the user.  Please try again.";
                state.error =
                    action.error?.message ||
                    action.error ||
                    "An error occurred while trying to register the user";
            });
    },
});

export const { logout } = authSlice.actions;

export const selectAuth = (state) => state.auth;
export const selectUser = (state) => state.auth.user;
export const selectUserId = (state) => state.auth.userId;
export const selectRefreshToken = (state) => state.auth.refreshToken;
export const selectAuthorizationToken = (state) =>
    state.auth.authorizationToken;
export const selectIsLoading = (state) => state.auth.isLoading;
export const selectIsAuthenticated = (state) => state.auth.isAuthenticated;
export const selectMessage = (state) => state.auth.message;
export const selectError = (state) => state.auth.error;

export default authSlice.reducer;
