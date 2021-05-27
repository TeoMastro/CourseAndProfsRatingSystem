import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Main } from './components/Main';
import { Courses } from './components/Courses';
import { Login } from './components/Login';
import './custom.css'
export default class App extends Component {
    static displayName = App.name;
    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/main' component={Main} />
            </Layout>
        );
    }
}