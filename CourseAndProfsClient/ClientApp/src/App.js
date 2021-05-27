import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Professors } from './components/Professors';
import { Myreviews } from './components/Myreviews';
import { Login } from './components/Login';
import './custom.css'
export default class App extends Component {
    static displayName = App.name;
    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/professors' component={Professors} />
                <Route path='/myreviews' component={Myreviews} />
                <Route path='/login' component={Login} />
            </Layout>
        );
    }
}