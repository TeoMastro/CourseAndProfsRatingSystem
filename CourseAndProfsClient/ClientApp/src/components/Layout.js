import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
export class Layout extends Component {
    static displayName = Layout.name;
    render() {
        return (
            <div className="divlay" style={{ width: '100vw', height: '100vh', padding: '0px', margin: '0px' }}>
                <NavMenu />
                <Container className="contlay" style={{ width: '100%', heigth: '100%', padding: '0px', margin: '0px'}}>
                    {this.props.children}
                </Container>
            </div>
        );
    }
}