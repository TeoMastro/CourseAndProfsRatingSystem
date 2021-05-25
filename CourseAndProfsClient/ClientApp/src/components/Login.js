import React, { useState } from 'react';
import './Login.css';
export class Login extends React.Component {
    handleSubmitLogin() {

    }
    onFormChange() {

    }
    render() {
        return (
            <div className="backForm">
                <h2 className="formTitle" >Login to your account</h2>
                <form className="divForm" onSubmit={this.handleSubmitLogin} onChange={this.onFormChange} autoComplete="off">
                    <label className="formText">Username</label>
                    <input className="formInput" type="text" name="username" placeholder="Your username here" /*value={userNameLogin} onChange={e => setUserNameLogin(e.target.value)}*//>
                    <label className="formText">Password</label>
                    <input className="formInput" type="password" name="password" placeholder="Your password here" /*value={passwordLogin} onChange={e => setPasswordLogin(e.target.value)}*/ />
                    <input className="formButton" type="submit" value="Login" /*disabled={}*/ />
                </form>
            </div>
        );
    }
}
export default Login;